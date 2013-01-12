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
            UpdateDataGrid();
        }

        void UpdateDataGrid()
        {
            var students = client.GetStudents();
            var classes = client.GetClasses();

            var list = (from s in students
                        from c in classes
                        where c.Id == s.ClassId
                        select new
                        {
                            Id = s.Id,
                            Username = s.Username,
                            FirstName = s.FirstName,
                            MiddleName = s.MiddleName,
                            LastName = s.LastName,
                            EGN = s.EGN,
                            Class = string.Format("{0} '{1}'", c.Number, c.Letter)
                        });

            this.dataGridStudents.ItemsSource = list;
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            AddStudentWindow addStudentWindow = new AddStudentWindow();
            if(addStudentWindow.ShowDialog() == true)
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
                UpdateDataGrid();
            }
        }

        private void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Do you really want to remove these students?", 
                "Are you sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var students = new List<Student>();
                foreach (var selected in this.dataGridStudents.SelectedItems)
                {
                    dynamic item = selected;
                    int id = int.Parse(item.Id.ToString());
                    students.Add(new Student(){ Id = id });
                }

                client.RemoveStudents(students.ToArray());
                MessageBox.Show("Students removed successfully!");
                UpdateDataGrid();
            }
        }
    }
}
