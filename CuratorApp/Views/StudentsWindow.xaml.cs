using CuratorApp.Models;
using CuratorApp.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace CuratorApp.Views
{
    public partial class StudentsWindow : Window
    {
        public StudentsWindow(StudentListViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is StudentListViewModel vm && vm.OpenDetailCommand.CanExecute(null))
            {
                vm.OpenDetailCommand.Execute(null);
            }
        }
    }
}
