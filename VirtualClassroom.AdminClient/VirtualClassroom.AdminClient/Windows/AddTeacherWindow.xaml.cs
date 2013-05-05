using System;
using System.Text.RegularExpressions;
using System.Windows;

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

        /// <summary>
        /// Checks whether the user input is valid
        /// </summary>
        private void ValidateInput()
        {
            if (string.IsNullOrEmpty(this.txtUsername.Text) 
                || string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                throw new Exception("Не сте въвели потребителско име");
            }

            if (!Regex.IsMatch(this.txtUsername.Text, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*$"))
            {
                throw new Exception("Невалидно потребителско име");
            }

            if (this.txtUsername.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Потребителското име не може да е по-дълго от  {0} символа!", 
                    MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtFirstName.Text) 
                || string.IsNullOrWhiteSpace(this.txtFirstName.Text))
            {
                throw new Exception("Не сте въвели име");
            }

            if (!Regex.IsMatch(this.txtFirstName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Невалидно име");
            }

            if (this.txtFirstName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Името не може да е по-дълго от {0} символа",
                    MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtMiddleName.Text) 
                || string.IsNullOrWhiteSpace(this.txtMiddleName.Text))
            {
                throw new Exception("Не сте въвели презиме");
            }

            if (!Regex.IsMatch(this.txtMiddleName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Невалидно презиме");
            }

            if (this.txtMiddleName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Презимето не може да е по-дълго от {0} символа", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtLastName.Text) 
                || string.IsNullOrWhiteSpace(this.txtLastName.Text))
            {
                throw new Exception("Не сте въвели фамилия");
            }

            if (!Regex.IsMatch(this.txtLastName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Невалидна фамилия");
            }

            if (this.txtLastName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Фамилията не може да е по-дълга от {0} символа", MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtPassword.Password) 
                || string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                throw new Exception("Не сте въвели парола");
            }

            if (this.txtPassword.Password.Length < MIN_PASS_LENGTH 
                || this.txtPassword.Password.Length > MAX_PASS_LENGTH)
            {
                throw new Exception(string.Format(
                    "Паролата трябва да е между  {0} и {1} символа",
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
                MessageBox.Show(ex.Message, "Грешно въведена информация");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
