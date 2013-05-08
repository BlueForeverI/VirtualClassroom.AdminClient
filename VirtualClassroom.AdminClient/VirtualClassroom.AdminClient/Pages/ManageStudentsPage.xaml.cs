using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageStudentsPage.xaml
    /// </summary>
    public partial class ManageStudentsPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        private void UpdateStudentViews()
        {
            Thread thread = new Thread(() => Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var students = client.GetStudentViews();
                    this.dataGridStudents.ItemsSource = students;
                })));
            thread.Start();
        }

        public ManageStudentsPage()
        {
            try
            {
                InitializeComponent();
                UpdateStudentViews();

            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddStudentWindow addStudentWindow = new AddStudentWindow();
                if (addStudentWindow.ShowDialog() == true)
                {
                    Student student = new Student()
                    {
                        Username = addStudentWindow.Username,
                        FirstName = addStudentWindow.FirstName,
                        MiddleName = addStudentWindow.MiddleName,
                        LastName = addStudentWindow.LastName,
                        EGN = addStudentWindow.EGN,
                        ClassId = addStudentWindow.ClassId
                    };

                    string secret = Crypto.GenerateRandomSecret();
                    student.Username = Crypto.EncryptStringAES(student.Username, secret);
                    string password = Crypto.EncryptStringAES(addStudentWindow.Password, secret);

                    if(client.RegisterStudent(student, password, secret))
                    {
                        UpdateStudentViews();
                        MessageBox.Show("Ученикът беше добавен успешно");   
                    }
                    else
                    {
                        MessageBox.Show("Ученикът не е валиден или вече съществува");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var students = new List<Student>();
                foreach (var selected in this.dataGridStudents.SelectedItems)
                {
                    dynamic item = selected;
                    int id = int.Parse(item.Id.ToString());
                    students.Add(new Student() { Id = id });
                }

                if(students.Count == 0)
                {
                    MessageBox.Show("Не сте избрали ученици");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези ученици?",
                        "Сигурен ли сте?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveStudents(students.ToArray());
                        UpdateStudentViews();
                        MessageBox.Show("Учениците бяха премахнати успешно");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dataGridStudents.SelectedIndex == -1)
                {
                    MessageBox.Show("Не сте избрали ученик");
                }
                else if (this.dataGridStudents.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Трябва да изберете точно един ученик");
                }
                else
                {
                    int studentId = (this.dataGridStudents.SelectedItem as dynamic).Id;

                    EditStudentWindow editStudentWindow = new EditStudentWindow(
                        client.GetStudentById(studentId));
                    if (editStudentWindow.ShowDialog() == true)
                    {
                        var student = editStudentWindow.Student;
                        string secret = Crypto.GenerateRandomSecret();
                        student.Username = Crypto.EncryptStringAES(student.Username, secret);
                        student.PasswordHash = Crypto.EncryptStringAES(student.PasswordHash, secret);

                        client.EditStudent(studentId, student, secret);
                        UpdateStudentViews();
                        MessageBox.Show("Ученикът беше редактиран успешно");
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImportStudents_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MS Access files (*.mdb)|*.mdb";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName.EndsWith(".mdb"))
                {
                    try
                    {
                        BackgroundWorker worker = new BackgroundWorker();

                        worker.DoWork += (o, ea) =>
                        {
                            string secret = Crypto.GenerateRandomSecret();
                            var students = AccessDatabaseHelper.GetStudentsFromAccess(
                                dialog.FileName, secret, client.GetClasses());

                            client.RegisterStudents(students.ToArray(), secret);
                            UpdateStudentViews();
                        };

                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            this.busyIndicator.IsBusy = false;
                            MessageBox.Show("Учениците бяха импортирани успешно");
                        };

                        this.busyIndicator.IsBusy = true;
                        worker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Грешка при импортирането. Учениците не бяха добавени");
                    }
                }
                else
                {
                    MessageBox.Show("Програмата поддържа само MS Access файлове (.mdb)");
                }
            }
        }
    }
}
