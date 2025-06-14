using CuratorApp.Services;
using CuratorApp.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace CuratorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var authService = new AuthService();
            bool isAuthorized = authService.IsAuthorized();

            if (isAuthorized)
            {
                var mainView = new MainView();
                mainView.Show();
            }
            else
            {
                var loginView = new LoginView();
                loginView.Show();
            }
        }
    }
}
