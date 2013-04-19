using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    /// <summary>
    /// Interaction logic for ManageStudentsPage.xaml
    /// </summary>
    public partial class ManageStudentsPage : Page
    {
        private AdminServiceClient client = ClientManager.GetClient();

        public ManageStudentsPage()
        {
            try
            {
                InitializeComponent();

                this.dataGridStudents.Items.Clear();
                this.dataGridStudents.ItemsSource = client.GetStudentViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAddStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AddStudentWindow addStudentWindow = new AddStudentWindow();
                if (addStudentWindow.ShowDialog() == true)
                {
                    Student student = new Student();
                    student.Username = addStudentWindow.Username;
                    student.FirstName = addStudentWindow.FirstName;
                    student.MiddleName = addStudentWindow.MiddleName;
                    student.LastName = addStudentWindow.LastName;
                    student.EGN = addStudentWindow.EGN;
                    student.ClassId = addStudentWindow.ClassId;

                    string secret = Crypto.GenerateRandomSecret(30);
                    student.Username = Crypto.EncryptStringAES(student.Username, secret);

                    client.RegisterStudent(student,
                        Crypto.EncryptStringAES(addStudentWindow.Password, secret), secret);

                    MessageBox.Show("Ученикът беше добавен успешно");
                    this.dataGridStudents.ItemsSource = client.GetStudentViews();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRemoveStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var students = new List<Student>();
                foreach (var selected in this.dataGridStudents.SelectedItems)
                {
                    dynamic item = selected;
                    int id = int.Parse(item.Id.ToString());
                    students.Add(new Student() { Id = id });
                }

                if(students.Count == 0)
                {
                    MessageBox.Show("Не сте избрали ученици");
                }
                else
                {
                    if (MessageBox.Show("Наистина ли искате да премахнете тези ученици?",
                        "Сигурен ли сте?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        client.RemoveStudents(students.ToArray());
                        MessageBox.Show("Учениците бяха премахнати успешно");
                        this.dataGridStudents.ItemsSource = client.GetStudentViews();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEditStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dataGridStudents.SelectedIndex == -1)
                {
                    MessageBox.Show("Не сте избрали ученик");
                }
                else if (this.dataGridStudents.SelectedItems.Count > 1)
                {
                    MessageBox.Show("Трябва да изберете точно един ученик");
                }
                else
                {
                    int studentId = (this.dataGridStudents.SelectedItem as dynamic).Id;

                    EditStudentWindow editStudentWindow = new EditStudentWindow(
                        client.GetStudentById(studentId));
                    if (editStudentWindow.ShowDialog() == true)
                    {
                        var student = editStudentWindow.Student;
                        string secret = Crypto.GenerateRandomSecret(40);
                        student.Username = Crypto.EncryptStringAES(student.Username, secret);
                        student.PasswordHash = Crypto.EncryptStringAES(student.PasswordHash, secret);

                        client.EditStudent(studentId, student, secret);

                        MessageBox.Show("Ученикът беше редактиран успешно");
                        this.dataGridStudents.ItemsSource = client.GetStudentViews();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Get a class ID based on its number and letter
        /// </summary>
        /// <param name="classes">Collection to search</param>
        /// <param name="number">Class number</param>
        /// <param name="letter">Class letter</param>
        /// <returns>Class ID, ot -1 if the class is not found</returns>
        private int GetClassId(Class[] classes, int number, string letter)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i].Letter.ToLower() == letter.ToLower() &&
                    classes[i].Number == number)
                {
                    return classes[i].Id;
                }
            }

            return -1;
        }

        /// <summary>
        /// Create a map between classes in Access file and 
        /// VirtualClassroom classes, based on number and letter
        /// </summary>
        /// <param name="conn">Access conenction to use</param>
        /// <returns>
        /// Dictionary with Access class id as a key and VirtualClassroom
        /// Class as a value
        /// </returns>
        private Dictionary<int, Class> GetClassMap(OleDbConnection conn)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Code Class]", conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            Dictionary<int, Class> result = new Dictionary<int, Class>();
            var classes = client.GetClasses();

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
                    int classId = GetClassId(classes, number, letter);

                    if (classId != -1)
                    {
                        result.Add(id, new Class() { Id = classId });
                    }
                }
            }

            reader.Close();
            return result;
        }

        /// <summary>
        /// Get all students from an Access database
        /// </summary>
        /// <param name="fileName">Access database file</param>
        /// <param name="secret">Key to encrypt usernames and passwords</param>
        /// <returns>All students from the database as a collection of VitualClassroom students</returns>
        private Student[] GetStudentsFromAccess(string fileName, string secret)
        {
            // open connection
            OleDbConnection conn = new OleDbConnection(
                "Provider=Microsoft.Jet.OLEDB.4.0; " +
                "Data Source=" + fileName);

            conn.Open();

            OleDbCommand cmd = new OleDbCommand("SELECT s.[Name 1], s.[Name 2], " +
                "s.[Name 3], s.[ID Number], sc.Class FROM Students s" +
                " INNER JOIN StudentClass sc ON s.[ID Number] = sc.[ID Number]", conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            // map the Access classes and VirtualClassroom classes
            var map = GetClassMap(conn);
            List<Student> students = new List<Student>();

            while (reader.Read())
            {
                string firstName = reader["Name 1"].ToString();
                string middleName = reader["Name 2"].ToString();
                string lastName = reader["Name 3"].ToString();
                string egn = reader["ID number"].ToString();
                egn = egn.PadLeft(10, '0');
                string username = string.Format("{0}.{1}.{2}",
                                                firstName.ToLower(), lastName.ToLower(), egn.Substring(7, 2));
                int classId = int.Parse(reader["Class"].ToString());

                if (map.ContainsKey(classId))
                {
                    Student student = new Student()
                    {
                        ClassId = map[classId].Id,
                        EGN = egn,
                        FirstName = firstName,
                        MiddleName = middleName,
                        LastName = lastName,
                        PasswordHash = Crypto.EncryptStringAES(username, secret),
                        Username = Crypto.EncryptStringAES(username, secret)
                    };

                    students.Add(student);

                    // for test purposes only - REMOVE IN PRODUCTION
                    if (students.Count >= 20)
                    {
                        break;
                    }
                }
            }

            reader.Close();
            conn.Close();

            return students.ToArray();
        }

        private void btnImportStudents_Click(object sender, RoutedEventArgs e)
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
                            var students = GetStudentsFromAccess(dialog.FileName, secret);

                            client.RegisterStudents(students.ToArray(), secret);
                            Dispatcher.BeginInvoke(
                                new Action(() =>
                                    this.dataGridStudents.ItemsSource = client.GetStudentViews()));
                        };

                        worker.RunWorkerCompleted += (o, ea) =>
                        {
                            this.busyIndicator.IsBusy = false;
                            MessageBox.Show("Учениците бяха импортирани успешно");
                        };

                        this.busyIndicator.IsBusy = true;
                        worker.RunWorkerAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Грешка при импортирането. Учениците не бяха добавени");
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
