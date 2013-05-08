using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VirtualClassroom.AdminClient.AdminService;
using System.Data.OleDb;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageClasses.xaml
    /// </summary>
    public partial class ManageClassesPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        private void UpdateClassViews()
        {
            Thread thread = new Thread(() =>Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var classes = client.GetClasses();
                    this.dataGridClasses.ItemsSource = classes;
                })));
            thread.Start();
        }

        public ManageClassesPage()
        {
            try
            {
                InitializeComponent();
                UpdateClassViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    Class c = new Class() {Letter = letter, Number = number};
                    if(client.AddClass(c))
                    {
                        UpdateClassViews();
                        MessageBox.Show("Класът беше добавен успешно");   
                    }
                    else
                    {
                        MessageBox.Show("Класът не е валиден или вече съществува");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                        UpdateClassViews();
                        MessageBox.Show("Класовете бяха премахнати успешно!");
                    }
                }   
            }
            catch(Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnImportClasses_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MS Access files (*.mdb)|*.mdb";
            if(dialog.ShowDialog() == true)
            {
                if(dialog.FileName.EndsWith(".mdb"))
                {
                    try
                    {
                        BackgroundWorker worker = new BackgroundWorker();

                        worker.DoWork += (o, ea) =>
                        {
                            var classes = AccessDatabaseHelper.GetClassesFromAccess(dialog.FileName);
                            client.AddClasses(classes);
                            UpdateClassViews();
                        };

                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            this.busyIndicator.IsBusy = false;
                            MessageBox.Show("Класовете бяха импортирани успешно");

                        };

                        this.busyIndicator.IsBusy = true;
                        worker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Грешка при импортирането. Класовете не бяха добавени");
                    }
                }
                else
                {
                    MessageBox.Show("Програмата поддържа само MS Access файлове (.mdb)");
                }
            }
        }
    }
}
