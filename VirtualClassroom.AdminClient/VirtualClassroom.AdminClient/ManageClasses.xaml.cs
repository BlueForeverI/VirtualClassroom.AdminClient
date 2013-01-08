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
    public partial class ManageClasses : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageClasses()
        {
            InitializeComponent();
            this.dataGridClasses.Items.Clear();
            this.dataGridClasses.ItemsSource = client.GetClasses();
        }

        private void btnAddClass_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRemoveClass_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
