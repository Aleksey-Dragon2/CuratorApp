using CuratorApp.Models;
using CuratorApp.Services;
using System.Windows;
using System.Windows.Controls;

namespace CuratorApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly AuthService _authService = new();
        private readonly SessionService _sessionService = new();

        public LoginView()
        {
            InitializeComponent();

            // Попытка авто-входа
            var tokens = _authService.GetStoredTokens();
            if (tokens != null && tokens.AccessTokenExpiration > DateTime.UtcNow)
            {
                OpenMainWindow();
            }
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            var credentials = new UserCredentials
            {
                Username = UsernameBox.Text,
                Password = PasswordBox.Password
            };

            var tokens = await _authService.LoginAsync(credentials);
            if (tokens == null)
            {
                MessageBox.Show("Ошибка входа. Проверьте логин и пароль.");
                return;
            }

            _sessionService.Start();
            _sessionService.SessionExpired += () =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("Сессия завершена. Пожалуйста, войдите снова.");
                    var login = new LoginView();
                    login.Show();
                    this.Close();
                });
            };

            OpenMainWindow();
        }

        private void OpenMainWindow()
        {
            var mainWindow = new MainView();
            mainWindow.Show();
            this.Close();
        }
    }
}
