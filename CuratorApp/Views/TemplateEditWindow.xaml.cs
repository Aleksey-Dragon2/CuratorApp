using CuratorApp.ViewModels;
using System.Windows;

namespace CuratorApp.Views
{
    public partial class TemplateEditWindow : Window
    {
        public TemplateEditWindow(TemplateEditViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            vm.CloseAction = () => DialogResult = true;

            Loaded += async (_, _) => await vm.LoadTemplateKeywordsAsync();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
