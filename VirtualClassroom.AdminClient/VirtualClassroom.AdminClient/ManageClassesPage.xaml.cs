using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Get all classes from an Access database file
        /// </summary>
        /// <param name="fileName">Access database file</param>
        /// <returns>All classes as a collection of VirtualClassroom classes</returns>
        private Class[] GetClassesFromAccess(string fileName)
        {
            // open connection
            OleDbConnection conn = new OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0; " +
                "Data Source=" + fileName);

            conn.Open();

            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Code Class]", conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            List<Class> classes = new List<Class>();
            while (reader.Read())
            {
                int id = int.Parse(reader["Class ID"].ToString());
                int number = int.Parse(reader["Class No"].ToString());
                int letterId = int.Parse(reader["Paralell class"].ToString());

                string letter = "";
                if (letterId == 0 || letterId == 1)
                {
                    letter = "А";
                }
                else if (letterId == 2)
                {
                    letter = "Б";
                }
                else if (letterId == 3)
                {
                    letter = "В";
                }
                else if (letterId == 4)
                {
                    letter = "Г";
                }
                else if (letterId == 5)
                {
                    letter = "Д";
                }

                if (letter != "" && number >= 1 && number <= 12)
                {
                    classes.Add(new Class()
                    {
                        Number = number,
                        Letter = letter
                    });
                }
            }

            reader.Close();
            conn.Close();
            return classes.ToArray();
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
                        var classes = GetClassesFromAccess(dialog.FileName);

                        client.AddClasses(classes);
                        MessageBox.Show("Класовете бяха импортирани успешно");
                        this.dataGridClasses.ItemsSource = client.GetClasses();

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
