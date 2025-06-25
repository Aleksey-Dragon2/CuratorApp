using CuratorApp.Models;
using CuratorApp.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using CuratorApp.ViewModels;
using CuratorApp.Views;

namespace CuratorApp.ViewModel
{
    public class TemplateListViewModel : INotifyPropertyChanged
    {
        private readonly IDocumentTemplateRepository _repo;

        public ObservableCollection<DocumentTemplate> Templates { get; set; } = new();

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        private DocumentTemplate? _selectedTemplate;
        public DocumentTemplate? SelectedTemplate
        {
            get => _selectedTemplate;
            set
            {
                _selectedTemplate = value;
                OnPropertyChanged(nameof(SelectedTemplate));
            }
        }

        public TemplateListViewModel(IDocumentTemplateRepository repo)
        {
            _repo = repo;

            CreateCommand = new RelayCommand(_ => Create());
            EditCommand = new RelayCommand(_ => Edit(), _ => SelectedTemplate != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteAsync(), _ => SelectedTemplate != null);

            LoadTemplates();
        }

        private async void LoadTemplates()
        {
            Templates.Clear();
            var list = await _repo.GetAllAsync();
            foreach (var template in list)
                Templates.Add(template);
        }

        private void Create()
        {
            var vm = new TemplateEditViewModel(_repo); // редактирование без шаблона
            var win = new TemplateEditWindow(vm);
            if (win.ShowDialog() == true)
                LoadTemplates();
        }

        private void Edit()
        {
            if (SelectedTemplate == null) return;
            var vm = new TemplateEditViewModel(_repo, SelectedTemplate);
            var win = new TemplateEditWindow(vm);
            if (win.ShowDialog() == true)
                LoadTemplates();
        }

        private async Task DeleteAsync()
        {
            if (SelectedTemplate == null) return;
            if (MessageBox.Show("Удалить шаблон?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _repo.DeleteAsync(SelectedTemplate.Id);
                LoadTemplates();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string prop) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

}
