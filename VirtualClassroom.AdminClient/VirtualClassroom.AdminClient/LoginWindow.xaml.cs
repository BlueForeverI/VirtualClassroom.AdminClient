using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using VirtualClassroom.AdminClient.AdminService;
using VirtualClassroom.Services.Services;
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private AdminServiceClient client;

        public LoginWindow()
        {
            ClientManager.CloseClient();
            client = ClientManager.GetClient();
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            Admin admin = new Admin();

            string username = txtUsername.Text;
            string password = txtPassword.Password;

            worker.DoWork += (o, ea) =>
                                 {
                                     try
                                     {
                                         string secret = Crypto.GenerateRandomSecret(30);
                                         admin = client.LoginAdmin(Crypto.EncryptStringAES(username, secret),
                                                                   Crypto.EncryptStringAES(password, secret),
                                                                   secret);
                                     }
                                     catch (Exception ex)
                                     {
                                         MessageBox.Show(ex.Message);
                                     }
                                 };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.busyIndicator.IsBusy = false;
                if (admin == null)
                {
                    MessageBox.Show("Wrong username or password!", "Invalid login",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    this.DialogResult = true;
                    this.Close();
                }
            };

            busyIndicator.IsBusy = true;
            worker.RunWorkerAsync();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
