using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.ViewModels
{
    public class StudentDetailViewModel
    {
        private readonly Student _student;
        private readonly IDocumentTemplateRepository _templateRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IAnnualRecordRepository _annualRecordRepo;
        private readonly int _groupId;

        public ObservableCollection<DocumentTemplate> Templates { get; } = new();
        public DocumentTemplate? SelectedTemplate { get; set; }

        public string FullName => $"{_student.LastName} {_student.FirstName} {_student.MiddleName}";
        public string Birthday => $"Дата рождения: {_student.Birthday:dd.MM.yyyy}";

        public string AgeInfo
        {
            get
            {
                int age = CalculateAge(_student.Birthday, DateTime.Today);
                return $"Возраст: {age} лет";
            }
        }

        public string Phone => $"Телефон: {_student.Phone}";
        public string Address => $"Адрес: {_student.Address}";
        public string EnrollmentYear => $"Год поступления: {_student.EnrollmentYear}";

        public ICommand GenerateCommand { get; }

        public StudentDetailViewModel(Student student,
            IDocumentTemplateRepository templateRepo,
            IGroupRepository groupRepo,
            IAnnualRecordRepository annualRecordRepository,
            int groupId)
        {
            _student = student;
            _templateRepo = templateRepo;
            _groupRepo = groupRepo;
            _annualRecordRepo = annualRecordRepository;
            _groupId = groupId;

            GenerateCommand = new RelayCommand(_ => Generate());

            LoadTemplates();
        }

        private static int CalculateAge(DateOnly birthday, DateTime today)
        {
            int age = today.Year - birthday.Year;
            var birthdayThisYear = new DateTime(today.Year, birthday.Month, birthday.Day);
            if (today < birthdayThisYear)
                age--;
            return age;
        }

        private async void LoadTemplates()
        {
            try
            {
                var individualTemplates = await _templateRepo.GetByTypeAsync(TemplateType.Individual);
                Templates.Clear();
                foreach (var t in individualTemplates)
                    Templates.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки шаблонов: " + ex.Message);
            }
        }

        private async void Generate()
        {
            if (SelectedTemplate == null)
            {
                MessageBox.Show("Выберите шаблон.");
                return;
            }

            try
            {
                var placeholders = await _templateRepo.GetKeywordsAsync(SelectedTemplate.Id);
                var group = await _groupRepo.GetByIdAsync(_groupId);

                if (group == null)
                {
                    MessageBox.Show("Не удалось загрузить информацию о группе.");
                    return;
                }

                var annualRecords = await _annualRecordRepo.GetByStudentIdAsync(_student.Id);
                int totalAbsences = annualRecords.Sum(r => r.AbsenceCount);
                var grades = annualRecords
                    .Where(r => r.FinalGrade.HasValue)
                    .Select(r => r.FinalGrade!.Value)
                    .ToList();
                double averageGrade = grades.Count > 0 ? grades.Average() : 0;

                var values = new Dictionary<string, string>();

                foreach (var key in placeholders)
                {
                    string placeholder = key.Placeholder;
                    switch (placeholder)
                    {
                        case "[ФИО]":
                            values[placeholder] = FullName;
                            break;
                        case "[Группа]":
                            values[placeholder] = group.Name ?? "";
                            break;
                        case "[Возраст]":
                            values[placeholder] = CalculateAge(_student.Birthday, DateTime.Today).ToString();
                            break;
                        case "[Пропуски]":
                            values[placeholder] = totalAbsences.ToString();
                            break;
                        case "[СреднийБалл]":
                            values[placeholder] = averageGrade.ToString("0.00");
                            break;
                        case "[ДатаРождения]":
                            values[placeholder] = _student.Birthday.ToString("dd.MM.yyyy");
                            break;
                        case "[Специальность]":
                            values[placeholder] = group.Specialization ?? "";
                            break;
                        default:
                            values[placeholder] = "";
                            break;
                    }
                }

                var processor = new TemplateProcessor();
                var studentFolder = $"{_student.LastName}_{_student.FirstName}";
                var dateTimeSuffix = DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
                var fileName = $"{SelectedTemplate.Name}_{_student.LastName}_{dateTimeSuffix}.docx";

                var output = processor.GenerateReport(
                    SelectedTemplate.TemplatePath,
                    values,
                    fileName,
                    groupName: group.Name!,
                    studentFolder: studentFolder
                );

                MessageBox.Show("Документ успешно создан:\n" + output);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при генерации:\n" + ex.Message);
            }
        }
    }
}
