using CuratorApp.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace CuratorApp.Views
{
    public partial class StudentAddWindow : Window
    {
        public Student Student { get; set; }

        public DateTime BirthdayDate
        {
            get => Student.Birthday.ToDateTime(TimeOnly.MinValue);
            set => Student.Birthday = DateOnly.FromDateTime(value);
        }

        public StudentAddWindow(int groupId)
        {
            InitializeComponent();

            Student = new Student
            {
                GroupId = groupId,
                Birthday = DateOnly.FromDateTime(DateTime.Today),
                EnrollmentYear = DateTime.Now.Year
            };

            DataContext = this;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Student.FirstName))
                errors.Add("Имя обязательно.");

            if (string.IsNullOrWhiteSpace(Student.LastName))
                errors.Add("Фамилия обязательна.");

            if (string.IsNullOrWhiteSpace(Student.Phone))
                errors.Add("Телефон обязателен.");
            else if (!Regex.IsMatch(Student.Phone, @"^\+?\d{7,15}$"))
                errors.Add("Телефон должен содержать только цифры и может начинаться с '+'. Пример: +71234567890");

            if (Student.EnrollmentYear < 1900 || Student.EnrollmentYear > DateTime.Now.Year)
                errors.Add($"Год поступления должен быть между 1900 и {DateTime.Now.Year}.");

            if (Student.Birthday.ToDateTime(TimeOnly.MinValue) > DateTime.Today)
                errors.Add("Дата рождения не может быть в будущем.");

            if (errors.Any())
            {
                MessageBox.Show(string.Join("\n", errors), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Close();
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
