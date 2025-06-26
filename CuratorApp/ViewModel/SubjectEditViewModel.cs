using CuratorApp.Models;
using CuratorApp.Repositories;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class SubjectEditViewModel : INotifyPropertyChanged
    {
        private readonly ISubjectRepository _repository;

        private Subject _subject;
        public Subject Subject
        {
            get => _subject;
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }

        public SubjectEditViewModel(Subject subject, ISubjectRepository repository)
        {
            _subject = subject;
            _repository = repository;

            SaveCommand = new RelayCommand(async param =>
            {
                if (string.IsNullOrWhiteSpace(Subject.Name))
                {
                    MessageBox.Show("Название обязательно");
                    return;
                }

                if (Subject.CourseNumber < 1)
                {
                    MessageBox.Show("Некорректный номер курса");
                    return;
                }

                try
                {
                    if (Subject.Id == 0)
                        await _repository.AddAsync(Subject);
                    else
                        await _repository.UpdateAsync(Subject);

                    if (param is Window window)
                        window.DialogResult = true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                }
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
