﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Application.Current.Resources["defaultErrorMessage"].ToString(), 
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                            var students = AccessDatabaseHelper.GetStudentsFromAccess(
                                dialog.FileName, secret, client.GetClasses());

                            client.RegisterStudents(students.ToArray(), secret);
                            Dispatcher.BeginInvoke(
                                new Action(() =>
                                    this.dataGridStudents.ItemsSource = 
                                        client.GetStudentViews()));
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
