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
    /// Interaction logic for ManageClasses.xaml
    /// </summary>
    public partial class ManageClassesPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageClassesPage()
        {
            InitializeComponent();
            this.dataGridClasses.Items.Clear();
            this.dataGridClasses.ItemsSource = client.GetClasses();
        }

        private void btnAddClass_Click(object sender, RoutedEventArgs e)
        {
            AddClassWindow addClassWindow = new AddClassWindow();
            if(addClassWindow.ShowDialog() == true)
            {
                string letter = addClassWindow.Letter;
                int number = addClassWindow.Number;
                client.AddClass(new Class(){Letter = letter, Number = number});
                MessageBox.Show("Class added successfully!");
                this.dataGridClasses.ItemsSource = client.GetClasses();
            }
        }

        private void btnRemoveClass_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
