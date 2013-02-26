using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        public const int MAX_NAME_LENGTH = 32;
        private const int MIN_PASS_LENGTH = 4;
        private const int MAX_PASS_LENGTH = 32;

        public AddTeacherWindow()
        {
            InitializeComponent();
        }

        private void ValidateInput()
        {
            if (string.IsNullOrEmpty(this.txtUsername.Text) || string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                throw new Exception("The username cannot be an empty string!");
            }

            if (!Regex.IsMatch(this.txtUsername.Text, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*"))
            {
                throw new Exception("The username is not in the correct format!");
            }

            if (this.txtUsername.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format("The username cannot be longer than {0} characters!", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtFirstName.Text) || string.IsNullOrWhiteSpace(this.txtFirstName.Text))
            {
                throw new Exception("The first name cannot be an empty string!");
            }

            if (!Regex.IsMatch(this.txtFirstName.Text, "[a-zA-Z]+"))
            {
                throw new Exception("The first name is not in the correct format!");
            }

            if (this.txtFirstName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format("The first name cannot be longer than {0} characters", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtMiddleName.Text) || string.IsNullOrWhiteSpace(this.txtMiddleName.Text))
            {
                throw new Exception("The middle name cannot be an empty string!");
            }

            if (!Regex.IsMatch(this.txtMiddleName.Text, "[a-zA-Z]+"))
            {
                throw new Exception("The middle name is not in the correct format!");
            }

            if (this.txtMiddleName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format("The middle name cannot be longer than {0} characters", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtLastName.Text) || string.IsNullOrWhiteSpace(this.txtLastName.Text))
            {
                throw new Exception("The last name cannot be an empty string!");
            }

            if (!Regex.IsMatch(this.txtLastName.Text, "[a-zA-Z]+"))
            {
                throw new Exception("The last name is not in the correct format!");
            }

            if (this.txtLastName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format("The last name cannot be longer than {0} characters", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtPassword.Password) || string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                throw new Exception("The password cannot be an empty string!");
            }

            if (this.txtPassword.Password.Length < MIN_PASS_LENGTH || this.txtPassword.Password.Length > MAX_PASS_LENGTH)
            {
                throw new Exception(string.Format("The password should be between {0} and {1} characters",
                    MIN_PASS_LENGTH, MAX_PASS_LENGTH));
            }
        }

        public string Username { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleName { get; private set; }
        public string LastName { get; private set; }
        public string Password { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInput();

                this.Username = this.txtUsername.Text;
                this.FirstName = this.txtFirstName.Text;
                this.MiddleName = this.txtMiddleName.Text;
                this.LastName = this.txtLastName.Text;
                this.Password = this.txtPassword.Password;

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Invalid input");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
