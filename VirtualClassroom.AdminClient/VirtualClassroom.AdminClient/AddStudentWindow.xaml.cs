using System;
using System.Text.RegularExpressions;
using System.Windows;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for AddStudentWindow.xaml
    /// </summary>
    public partial class AddStudentWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();
        public const int MAX_NAME_LENGTH = 32;
        private const int MIN_PASS_LENGTH = 4;
        private const int MAX_PASS_LENGTH = 32;

        public AddStudentWindow()
        {
            InitializeComponent();

            this.comboClasses.Items.Clear();
            this.comboClasses.ItemsSource = client.GetClasses();
        }

        /// <summary>
        /// Checks whether the user input is valid
        /// </summary>
        private void ValidateInput()
        {
            if(string.IsNullOrEmpty(this.txtUsername.Text) 
                || string.IsNullOrWhiteSpace(this.txtUsername.Text))
            {
                throw new Exception("Не сте въвели потребителско име");
            }

            if (!Regex.IsMatch(this.txtUsername.Text, "^[a-zA-Z]+[a-zA-Z0-9_\\.]*$"))
            {
                throw new Exception("Потребителското име не е валидно");
            }

            if(this.txtUsername.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Потребителското име не може да е по-дълго от {0} символа!", 
                    MAX_NAME_LENGTH));
            }

            if(string.IsNullOrEmpty(this.txtFirstName.Text) 
                || string.IsNullOrWhiteSpace(this.txtFirstName.Text))
            {
                throw new Exception("Не сте въвели име");
            }

            if (!Regex.IsMatch(this.txtFirstName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Името не е валидно");
            }

            if(this.txtFirstName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Името не може да бъде по-дълго от {0} символа", 
                    MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtMiddleName.Text) 
                || string.IsNullOrWhiteSpace(this.txtMiddleName.Text))
            {
                throw new Exception("Не сте въвели презима");
            }

            if (!Regex.IsMatch(this.txtMiddleName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Презимето не е валидно");
            }

            if (this.txtMiddleName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Презимето не може да е по-дълго от {0} символа", 
                    MAX_NAME_LENGTH));
            }

            if (string.IsNullOrEmpty(this.txtLastName.Text) 
                || string.IsNullOrWhiteSpace(this.txtLastName.Text))
            {
                throw new Exception("Не сте въвели фамилия");
            }

            if (!Regex.IsMatch(this.txtLastName.Text, "\\A[а-яА-Я]+(-)?[а-яА-Я]+\\Z"))
            {
                throw new Exception("Фамилията не е валидна");
            }

            if (this.txtLastName.Text.Length > MAX_NAME_LENGTH)
            {
                throw new Exception(string.Format(
                    "Фамилията не може да е по-дълга от {0} символа", 
                    MAX_NAME_LENGTH));
            }

            if (!IsEgnValid(this.txtEgn.Text))
            {
                throw new Exception("Невалидно ЕГН");
            }

            if(string.IsNullOrEmpty(this.txtPassword.Password) 
                || string.IsNullOrWhiteSpace(this.txtPassword.Password))
            {
                throw new Exception("Не сте въвели парола");
            }

            if(this.txtPassword.Password.Length < MIN_PASS_LENGTH 
                || this.txtPassword.Password.Length > MAX_PASS_LENGTH)
            {
                throw new Exception(string.Format("Паролата трябва да е между {0} и {1} символа",
                    MIN_PASS_LENGTH, MAX_PASS_LENGTH));
            }

            if(this.comboClasses.SelectedIndex < 0)
            {
                throw new Exception("Не сте избрали клас");
            }
        }

        /// <summary>
        /// Checks the the given EGN for errors
        /// </summary>
        /// <param name="egn">The EGN to test</param>
        /// <returns>Whether the EGN is valid</returns>
        private bool IsEgnValid(string egn)
        {
            if (string.IsNullOrEmpty(egn))
            {
                return false;
            }

            if (egn.Length != 10)
            {
                return false;
            }

            double num;
            if (!double.TryParse(egn, out num))
            {
                return false;
            }

            int[] weights = { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
            string date = string.Empty;
            DateTime dt;
            int year = Int16.Parse(egn.Substring(0, 2));
            int month = Int16.Parse(egn.Substring(2, 2));
            int day = Int16.Parse(egn.Substring(4, 2));

            if (month > 40)
            {
                date = (month - 40) + "-" + day + "-" + (year + 2000);
            }
            else if (month > 20)
            {
                date = (month - 20) + "-" + day + "-" + (year + 1800);
            }
            else
            {
                date = month + "-" + day + "-" + (year + 1900);
            }

            if (DateTime.TryParse(date, out dt) == false)
            {
                return false;
            }

            int checkSum = Int32.Parse(egn.Substring(9, 1));
            int egnSum = 0;
            int validCheckSum = 0;

            for (int i = 0; i < 9; i++)
            {
                egnSum += int.Parse(egn.Substring(i, 1)) * weights[i];
            }

            validCheckSum = egnSum % 11;

            if (validCheckSum == 10)
            {
                validCheckSum = 0;
            }

            if (checkSum == validCheckSum)
            {
                return true;
            }

            return false; 
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
            try
            {
                this.ValidateInput();

                this.Username = this.txtUsername.Text;
                this.FirstName = this.txtFirstName.Text;
                this.MiddleName = this.txtMiddleName.Text;
                this.LastName = this.txtLastName.Text;
                this.Password = this.txtPassword.Password;
                this.EGN = this.txtEgn.Text;
                this.ClassId = (this.comboClasses.SelectedItem as Class).Id;

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
