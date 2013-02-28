using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VirtualClassroom.AdminClient.AdminService;
using VirtualClassroom.Services.Services;

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
            InitializeComponent();
            this.dataGridTeachers.Items.Clear();
            this.dataGridTeachers.ItemsSource = client.GetTeachers();
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

                    MessageBox.Show("Teacher added successfully!");
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
                    MessageBox.Show("You have not selected any teachers!");
                }
                else
                {
                    if (MessageBox.Show("Do you really want to remove these teachers?", "Are you sure?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveTeachers(teachers.ToArray());
                        MessageBox.Show("Teachers removed successfully!");
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
