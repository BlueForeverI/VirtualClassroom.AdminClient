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
            InitializeComponent();

            this.dataGridStudents.Items.Clear();
            this.dataGridStudents.ItemsSource = client.GetStudentViews();
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

                    client.RegisterStudent(student, addStudentWindow.Password);
                    MessageBox.Show("Student added successfully!");
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
                    MessageBox.Show("You have not selected any students");
                }
                else
                {
                    if (MessageBox.Show("Do you really want to remove these students?",
                        "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveStudents(students.ToArray());
                        MessageBox.Show("Students removed successfully!");
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
