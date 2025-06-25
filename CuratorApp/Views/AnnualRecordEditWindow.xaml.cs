using CuratorApp.ViewModel;
using System.Windows;

namespace CuratorApp.Views
{
    public partial class AnnualRecordEditWindow : Window
    {
        public AnnualRecordEditWindow(AnnualRecordEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.CloseRequested += (s, e) =>
            {
                DialogResult = e;
                Close();
            };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
