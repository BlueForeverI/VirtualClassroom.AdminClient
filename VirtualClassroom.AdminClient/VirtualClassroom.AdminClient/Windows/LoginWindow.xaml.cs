using System;
using System.ComponentModel;
using System.Windows;
using VirtualClassroom.AdminClient.AdminService;
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

            //call the service in another thread, while
            //showing user-friendly message
            worker.DoWork += (o, ea) =>
            {
                try
                {
                    //encrypt login details
                    string secret = Crypto.GenerateRandomSecret();
                    admin = client.LoginAdmin(Crypto.EncryptStringAES(username, secret),
                                            Crypto.EncryptStringAES(password, secret),
                                            secret);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                        "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.busyIndicator.IsBusy = false;
                if (admin == null)
                {
                    MessageBox.Show("Невалидно потребителско име или парола", "Грешка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    // login is successfull
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
