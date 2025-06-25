using CuratorApp.Models;
using CuratorApp.Repositories;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class AnnualRecordEditViewModel : INotifyPropertyChanged
    {
        private readonly IAnnualRecordRepository _repo;
        private readonly int _groupId;

        public AnnualRecord Record { get; set; }

        public ObservableCollection<Student> Students { get; set; } = new();
        public ObservableCollection<Subject> Subjects { get; set; } = new();

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent != value)
                {
                    _selectedStudent = value;
                    Record.StudentId = value?.Id ?? 0;
                    OnPropertyChanged(nameof(SelectedStudent));
                }
            }
        }

        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject != value)
                {
                    _selectedSubject = value;
                    Record.SubjectId = value?.Id ?? 0;
                    OnPropertyChanged(nameof(SelectedSubject));
                }
            }
        }

        public int CourseNumber
        {
            get => Record.CourseNumber;
            set
            {
                if (Record.CourseNumber != value)
                {
                    Record.CourseNumber = value;
                    OnPropertyChanged(nameof(CourseNumber));
                }
            }
        }

        public int? FinalGrade
        {
            get => Record.FinalGrade;
            set
            {
                if (Record.FinalGrade != value)
                {
                    Record.FinalGrade = value;
                    OnPropertyChanged(nameof(FinalGrade));
                }
            }
        }

        public int AbsenceCount
        {
            get => Record.AbsenceCount;
            set
            {
                if (Record.AbsenceCount != value)
                {
                    Record.AbsenceCount = value;
                    OnPropertyChanged(nameof(AbsenceCount));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public AnnualRecordEditViewModel(AnnualRecord record, IAnnualRecordRepository repo, int groupId)
        {
            Record = record;
            _repo = repo;
            _groupId = groupId;

            LoadData();

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
        }

        private async void LoadData()
        {
            var students = await _repo.GetStudentsByGroupIdAsync(_groupId);
            Students.Clear();
            foreach (var s in students)
                Students.Add(s);

            var subjects = await _repo.GetAllSubjectsAsync();
            Subjects.Clear();
            foreach (var s in subjects)
                Subjects.Add(s);

            SelectedStudent = Students.FirstOrDefault(s => s.Id == Record.StudentId);
            SelectedSubject = Subjects.FirstOrDefault(s => s.Id == Record.SubjectId);
        }

        private async Task SaveAsync()
        {
            if (!Validate())
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            if (Record.Id == 0)
                await _repo.AddAsync(Record);
            else
                await _repo.UpdateAsync(Record);

            CloseRequested?.Invoke(this, true);
        }

        private bool Validate()
        {
            if (SelectedStudent == null)
                return false;
            if (SelectedSubject == null)
                return false;
            if (CourseNumber <= 0)
                return false;
            if (AbsenceCount < 0)
                return false;
            return true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event EventHandler<bool>? CloseRequested;
    }
}
