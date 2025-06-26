using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class SubjectListViewModel : INotifyPropertyChanged
    {
        private readonly ISubjectRepository _repository;
        public ObservableCollection<Subject> Subjects { get; } = new();
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set { _selectedSubject = value; OnPropertyChanged(nameof(SelectedSubject)); }
        }

        public SubjectListViewModel(ISubjectRepository repository)
        {
            _repository = repository;
            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedSubject != null);
            DeleteCommand = new RelayCommand(async _ => await Delete(), _ => SelectedSubject != null);
            Load();
        }

        private async void Load()
        {
            Subjects.Clear();
            var list = await _repository.GetAllAsync();
            foreach (var subject in list)
                Subjects.Add(subject);
        }

        private void Add()
        {
            var vm = new SubjectEditViewModel(new Subject(), _repository);
            var win = new SubjectEditWindow(vm);
            if (win.ShowDialog() == true)
                Load();
        }

        private void Edit()
        {
            if (SelectedSubject == null) return;
            var copy = new Subject
            {
                Id = SelectedSubject.Id,
                Name = SelectedSubject.Name,
                CourseNumber = SelectedSubject.CourseNumber
            };
            var vm = new SubjectEditViewModel(copy, _repository);
            var win = new SubjectEditWindow(vm);
            if (win.ShowDialog() == true)
                Load();
        }

        private async Task Delete()
        {
            if (SelectedSubject == null) return;
            if (MessageBox.Show("Удалить предмет?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _repository.DeleteAsync(SelectedSubject.Id);
                Load();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
