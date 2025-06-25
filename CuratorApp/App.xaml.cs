using CuratorApp.Repositories;
using System.Configuration;
using System.Data;
using System.Windows;
using CuratorApp.Data;

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

            // Создание и отображение окна
            var loginWindow = new LoginWindow(new CuratorRepository(new ApplicationContext()),
                new GroupRepository(new ApplicationContext()));
            loginWindow.Show();
        }
    }

}
