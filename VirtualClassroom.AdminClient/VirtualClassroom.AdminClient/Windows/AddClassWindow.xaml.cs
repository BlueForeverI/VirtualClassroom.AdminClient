using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for AddClass.xaml
    /// </summary>
    public partial class AddClassWindow : Window
    {
        public AddClassWindow()
        {
            InitializeComponent();
        }

        public string Letter { get;private set; }
        public int Number { get; private set; }

        /// <summary>
        /// Checks whether the user input is valid
        /// </summary>
        private void ValidateInput()
        {
            if(string.IsNullOrEmpty(this.txtNumber.Text) || string.IsNullOrWhiteSpace(this.txtNumber.Text))
            {
                throw new Exception("Не сте въвели клас");
            }

            if(!Regex.IsMatch(this.txtNumber.Text, "[0-9]+"))
            {
                throw new Exception("Невалидна стойност за клас");
            }

            int number = int.Parse(this.txtNumber.Text);
            if(number < 1 || number > 12)
            {
                throw new Exception("Класът трябва да е между 1 и 12");
            }

            if(string.IsNullOrEmpty(this.txtLetter.Text) || string.IsNullOrWhiteSpace(this.txtLetter.Text))
            {
                throw new Exception("Не сте въвели паралелка");
            }

            if (!Regex.IsMatch(this.txtLetter.Text, "[а-яА-Я]") || this.txtLetter.Text.Length > 1)
            {
                throw new Exception("Невалидна стойност за паралелка");
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ValidateInput();
                this.Letter = this.txtLetter.Text;
                this.Number = int.Parse(this.txtNumber.Text);
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
