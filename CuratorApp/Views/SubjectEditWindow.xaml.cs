using System.Windows;

namespace CuratorApp.Views
{
    public partial class SubjectEditWindow : Window
    {
        public SubjectEditWindow(object dataContext)
        {
            InitializeComponent();
            DataContext = dataContext;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
