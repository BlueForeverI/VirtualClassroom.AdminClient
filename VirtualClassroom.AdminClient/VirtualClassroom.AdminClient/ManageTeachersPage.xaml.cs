using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageTeachersPage.xaml
    /// </summary>
    public partial class ManageTeachersPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageTeachersPage()
        {
            try
            {
                InitializeComponent();
                this.dataGridTeachers.Items.Clear();
                this.dataGridTeachers.ItemsSource = client.GetTeachers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

                    MessageBox.Show("Учителят беше добавен успешно");
                    this.dataGridTeachers.ItemsSource = client.GetTeachers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        MessageBox.Show("Учителите бяха премахнати успешно");
                        this.dataGridTeachers.ItemsSource = client.GetTeachers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
