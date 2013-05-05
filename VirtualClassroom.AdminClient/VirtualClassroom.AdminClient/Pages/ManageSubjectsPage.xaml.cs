using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageSubjectsPage.xaml
    /// </summary>
    public partial class ManageSubjectsPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        private void UpdateSubjectViews()
        {
            Thread thread = new Thread(() => Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    var subjects = client.GetSubjectViews();
                    this.dataGridSubjects.ItemsSource = subjects;
                })));
            thread.Start();
        }

        public ManageSubjectsPage()
        {
            try
            {

                InitializeComponent();
                UpdateSubjectViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddSubject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddSubjectWindow addSubjectWindow = new AddSubjectWindow();
                if (addSubjectWindow.ShowDialog() == true)
                {
                    Subject subject = new Subject();
                    subject.Name = addSubjectWindow.SubjectName;
                    subject.TeacherId = addSubjectWindow.TeacherId;

                    client.AddSubject(subject);
                    UpdateSubjectViews();
                    MessageBox.Show("Предметът беше добавен успешно");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRemoveSubject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var subjects = new List<Subject>();
                foreach (dynamic selected in this.dataGridSubjects.SelectedItems)
                {
                    int id = int.Parse(selected.Id.ToString());
                    subjects.Add(new Subject() { Id = id });
                }

                if (subjects.Count == 0)
                {
                    MessageBox.Show("Не сте избрали предмети");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези предмети?", "Сигурен ли сте?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveSubjects(subjects.ToArray());
                        UpdateSubjectViews();
                        MessageBox.Show("Предметите бяха премахнати успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddToClass_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Subject> subjects = new List<Subject>();

                foreach (dynamic selected in this.dataGridSubjects.SelectedItems)
                {
                    int id = int.Parse(selected.Id.ToString());
                    subjects.Add(new Subject() { Id = id });
                }

                if (subjects.Count == 0)
                {
                    MessageBox.Show("Не сте избрали предмети");
                }
                else
                {
                    AddToClassWindow addToClassWindow = new AddToClassWindow();
                    if (addToClassWindow.ShowDialog() == true)
                    {
                        Class c = new Class() {Id = addToClassWindow.ClassId};
                        client.AddSubjectsToClass(c, subjects.ToArray());
                        MessageBox.Show("Предметите бяха добавени към класа успешно");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(),
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
