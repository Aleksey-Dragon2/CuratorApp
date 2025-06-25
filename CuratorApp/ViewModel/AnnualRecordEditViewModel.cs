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
        private readonly ISubjectRepository _subjectRepo;
        private readonly int _groupId;

        public AnnualRecord Record { get; }
        public ObservableCollection<Student> Students { get; } = new();
        public ObservableCollection<Subject> Subjects { get; } = new();

        private Student? _selectedStudent;
        public Student? SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (_selectedStudent == value) return;
                _selectedStudent = value;
                Record.StudentId = value?.Id ?? 0;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject == value) return;
                _selectedSubject = value;
                Record.SubjectId = value?.Id ?? 0;
                OnPropertyChanged(nameof(SelectedSubject));
            }
        }

        public int CourseNumber
        {
            get => Record.CourseNumber;
            set
            {
                if (Record.CourseNumber == value) return;
                Record.CourseNumber = value;
                OnPropertyChanged(nameof(CourseNumber));
            }
        }

        public int? FinalGrade
        {
            get => Record.FinalGrade;
            set
            {
                if (Record.FinalGrade == value) return;
                Record.FinalGrade = value;
                OnPropertyChanged(nameof(FinalGrade));
            }
        }

        public int AbsenceCount
        {
            get => Record.AbsenceCount;
            set
            {
                if (Record.AbsenceCount == value) return;
                Record.AbsenceCount = value;
                OnPropertyChanged(nameof(AbsenceCount));
            }
        }

        public ICommand SaveCommand { get; }

        public AnnualRecordEditViewModel(
            AnnualRecord record,
            IAnnualRecordRepository repo,
            ISubjectRepository subjectRepo,
            int groupId)
        {
            Record = record;
            _repo = repo;
            _subjectRepo = subjectRepo;
            _groupId = groupId;

            LoadData();
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
        }

        private async void LoadData()
        {
            try
            {
                var studentsTask = _repo.GetStudentsByGroupIdAsync(_groupId);
                var subjectsTask = _subjectRepo.GetAllAsync();

                await Task.WhenAll(studentsTask, subjectsTask);

                Students.Clear();
                foreach (var s in await studentsTask)
                    Students.Add(s);

                Subjects.Clear();
                foreach (var s in await subjectsTask)
                    Subjects.Add(s);

                SelectedStudent = Students.FirstOrDefault(s => s.Id == Record.StudentId);
                SelectedSubject = Subjects.FirstOrDefault(s => s.Id == Record.SubjectId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            if (!Validate())
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            try
            {
                if (Record.Id == 0)
                    await _repo.AddAsync(Record);
                else
                    await _repo.UpdateAsync(Record);

                CloseRequested?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private bool Validate()
        {
            return SelectedStudent != null &&
                   SelectedSubject != null &&
                   CourseNumber > 0 &&
                   AbsenceCount >= 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public event EventHandler<bool>? CloseRequested;
    }
}