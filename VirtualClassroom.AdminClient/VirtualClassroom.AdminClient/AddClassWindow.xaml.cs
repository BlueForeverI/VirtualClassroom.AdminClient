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

        private void ValidateInput()
        {
            if(string.IsNullOrEmpty(this.txtNumber.Text) || string.IsNullOrWhiteSpace(this.txtNumber.Text))
            {
                throw new Exception("The number cannot be an empty string!");
            }

            if(!Regex.IsMatch(this.txtNumber.Text, "[0-9]+"))
            {
                throw new Exception("Invalid number value!");
            }

            int number = int.Parse(this.txtNumber.Text);
            if(number < 1 || number > 12)
            {
                throw new Exception("The number should be between 1 and 12!");
            }

            if(string.IsNullOrEmpty(this.txtLetter.Text) || string.IsNullOrWhiteSpace(this.txtLetter.Text))
            {
                throw new Exception("The letter cannot be an empty string!");
            }

            if (!Regex.IsMatch(this.txtLetter.Text, "[A-Za-z]") || this.txtLetter.Text.Length > 1)
            {
                throw new Exception("Invalid letter value!");
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
                MessageBox.Show(ex.Message, "Invalid input");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
