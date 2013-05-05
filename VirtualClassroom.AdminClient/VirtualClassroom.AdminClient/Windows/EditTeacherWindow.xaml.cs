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
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for EditTeacherWindow.xaml
    /// </summary>
    public partial class EditTeacherWindow : Window
    {
        public const int MAX_NAME_LENGTH = 32;
        private const int MIN_PASS_LENGTH = 4;
        private const int MAX_PASS_LENGTH = 32;

        public EditTeacherWindow(Teacher teacher)
        {
            InitializeComponent();
            this.Teacher = new Teacher();

            this.txtUsername.Text = teacher.Username;
            this.txtFirstName.Text = teacher.FirstName;
            this.txtMiddleName.Text = teacher.MiddleName;
            this.txtLastName.Text = teacher.LastName;
        }

        public Teacher Teacher { get; private set; }

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

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ValidateInput();

                this.Teacher.Username = this.txtUsername.Text;
                this.Teacher.FirstName = this.txtFirstName.Text;
                this.Teacher.MiddleName = this.txtMiddleName.Text;
                this.Teacher.LastName = this.txtLastName.Text;
                this.Teacher.PasswordHash = this.txtPassword.Password;

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
