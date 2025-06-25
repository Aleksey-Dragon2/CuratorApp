using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModels
{
    public class StudentListViewModel : INotifyPropertyChanged
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IGroupRepository _groupRepo;

        private readonly int _groupId;

        public ObservableCollection<Student> Students { get; set; } = new();

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set => SetField(ref _selectedStudent, value);
        }

        private string _upcomingBirthdayInfo = "";
        public string UpcomingBirthdayInfo
        {
            get => _upcomingBirthdayInfo;
            set => SetField(ref _upcomingBirthdayInfo, value);
        }


        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand OpenDetailCommand { get; }


        public StudentListViewModel(IStudentRepository studentRepo, IGroupRepository groupRepo, int groupId)
        {
            _studentRepo = studentRepo;
            _groupRepo = groupRepo;
            _groupId = groupId;

            AddCommand = new RelayCommand(_ => AddStudent());
            EditCommand = new RelayCommand(
                _ => EditStudent(SelectedStudent!),
                _ => SelectedStudent != null
            );
            DeleteCommand = new RelayCommand(async _ => await DeleteStudentAsync(), _ => SelectedStudent != null);
            RefreshCommand = new RelayCommand(async _ => await LoadStudentsAsync());
            OpenDetailCommand = new RelayCommand(_ => OpenDetail(), _ => SelectedStudent != null);


            _ = LoadStudentsAsync();
        }

        private async Task LoadStudentsAsync()
        {
            Students.Clear();
            var list = await _studentRepo.GetByGroupIdAsync(_groupId);
            foreach (var s in list)
                Students.Add(s);

            SetUpcomingBirthdayInfo();
        }

        private void SetUpcomingBirthdayInfo()
        {
            if (Students.Count == 0)
            {
                UpcomingBirthdayInfo = "Нет студентов.";
                return;
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            var next = Students
                .Select(s =>
                {
                    var nextBirthday = new DateOnly(today.Year, s.Birthday.Month, s.Birthday.Day);
                    if (nextBirthday < today)
                        nextBirthday = nextBirthday.AddYears(1);

                    return new { Student = s, DaysLeft = (nextBirthday.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days };
                })
                .OrderBy(x => x.DaysLeft)
                .FirstOrDefault();

            if (next != null)
            {
                var s = next.Student;
                UpcomingBirthdayInfo = $"Ближайший день рождения: {s.FirstName} {s.LastName} — через {next.DaysLeft} дн.";
            }
            else
            {
                UpcomingBirthdayInfo = "Нет предстоящих дней рождения.";
            }
        }

        private async void AddStudent()
        {
            var addWindow = new StudentAddWindow(_groupId);
            var result = addWindow.ShowDialog();
            if (result == true)
            {
                var newStudent = addWindow.Student;
                try
                {
                    await _studentRepo.CreateAsync(newStudent);
                    await LoadStudentsAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении: {ex.Message}");
                }
            }
        }
        private void OpenDetail()
        {
            if (SelectedStudent == null)
                return;

            var window = new StudentDetailWindow(SelectedStudent, _groupId);
            window.ShowDialog();
        }



        private async void EditStudent(Student student)
        {
            var dialog = new StudentEditWindow(student, _studentRepo, _groupRepo);
            if (dialog.ShowDialog() == true)
            {
                var updated = dialog.Student;
                await _studentRepo.UpdateAsync(updated);

                // Обновляем локальную коллекцию
                var existing = Students.FirstOrDefault(s => s.Id == updated.Id);
                if (existing != null)
                {
                    existing.FirstName = updated.FirstName;
                    existing.LastName = updated.LastName;
                    existing.MiddleName = updated.MiddleName;
                    existing.Birthday = updated.Birthday;
                    existing.Phone = updated.Phone;
                    existing.Address = updated.Address;
                    existing.EnrollmentYear = updated.EnrollmentYear;

                    OnPropertyChanged(nameof(Students)); // Обновление привязки
                }
            }
            await LoadStudentsAsync();
        }

        private async Task DeleteStudentAsync()
        {
            if (SelectedStudent is null) return;

            if (MessageBox.Show("Удалить студента?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _studentRepo.DeleteAsync(SelectedStudent.Id);
                Students.Remove(SelectedStudent);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propName!);
            return true;
        }
    }
}
