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
            InitializeComponent();
        }

        private void btnManageClasses_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("ManageClassesPage.xaml", UriKind.Relative);
        }

        private void btnManageTeachers_Click(object sender, RoutedEventArgs e)
        {
            this.frameMainContent.Source = new Uri("ManageTeachersPage.xaml", UriKind.Relative);
        }
    }
}
