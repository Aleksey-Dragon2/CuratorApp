using CuratorApp.Data;
using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.ViewModel;
using CuratorApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModels
{
    public class CuratorAccountViewModel : INotifyPropertyChanged
    {
        private readonly ICuratorRepository _curatorRepository;
        private readonly IGroupRepository _groupRepository;

        private Curator _curator;

        public ObservableCollection<Group> Groups { get; set; } = new();
        public ICommand OpenTemplatesCommand { get; }

        private Group? _selectedGroup;
        public Group? SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (SetField(ref _selectedGroup, value))
                {
                    if (value != null)
                        GroupId = value.Id;
                }
            }
        }

        private string _firstName = "";
        public string FirstName
        {
            get => _firstName;
            set => SetField(ref _firstName, value);
        }

        private string _lastName = "";
        public string LastName
        {
            get => _lastName;
            set => SetField(ref _lastName, value);
        }

        private string? _phone;
        public string? Phone
        {
            get => _phone;
            set => SetField(ref _phone, value);
        }

        private int _groupId;
        public int GroupId
        {
            get => _groupId;
            set => SetField(ref _groupId, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand OpenStudentsCommand { get; }
        public ICommand OpenPerformanceCommand { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public CuratorAccountViewModel(ICuratorRepository curatorRepository, IGroupRepository groupRepository, Curator curator)
        {
            _curatorRepository = curatorRepository;
            _curator = curator;
            _groupRepository = groupRepository;

            FirstName = curator.FirstName;
            LastName = curator.LastName;
            Phone = curator.Phone;
            GroupId = curator.GroupId;

            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            OpenStudentsCommand = new RelayCommand(_ => OpenStudents());
            OpenPerformanceCommand = new RelayCommand(_ => OpenPerformance());
            OpenTemplatesCommand = new RelayCommand(_ => OpenTemplates());
            LoadGroups();
        }

        private async void LoadGroups()
        {
            var groups = await _curatorRepository.GetGroupsAsync();
            Groups.Clear();
            foreach (var group in groups)
                Groups.Add(group);

            SelectedGroup = Groups.FirstOrDefault(g => g.Id == GroupId);
        }

        private async Task SaveAsync()
        {
            _curator.FirstName = FirstName;
            _curator.LastName = LastName;
            _curator.Phone = Phone;
            if (SelectedGroup != null)
                _curator.GroupId = SelectedGroup.Id;

            try
            {
                await _curatorRepository.UpdateAsync(_curator);
                // Можно показать сообщение об успехе
                MessageBox.Show("Данные успешно сохранены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void OpenStudents()
        {
            var studentRepo = new StudentRepository(new ApplicationContext());
            var vm = new StudentListViewModel(studentRepo, _groupRepository, _curator.GroupId);

            var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault(w => w.DataContext == this);

            mainWindow?.Hide();

            var studentsWindow = new StudentsWindow(vm);

            studentsWindow.Closed += (s, e) =>
            {
                mainWindow?.Show();
                studentsWindow.Closed -= (s, e) => mainWindow?.Show();
            };

            studentsWindow.ShowDialog();
        }



        private void OpenPerformance()
        {
            var annualRepo = new AnnualRecordRepository(new ApplicationContext());
            var documentRepo = new DocumentTemplateRepository(new ApplicationContext());
            var groupRepo = new GroupRepository(new ApplicationContext());
            var subjectRepo = new SubjectRepository(new ApplicationContext());
            var vm = new PerformanceViewModel(annualRepo, documentRepo, groupRepo, subjectRepo, _curator.GroupId);


            var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault(w => w.DataContext == this);

            mainWindow?.Hide();
            var performanceWindow = new PerformanceWindow(vm);
            performanceWindow.Closed += (s, e) =>
            {
                mainWindow?.Show();
                performanceWindow.Closed -= (s, e) => mainWindow?.Show();
            };
            performanceWindow.ShowDialog();
        }


        private void OpenTemplates()
        {
            var repo = new DocumentTemplateRepository(new ApplicationContext());
            var vm = new TemplateListViewModel(repo);

            var mainWindow = Application.Current.Windows
                .OfType<MainWindow>()
                .FirstOrDefault(w => w.DataContext == this);

            mainWindow?.Hide();

            var templatesWindow = new TemplateListWindow(vm);

            templatesWindow.Closed += (s, e) =>
            {
                mainWindow?.Show();
                templatesWindow.Closed -= (s, e) => mainWindow?.Show();
            };

            templatesWindow.ShowDialog();
        }


        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
