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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VirtualClassroom.AdminClient;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LoginWindow loginWindow = new LoginWindow();

            if(loginWindow.ShowDialog() == true)
            {
                InitializeComponent();
            }
            else
            {
                this.Close();
            }
        }

        private void btnManageClasses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.frameMainContent.Source = new Uri("ManageClassesPage.xaml", UriKind.Relative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnManageTeachers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.frameMainContent.Source = new Uri("ManageTeachersPage.xaml", UriKind.Relative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnManageStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.frameMainContent.Source = new Uri("ManageStudentsPage.xaml", UriKind.Relative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnManageSubjects_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.frameMainContent.Source = new Uri("ManageSubjectsPage.xaml", UriKind.Relative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                ClientManager.CloseClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
