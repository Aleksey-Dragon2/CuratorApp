using CuratorApp.Models;
using CuratorApp.Repositories;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace CuratorApp.Views
{
    public partial class StudentEditWindow : Window, INotifyPropertyChanged
    {
        public Student Student { get; set; }

        private string _groupName = "";
        public string GroupName
        {
            get => _groupName;
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime BirthdayDate
        {
            get => Student.Birthday.ToDateTime(TimeOnly.MinValue);
            set
            {
                if (Student.Birthday != DateOnly.FromDateTime(value))
                {
                    Student.Birthday = DateOnly.FromDateTime(value);
                    OnPropertyChanged();
                }
            }
        }

        private readonly IStudentRepository _studentRepo;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public StudentEditWindow(Student student, IStudentRepository studentRepo, IGroupRepository groupRepo)
        {
            InitializeComponent();

            Student = new Student
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                MiddleName = student.MiddleName,
                Phone = student.Phone,
                Address = student.Address,
                Birthday = student.Birthday,
                EnrollmentYear = student.EnrollmentYear,
                GroupId = student.GroupId
            };

            _studentRepo = studentRepo;

            DataContext = this;
            _ = LoadGroupNameAsync(groupRepo, student.GroupId);
        }

        private async Task LoadGroupNameAsync(IGroupRepository groupRepo, int groupId)
        {
            var group = await groupRepo.GetByIdAsync(groupId);
            GroupName = group?.Name ?? "Неизвестно";
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _studentRepo.UpdateAsync(Student);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
