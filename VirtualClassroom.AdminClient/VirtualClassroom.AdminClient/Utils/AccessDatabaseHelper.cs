using System;
using System.Collections.Generic;
using System.Data.OleDb;
using VirtualClassroom.AdminClient.AdminService;

namespace VirtualClassroom.AdminClient
{
    public static class AccessDatabaseHelper
    {
        /// <summary>
        /// Get all classes from an Access database file
        /// </summary>
        /// <param name="fileName">Access database file</param>
        /// <returns>All classes as a collection of VirtualClassroom classes</returns>
        public static Class[] GetClassesFromAccess(string fileName)
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

        /// <summary>
        /// Get all teachers from an Access database
        /// </summary>
        /// <param name="fileName">Access database file</param>
        /// <param name="secret">Key to encrypt usernames and passwords</param>
        /// <returns>All teachers from the database as a collection of VitualClassroom students</returns>
        public static Teacher[] GetTeachersFromAccess(string fileName, string secret)
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
                username = username.ConvertCyrillicToLatinLetters();

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

        /// <summary>
        /// Get a class ID based on its number and letter
        /// </summary>
        /// <param name="classes">Collection to search</param>
        /// <param name="number">Class number</param>
        /// <param name="letter">Class letter</param>
        /// <returns>Class ID, ot -1 if the class is not found</returns>
        private static int GetClassId(Class[] classes, int number, string letter)
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
        private static Dictionary<int, Class> GetClassMap(OleDbConnection conn, Class[] classes)
        {
            OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Code Class]", conn);
            OleDbDataReader reader = cmd.ExecuteReader();

            Dictionary<int, Class> result = new Dictionary<int, Class>();

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
        public static Student[] GetStudentsFromAccess(string fileName, string secret, Class[] classes)
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
            var map = GetClassMap(conn, classes);
            List<Student> students = new List<Student>();

            while (reader.Read())
            {
                string firstName = reader["Name 1"].ToString();
                string middleName = reader["Name 2"].ToString();
                string lastName = reader["Name 3"].ToString();
                string egn = reader["ID number"].ToString();
                egn = egn.PadLeft(10, '0');
                string username = string.Format("{0}.{1}.{2}",
                                                firstName.ToLower(), 
                                                lastName.ToLower(), 
                                                egn.Substring(7, 2));

                username = username.ConvertCyrillicToLatinLetters();
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
    }
}
