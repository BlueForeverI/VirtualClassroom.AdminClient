using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageClasses.xaml
    /// </summary>
    public partial class ManageClassesPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageClassesPage()
        {
            try
            {
                InitializeComponent();
                this.dataGridClasses.Items.Clear();
                this.dataGridClasses.ItemsSource = client.GetClasses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddClassWindow addClassWindow = new AddClassWindow();
                if (addClassWindow.ShowDialog() == true)
                {
                    string letter = addClassWindow.Letter;
                    int number = addClassWindow.Number;
                    client.AddClass(new Class() { Letter = letter, Number = number });
                    MessageBox.Show("Класът беше добавен успешно");
                    this.dataGridClasses.ItemsSource = client.GetClasses();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Class> classes = new List<Class>();
                foreach (var selected in this.dataGridClasses.SelectedItems)
                {
                    classes.Add(new Class() { Id = (selected as Class).Id });
                }

                if (classes.Count == 0)
                {
                    MessageBox.Show("Не сте избрали класове");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези класове?", "Сигурен ли сте?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveClasses(classes.ToArray());
                        MessageBox.Show("Класовете бяха премахнати успешно!");
                        this.dataGridClasses.ItemsSource = client.GetClasses();
                    }
                }   
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddToSubject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Class> classes = new List<Class>();
                foreach (var selected in this.dataGridClasses.SelectedItems)
                {
                    classes.Add(new Class() { Id = (selected as Class).Id });
                }

                if (classes.Count == 0)
                {
                    MessageBox.Show("Не сте избрали класове");
                }
                else
                {
                    AddToSubjectWindow addToSubjectWindow = new AddToSubjectWindow();
                    if (addToSubjectWindow.ShowDialog() == true)
                    {
                        Subject subject = new Subject() {Id = addToSubjectWindow.SubjectId};
                        client.AddClassesToSubject(subject, classes.ToArray());
                        MessageBox.Show("Класовете бяха добавени към предмета успешно");
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
