using System;
using System.Windows;
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

        /// <summary>
        /// Checks whether the user input is valid
        /// </summary>
        private void ValidateInput()
        {
            if(string.IsNullOrEmpty(this.txtName.Text) 
                || string.IsNullOrWhiteSpace(this.txtName.Text))
            {
                throw new Exception("Не сте въвели име на предмета");
            }

            if(this.txtName.Text.Length > MAX_SUBJECT_LENGTH)
            {
                throw new Exception(string.Format(
                    "Името не предмета не може да е по-дълго от {0} символа",
                    MAX_SUBJECT_LENGTH));
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
