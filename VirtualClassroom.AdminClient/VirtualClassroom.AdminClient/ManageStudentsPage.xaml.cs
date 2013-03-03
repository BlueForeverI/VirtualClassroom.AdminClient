using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageStudentsPage.xaml
    /// </summary>
    public partial class ManageStudentsPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageStudentsPage()
        {
            try
            {
                InitializeComponent();

                this.dataGridStudents.Items.Clear();
                this.dataGridStudents.ItemsSource = client.GetStudentViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddStudentWindow addStudentWindow = new AddStudentWindow();
                if (addStudentWindow.ShowDialog() == true)
                {
                    Student student = new Student();
                    student.Username = addStudentWindow.Username;
                    student.FirstName = addStudentWindow.FirstName;
                    student.MiddleName = addStudentWindow.MiddleName;
                    student.LastName = addStudentWindow.LastName;
                    student.EGN = addStudentWindow.EGN;
                    student.ClassId = addStudentWindow.ClassId;

                    string secret = Crypto.GenerateRandomSecret(30);
                    student.Username = Crypto.EncryptStringAES(student.Username, secret);

                    client.RegisterStudent(student,
                        Crypto.EncryptStringAES(addStudentWindow.Password, secret), secret);

                    MessageBox.Show("Ученикът беше добавен успешно");
                    this.dataGridStudents.ItemsSource = client.GetStudentViews();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        MessageBox.Show("Учениците бяха премахнати успешно");
                        this.dataGridStudents.ItemsSource = client.GetStudentViews();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
