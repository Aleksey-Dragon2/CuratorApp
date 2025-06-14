using CuratorApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CuratorApp.Views
{
    /// <summary>
    /// Логика взаимодействия для MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly SessionService _sessionService;

        public MainView()
        {
            InitializeComponent();

            _sessionService = new SessionService();
            _sessionService.SessionExpired += OnSessionExpired;
            _sessionService.Start();

            // Пример: имитация активности по таймеру или действиям пользователя
            MouseMove += async (_, _) => await _sessionService.NotifyActivityAsync();
            KeyDown += async (_, _) => await _sessionService.NotifyActivityAsync();
            UpdateButton.Click += async (_, _) =>
            {
                await _sessionService.NotifyActivityAsync();
                MessageBox.Show("Кнопка нажата");
            };
        }

        private void OnSessionExpired()
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show("Сессия завершена. Войдите снова.");
                var login = new LoginView();
                login.Show();
                Close();
            });
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _sessionService.Stop();
            new AuthService().Logout();

            var login = new LoginView();
            login.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
