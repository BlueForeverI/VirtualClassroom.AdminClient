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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = client.LoginAdmin(txtUsername.Text, txtPassword.Text);
            if(admin == null)
            {
                MessageBox.Show("Wrong username or password!", "Invalid login",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
