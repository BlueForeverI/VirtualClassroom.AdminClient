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
    /// Interaction logic for AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public AddStudentWindow()
        {
            InitializeComponent();

            this.comboClasses.Items.Clear();
            this.comboClasses.ItemsSource = client.GetClasses();
        }

        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string EGN { get; private set; }
        public string Password { get; private set; }
        public int ClassId { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.Username = this.txtUsername.Text;
            this.FirstName = this.txtFirstName.Text;
            this.MiddleName = this.txtMiddleName.Text;
            this.LastName = this.txtLastName.Text;
            this.Password = this.txtPassword.Text;
            this.EGN = this.txtEgn.Text;
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
