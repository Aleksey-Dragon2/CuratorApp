using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.Services;
using CuratorApp.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModel
{
    public class SubjectPerformance
    {
        public string SubjectName { get; set; } = "";
        public double AverageGrade { get; set; }
    }

    public class PerformanceViewModel : INotifyPropertyChanged
    {
        private readonly IAnnualRecordRepository _repo;
        private readonly IDocumentTemplateRepository _templateRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly int _groupId;

        public ObservableCollection<AnnualRecord> Records { get; set; } = new();
        public ObservableCollection<SubjectPerformance> SubjectAverages { get; set; } = new();

        public ObservableCollection<DocumentTemplate> GroupTemplates { get; set; } = new();
        public DocumentTemplate? SelectedGroupTemplate { get; set; }

        public double AverageGrade => Records.Where(r => r.FinalGrade != null).Average(r => r.FinalGrade) switch
        {
            double avg => Math.Round(avg, 2),
            _ => 0
        };

        public int TotalAbsences => Records.Sum(r => r.AbsenceCount);

        public ICommand GenerateGroupReportCommand { get; }
        public ICommand AddRecordCommand { get; }
        public ICommand EditRecordCommand { get; }
        public ICommand DeleteRecordCommand { get; }

        private AnnualRecord? _selectedRecord;
        public AnnualRecord? SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                _selectedRecord = value;
                OnPropertyChanged(nameof(SelectedRecord));
            }
        }

        public PerformanceViewModel(
            IAnnualRecordRepository repo,
            IDocumentTemplateRepository templateRepo,
            IGroupRepository groupRepo,
            int groupId)
        {
            _repo = repo;
            _templateRepo = templateRepo;
            _groupRepo = groupRepo;
            _groupId = groupId;

            GenerateGroupReportCommand = new RelayCommand(async _ => await GenerateGroupReportAsync(), _ => SelectedGroupTemplate != null);
            AddRecordCommand = new RelayCommand(_ => AddRecord());
            EditRecordCommand = new RelayCommand(_ => EditRecord(), _ => SelectedRecord != null);
            DeleteRecordCommand = new RelayCommand(async _ => await DeleteRecordAsync(), _ => SelectedRecord != null);

            LoadData(groupId);
            LoadGroupTemplates();
        }

        private async void LoadData(int groupId)
        {
            var data = await _repo.GetByGroupIdAsync(groupId);
            Records.Clear();
            foreach (var r in data)
                Records.Add(r);

            var subjectGroups = data
                .Where(r => r.FinalGrade != null)
                .GroupBy(r => r.Subject.Name)
                .Select(g => new SubjectPerformance
                {
                    SubjectName = g.Key,
                    AverageGrade = Math.Round(g.Average(r => r.FinalGrade ?? 0), 2)
                });

            SubjectAverages.Clear();
            foreach (var s in subjectGroups)
                SubjectAverages.Add(s);

            OnPropertyChanged(nameof(AverageGrade));
            OnPropertyChanged(nameof(TotalAbsences));
        }

        private async void LoadGroupTemplates()
        {
            GroupTemplates.Clear();
            var templates = await _templateRepo.GetByTypeAsync(TemplateType.Group);
            foreach (var t in templates)
                GroupTemplates.Add(t);
        }

        private async Task GenerateGroupReportAsync()
        {
            if (SelectedGroupTemplate == null) return;

            try
            {
                var group = await _groupRepo.GetByIdAsync(_groupId);
                if (group == null)
                {
                    MessageBox.Show("Группа не найдена");
                    return;
                }

                var keywords = await _templateRepo.GetKeywordsAsync(SelectedGroupTemplate.Id);
                var values = new System.Collections.Generic.Dictionary<string, string>();

                foreach (var key in keywords)
                {
                    values[key.Placeholder] = key.Placeholder switch
                    {
                        "[Группа]" => group.Name,
                        "[СреднийБалл]" => AverageGrade.ToString("0.00"),
                        "[Пропуски]" => TotalAbsences.ToString(),
                        "[Дата]" => DateTime.Now.ToString("dd.MM.yyyy"),
                        "[Время]" => DateTime.Now.ToString("HH:mm"),
                        "[Специальность]" => group.Specialization ?? "",
                        _ => ""
                    };
                }

                var processor = new TemplateProcessor();
                var fileName = $"{SelectedGroupTemplate.Name}_{DateTime.Now:dd-MM-yyyy_HH-mm}.docx";

                var output = processor.GenerateReport(
                    SelectedGroupTemplate.TemplatePath,
                    values,
                    fileName,
                    groupName: group.Name
                );

                MessageBox.Show("Отчёт сгенерирован:\n" + output);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка генерации отчёта:\n" + ex.Message);
            }
        }

        private void AddRecord()
        {
            var newRecord = new AnnualRecord();
            var vm = new AnnualRecordEditViewModel(newRecord, _repo, _groupId);
            var win = new AnnualRecordEditWindow(vm);
            if (win.ShowDialog() == true)
                LoadData(_groupId);
        }

        private void EditRecord()
        {
            if (SelectedRecord == null) return;
            var copy = new AnnualRecord
            {
                Id = SelectedRecord.Id,
                StudentId = SelectedRecord.StudentId,
                SubjectId = SelectedRecord.SubjectId,
                CourseNumber = SelectedRecord.CourseNumber,
                FinalGrade = SelectedRecord.FinalGrade,
                AbsenceCount = SelectedRecord.AbsenceCount
            };
            var vm = new AnnualRecordEditViewModel(copy, _repo, _groupId);
            var win = new AnnualRecordEditWindow(vm);
            if (win.ShowDialog() == true)
                LoadData(_groupId);
        }

        private async Task DeleteRecordAsync()
        {
            if (SelectedRecord == null) return;
            if (MessageBox.Show("Удалить запись?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                await _repo.DeleteAsync(SelectedRecord.Id);
                LoadData(_groupId);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
