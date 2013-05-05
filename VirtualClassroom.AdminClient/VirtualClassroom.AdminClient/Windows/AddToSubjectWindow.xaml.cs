using System;
using System.Windows;
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
            this.SubjectId = (this.comboSubjects.SelectedItem as SubjectView).Id;
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
