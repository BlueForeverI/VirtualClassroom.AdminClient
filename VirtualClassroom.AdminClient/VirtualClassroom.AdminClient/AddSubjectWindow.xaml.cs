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

        public AddSubjectWindow()
        {
            InitializeComponent();

            this.comboTeachers.Items.Clear();
            this.comboTeachers.ItemsSource = client.GetTeachers();
        }

        public string Name { get; private set; }
        public int TeacherId { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.Name = this.txtName.Text;
            this.TeacherId = (this.comboTeachers.SelectedItem as Teacher).Id;
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
