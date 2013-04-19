using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageTeachersPage.xaml
    /// </summary>
    public partial class ManageTeachersPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageTeachersPage()
        {
            try
            {
                InitializeComponent();
                this.dataGridTeachers.Items.Clear();
                this.dataGridTeachers.ItemsSource = client.GetTeachers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddTeacherWindow addTeacherWindow = new AddTeacherWindow();
                if (addTeacherWindow.ShowDialog() == true)
                {
                    string username = addTeacherWindow.Username;
                    string firstName = addTeacherWindow.FirstName;
                    string middleName = addTeacherWindow.MiddleName;
                    string lastName = addTeacherWindow.LastName;
                    string password = addTeacherWindow.Password;

                    Teacher teacher = new Teacher()
                    {
                        Username = username,
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName
                    };

                    string secret = Crypto.GenerateRandomSecret(30);
                    teacher.Username = Crypto.EncryptStringAES(teacher.Username, secret);

                    client.RegisterTeacher(teacher,
                        Crypto.EncryptStringAES(addTeacherWindow.Password, secret),
                        secret);

                    MessageBox.Show("Учителят беше добавен успешно");
                    this.dataGridTeachers.ItemsSource = client.GetTeachers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var teachers = new List<Teacher>();
                foreach (var selected in this.dataGridTeachers.SelectedItems)
                {
                    teachers.Add(new Teacher() { Id = (selected as Teacher).Id });
                }

                if (teachers.Count == 0)
                {
                    MessageBox.Show("Не сте избрали учители");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези учители?", "Сигурен ли сте?",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveTeachers(teachers.ToArray());
                        MessageBox.Show("Учителите бяха премахнати успешно");
                        this.dataGridTeachers.ItemsSource = client.GetTeachers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditTeacher_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.dataGridTeachers.SelectedIndex == -1)
                {
                    MessageBox.Show("Не сте избрали учител");
                }
                else if(this.dataGridTeachers.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Трябва да изберете един учител");
                }
                else
                {
                    int teacherId = (this.dataGridTeachers.SelectedItem as dynamic).Id;

                    EditTeacherWindow editTeacherWindow = new EditTeacherWindow(
                        client.GetTeacherById(teacherId));

                    if(editTeacherWindow.ShowDialog() == true)
                    {
                        var teacher = editTeacherWindow.Teacher;
                        string secret = Crypto.GenerateRandomSecret(40);
                        teacher.Username = Crypto.EncryptStringAES(teacher.Username, secret);
                        teacher.PasswordHash = Crypto.EncryptStringAES(teacher.PasswordHash, secret);

                        client.EditTeacher(teacherId, teacher, secret);

                        MessageBox.Show("Учителят беше редактиран успешно");
                        this.dataGridTeachers.ItemsSource = client.GetTeachers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Get all teachers from an Access database
        /// </summary>
        /// <param name="fileName">Access database file</param>
        /// <param name="secret">Key to encrypt usernames and passwords</param>
        /// <returns>All teachers from the database as a collection of VitualClassroom students</returns>
        private Teacher[] GetTeachersFromAccess(string fileName, string secret)
        {
            // open connection
            OleDbConnection conn = new OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0; " +
                "Data Source=" + fileName);

            conn.Open();

            OleDbCommand cmd = new OleDbCommand("SELECT [First name], [Second name], [Family name], [ID Number] " +
                "FROM [Teachers - personal status]", conn);

            OleDbDataReader reader = cmd.ExecuteReader();

            List<Teacher> teachers = new List<Teacher>();
            while (reader.Read())
            {
                string firstName = reader["First name"].ToString();
                string middleName = reader["Second name"].ToString();
                string lastName = reader["Family name"].ToString();
                string egn = reader["ID Number"].ToString();
                egn = egn.PadLeft(10, '0');
                string username = string.Format("{0}.{1}.{2}",
                                                firstName, lastName, egn.Substring(7, 2));

                Teacher teacher = new Teacher()
                {
                    FirstName = firstName,
                    MiddleName = middleName,
                    LastName = lastName,
                    Username = Crypto.EncryptStringAES(username, secret),
                    PasswordHash = Crypto.EncryptStringAES(username, secret)
                };

                teachers.Add(teacher);
            }

            reader.Close();
            conn.Close();

            return teachers.ToArray();
        }

        private void btnImportTeachers_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "MS Access files (*.mdb)|*.mdb";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName.EndsWith(".mdb"))
                {
                    try
                    {
                        BackgroundWorker worker = new BackgroundWorker();

                        worker.DoWork += (o, ea) =>
                        {
                            string secret = Crypto.GenerateRandomSecret(40);
                            var teachers = GetTeachersFromAccess(dialog.FileName, secret);

                            client.RegisterTeachers(teachers, secret);
                            Dispatcher.BeginInvoke(
                                new Action(() =>
                                    this.dataGridTeachers.ItemsSource = client.GetTeachers()));
                        };

                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            this.busyIndicator.IsBusy = false;
                            MessageBox.Show("Учителите бяха импортирани успешно");
                        };

                        this.busyIndicator.IsBusy = true;
                        worker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Грешка при импортирането. Учителите не бяха добавени");
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
