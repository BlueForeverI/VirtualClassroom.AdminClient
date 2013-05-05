using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageTeachersPage.xaml
    /// </summary>
    public partial class ManageTeachersPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        private void UpdateTeacherViews()
        {
            Thread thread = new Thread(() => Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var teachers = client.GetTeachers();
                    this.dataGridTeachers.ItemsSource = teachers;
                })));
            thread.Start();
        }

        public ManageTeachersPage()
        {
            try
            {
                InitializeComponent();
                UpdateTeacherViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTeacherWindow addTeacherWindow = new AddTeacherWindow();
                if (addTeacherWindow.ShowDialog() == true)
                {
                    string username = addTeacherWindow.Username;
                    string firstName = addTeacherWindow.FirstName;
                    string middleName = addTeacherWindow.MiddleName;
                    string lastName = addTeacherWindow.LastName;
                    string password = addTeacherWindow.Password;

                    Teacher teacher = new Teacher()
                    {
                        Username = username,
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName
                    };

                    string secret = Crypto.GenerateRandomSecret(30);
                    teacher.Username = Crypto.EncryptStringAES(teacher.Username, secret);

                    client.RegisterTeacher(teacher,
                        Crypto.EncryptStringAES(addTeacherWindow.Password, secret),
                        secret);

                    UpdateTeacherViews();
                    MessageBox.Show("Учителят беше добавен успешно");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var teachers = new List<Teacher>();
                foreach (var selected in this.dataGridTeachers.SelectedItems)
                {
                    teachers.Add(new Teacher() { Id = (selected as Teacher).Id });
                }

                if (teachers.Count == 0)
                {
                    MessageBox.Show("Не сте избрали учители");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези учители?", "Сигурен ли сте?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveTeachers(teachers.ToArray());
                        UpdateTeacherViews();
                        MessageBox.Show("Учителите бяха премахнати успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.dataGridTeachers.SelectedIndex == -1)
                {
                    MessageBox.Show("Не сте избрали учител");
                }
                else if(this.dataGridTeachers.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Трябва да изберете един учител");
                }
                else
                {
                    int teacherId = (this.dataGridTeachers.SelectedItem as dynamic).Id;

                    EditTeacherWindow editTeacherWindow = new EditTeacherWindow(
                        client.GetTeacherById(teacherId));

                    if(editTeacherWindow.ShowDialog() == true)
                    {
                        var teacher = editTeacherWindow.Teacher;
                        string secret = Crypto.GenerateRandomSecret(40);
                        teacher.Username = Crypto.EncryptStringAES(teacher.Username, secret);
                        teacher.PasswordHash = Crypto.EncryptStringAES(teacher.PasswordHash, secret);

                        client.EditTeacher(teacherId, teacher, secret);
                        UpdateTeacherViews();
                        MessageBox.Show("Учителят беше редактиран успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImportTeachers_Click(object sender, RoutedEventArgs e)
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
                            string secret = Crypto.GenerateRandomSecret(40);
                            var teachers = 
                                AccessDatabaseHelper.GetTeachersFromAccess(dialog.FileName, secret);

                            client.RegisterTeachers(teachers, secret);
                            UpdateTeacherViews();
                        };

                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            this.busyIndicator.IsBusy = false;
                            MessageBox.Show("Учителите бяха импортирани успешно");
                        };

                        this.busyIndicator.IsBusy = true;
                        worker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Грешка при импортирането. Учителите не бяха добавени");
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
