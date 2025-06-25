using CuratorApp.Models;
using CuratorApp.Repositories;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModels
{
    public class TemplateEditViewModel : INotifyPropertyChanged
    {
        private readonly IDocumentTemplateRepository _repo;
        private readonly DocumentTemplate? _originalTemplate;

        public string Name { get; set; } = "";
        public string? SelectedFilePath { get; set; }

        public ObservableCollection<string> AvailableKeys { get; set; } = new();
        public ObservableCollection<string> SelectedKeys { get; set; } = new();

        public ICommand BrowseFileCommand { get; }
        public ICommand AddKeyCommand { get; }
        public ICommand RemoveKeyCommand { get; }
        public ICommand SaveCommand { get; }
        public IEnumerable<TemplateType> TemplateTypes => Enum.GetValues(typeof(TemplateType)).Cast<TemplateType>();

        private string? _selectedAvailableKey;
        public string? SelectedAvailableKey
        {
            get => _selectedAvailableKey;
            set { _selectedAvailableKey = value; OnPropertyChanged(nameof(SelectedAvailableKey)); }
        }

        private string? _selectedUsedKey;
        public string? SelectedUsedKey
        {
            get => _selectedUsedKey;
            set { _selectedUsedKey = value; OnPropertyChanged(nameof(SelectedUsedKey)); }
        }

        public Action? CloseAction { get; set; }
        public event PropertyChangedEventHandler? PropertyChanged;
        private TemplateType _selectedTemplateType;
        public TemplateType SelectedTemplateType
        {
            get => _selectedTemplateType;
            set
            {
                if (_selectedTemplateType != value)
                {
                    _selectedTemplateType = value;
                    OnPropertyChanged(nameof(SelectedTemplateType));
                }
            }
        }

        public TemplateEditViewModel(IDocumentTemplateRepository repo, DocumentTemplate? template = null)
        {
            _repo = repo;
            _originalTemplate = template;

            if (template != null)
            {
                // Заполняем поля из переданного шаблона
                Name = template.Name;
                SelectedFilePath = template.TemplatePath;
                SelectedTemplateType = template.TemplateType;
            }
            else
            {
                SelectedTemplateType = TemplateType.Individual;
            }

            BrowseFileCommand = new RelayCommand(_ => BrowseFile());
            AddKeyCommand = new RelayCommand(_ =>
            {
                if (SelectedAvailableKey != null && !SelectedKeys.Contains(SelectedAvailableKey))
                    SelectedKeys.Add(SelectedAvailableKey);
            });

            RemoveKeyCommand = new RelayCommand(_ =>
            {
                if (SelectedUsedKey != null)
                    SelectedKeys.Remove(SelectedUsedKey);
            });

            SaveCommand = new RelayCommand(async _ => await SaveAsync());

            LoadDefaultKeys();

            if (template != null)
            {
                Name = template.Name;
                SelectedFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, template.TemplatePath);
            }
        }

        private void LoadDefaultKeys()
        {
            AvailableKeys.Clear();
            var keys = new[]
            {
                "[ФИО]", "[Возраст]", "[Группа]", "[Специальность]",
                "[СреднийБалл]", "[Пропуски]", "[ДатаРождения]"
            };
            foreach (var key in keys)
                AvailableKeys.Add(key);
        }

        private void BrowseFile()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Word Documents|*.docx",
                Title = "Выберите шаблон Word"
            };

            if (dialog.ShowDialog() == true)
            {
                SelectedFilePath = dialog.FileName;
                OnPropertyChanged(nameof(SelectedFilePath));
            }
        }

        public async Task LoadTemplateKeywordsAsync()
        {
            if (_originalTemplate == null)
                return;

            var keywords = await _repo.GetKeywordsAsync(_originalTemplate.Id);
            SelectedKeys.Clear();
            foreach (var k in keywords)
                SelectedKeys.Add(k.Placeholder);
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(SelectedFilePath))
            {
                MessageBox.Show("Укажите имя и выберите файл");
                return;
            }

            string targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            Directory.CreateDirectory(targetDir);

            string destPath = Path.Combine(targetDir, Path.GetFileName(SelectedFilePath));

            try
            {
                File.Copy(SelectedFilePath, destPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при копировании файла: " + ex.Message);
                return;
            }

            var template = new DocumentTemplate
            {
                Id = _originalTemplate?.Id ?? 0,
                Name = Name,
                TemplatePath = Path.Combine("Templates", Path.GetFileName(SelectedFilePath)),
                TemplateType = SelectedTemplateType
            };

            if (_originalTemplate == null)
                await _repo.CreateAsync(template);
            else
                await _repo.UpdateAsync(template);

            await _repo.SaveKeywordsAsync(template.Id, SelectedKeys.ToList());

            CloseAction?.Invoke();
        }


        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
