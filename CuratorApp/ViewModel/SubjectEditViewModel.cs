using CuratorApp.Models;
using CuratorApp.Repositories;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class SubjectEditViewModel : INotifyPropertyChanged
    {
        private readonly ISubjectRepository _repository;
        public Subject Subject { get; }

        public ICommand SaveCommand { get; }

        public SubjectEditViewModel(Subject subject, ISubjectRepository repository)
        {
            Subject = subject;
            _repository = repository;
            SaveCommand = new RelayCommand(async w =>
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

                if (Subject.Id == 0)
                    await _repository.AddAsync(Subject);
                else
                    await _repository.UpdateAsync(Subject);

                if (w is Window window)
                    window.DialogResult = true;
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
