using System;
using System.Collections.Generic;
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

        public ManageSubjectsPage()
        {
            try
            {

                InitializeComponent();

                this.dataGridSubjects.Items.Clear();
                this.dataGridSubjects.ItemsSource = client.GetSubjectViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    MessageBox.Show("Предметът беше добавен успешно");
                    this.dataGridSubjects.ItemsSource = client.GetSubjectViews();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        MessageBox.Show("Предметите бяха премахнати успешно");
                        this.dataGridSubjects.ItemsSource = client.GetSubjectViews();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                MessageBox.Show(ex.Message);
            }
        }
    }
}
