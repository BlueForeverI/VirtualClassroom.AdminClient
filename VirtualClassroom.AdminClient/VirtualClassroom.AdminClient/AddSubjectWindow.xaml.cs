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
    /// Interaction logic for AddSubjectWindow.xaml
    /// </summary>
    public partial class AddSubjectWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();
        private const int MAX_SUBJECT_LENGTH = 60;

        public AddSubjectWindow()
        {
            InitializeComponent();

            this.comboTeachers.Items.Clear();
            this.comboTeachers.ItemsSource = client.GetTeachers();
        }

        private void ValidateInput()
        {
            if(string.IsNullOrEmpty(this.txtName.Text) || string.IsNullOrWhiteSpace(this.txtName.Text))
            {
                throw new Exception("The subject name cannot be an empty string!");
            }

            if(this.txtName.Text.Length > MAX_SUBJECT_LENGTH)
            {
                throw new Exception(string.Format("The subject name cannot be more than {0} characters", MAX_SUBJECT_LENGTH));
            }
        }

        public string SubjectName { get; private set; }
        public int TeacherId { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateInput();

                this.SubjectName = this.txtName.Text;
                this.TeacherId = (this.comboTeachers.SelectedItem as Teacher).Id;
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
