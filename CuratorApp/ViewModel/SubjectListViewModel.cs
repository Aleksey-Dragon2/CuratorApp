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

        private Subject? _selectedSubject;
        public Subject? SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                if (_selectedSubject != value)
                {
                    _selectedSubject = value;
                    OnPropertyChanged(nameof(SelectedSubject));
                    // Обновляем состояние команд при смене выбранного предмета
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public SubjectListViewModel(ISubjectRepository repository)
        {
            _repository = repository;

            AddCommand = new RelayCommand(_ => Add());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedSubject != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedSubject != null);

            _ = LoadAsync();
        }

        private async Task LoadAsync()
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
            {
                _ = LoadAsync();
            }
        }

        private void Edit()
        {
            if (SelectedSubject == null) return;

            // Копируем, чтобы не редактировать напрямую
            var copy = new Subject
            {
                Id = SelectedSubject.Id,
                Name = SelectedSubject.Name,
                CourseNumber = SelectedSubject.CourseNumber
            };

            var vm = new SubjectEditViewModel(copy, _repository);
            var win = new SubjectEditWindow(vm);
            if (win.ShowDialog() == true)
            {
                _ = LoadAsync();
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedSubject == null) return;

            var res = MessageBox.Show(
                $"Удалить предмет \"{SelectedSubject.Name}\"?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                try
                {
                    await _repository.DeleteAsync(SelectedSubject.Id);
                    await LoadAsync();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
