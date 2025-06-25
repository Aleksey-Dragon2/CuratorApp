using CuratorApp.Models;
using CuratorApp.Repositories;
using CuratorApp.ViewModels;
using System.Windows;

namespace CuratorApp.Views
{
    public partial class StudentDetailWindow : Window
    {
        private readonly Data.ApplicationContext context = new();
        public StudentDetailWindow(Student student, int groupId)
        {
            InitializeComponent();

            var templateRepo = new DocumentTemplateRepository(context);
            var groupRepo = new GroupRepository(context);
            var annualRepo = new AnnualRecordRepository(context);
            DataContext = new StudentDetailViewModel(student, templateRepo, groupRepo, annualRepo, groupId);
        }
    }
}
