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
    /// Interaction logic for AddToSubject.xaml
    /// </summary>
    public partial class AddToSubjectWindow : Window
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public AddToSubjectWindow()
        {
            InitializeComponent();

            this.comboSubjects.Items.Clear();
            this.comboSubjects.ItemsSource = client.GetSubjectViews();
        }

        public int SubjectId { get; private set; }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            this.SubjectId = (this.comboSubjects.SelectedItem as Subject).Id;
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
