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
    /// Interaction logic for ManageSubjectsPage.xaml
    /// </summary>
    public partial class ManageSubjectsPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageSubjectsPage()
        {
            InitializeComponent();

            this.dataGridSubjects.Items.Clear();
            UpdateDataGrid();
        }

        private void UpdateDataGrid()
        {
            var subjects = client.GetSubjects();
            var teachers = client.GetTeachers();

            var list = (from s in subjects
                        from t in teachers
                        where s.TeacherId == t.Id
                        select new
                                   {
                                       Id = s.Id,
                                       Name = s.Name,
                                       FullName = t.FirstName + " " + t.LastName
                                   });

            this.dataGridSubjects.ItemsSource = list;
        }

        private void btnAddSubject_Click(object sender, RoutedEventArgs e)
        {
            AddSubjectWindow addSubjectWindow = new AddSubjectWindow();
            if(addSubjectWindow.ShowDialog() == true)
            {
                Subject subject = new Subject();
                subject.Name = addSubjectWindow.Name;
                subject.TeacherId = addSubjectWindow.TeacherId;

                client.AddSubject(subject);
                MessageBox.Show("Subject added successfully!");
                UpdateDataGrid();
            }
        }

        private void btnRemoveSubject_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Do you really want to remove this subject?", "Are you sure?",
                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                dynamic selected = this.dataGridSubjects.SelectedItem;
                int id = int.Parse(selected.Id.ToString());

                Subject subject = new Subject() { Id = id };
                client.RemoveSubject(subject);
                MessageBox.Show("Subject removed successfully!");
                UpdateDataGrid();
            }
        }
    }
}
