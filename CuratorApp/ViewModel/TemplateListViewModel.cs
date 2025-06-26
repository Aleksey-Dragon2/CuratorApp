using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.ViewModels;
using CuratorApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class TemplateListViewModel : INotifyPropertyChanged
    {
        private readonly IDocumentTemplateRepository _repo;

        public ObservableCollection<DocumentTemplate> Templates { get; set; } = new();

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

        public ICommand CreateCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public TemplateListViewModel(IDocumentTemplateRepository repo)
        {
            _repo = repo;

            CreateCommand = new RelayCommand(_ => CreateTemplate());
            EditCommand = new RelayCommand(_ => EditTemplate(), _ => SelectedTemplate != null);
            DeleteCommand = new RelayCommand(async _ => await DeleteTemplateAsync(), _ => SelectedTemplate != null);

            LoadTemplates();
        }

        private async void LoadTemplates()
        {
            try
            {
                Templates.Clear();
                var list = await _repo.GetAllAsync();
                foreach (var template in list)
                    Templates.Add(template);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки шаблонов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateTemplate()
        {
            try
            {
                var vm = new TemplateEditViewModel(_repo);
                var win = new TemplateEditWindow(vm);
                if (win.ShowDialog() == true)
                    LoadTemplates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании шаблона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditTemplate()
        {
            if (SelectedTemplate == null)
            {
                MessageBox.Show("Шаблон не выбран для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var vm = new TemplateEditViewModel(_repo, SelectedTemplate);
                var win = new TemplateEditWindow(vm);
                if (win.ShowDialog() == true)
                    LoadTemplates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании шаблона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteTemplateAsync()
        {
            if (SelectedTemplate == null)
            {
                MessageBox.Show("Выберите шаблон для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Удалить шаблон?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                await _repo.DeleteAsync(SelectedTemplate.Id);
                LoadTemplates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении шаблона: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
