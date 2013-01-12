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
using System.Windows.Shapes;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for AddToClassWindow.xaml
    /// </summary>
    public partial class AddToClassWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public AddToClassWindow()
        {
            InitializeComponent();

            this.comboClasses.Items.Clear();
            this.comboClasses.ItemsSource = client.GetClasses();
        }

        public int ClassId { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.ClassId = (this.comboClasses.SelectedItem as Class).Id;
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
