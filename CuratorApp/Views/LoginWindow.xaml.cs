using CuratorApp.Repositories;
using System.Printing;
using System.Windows;
using CuratorApp.Models;
using CuratorApp.Views;
namespace CuratorApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ICuratorRepository _curatorRepository;
        private readonly IGroupRepository _groupRepository;


        public LoginWindow(ICuratorRepository curatorRepository, IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
            _curatorRepository = curatorRepository;
            InitializeComponent();
        }

        // Обработчик кнопки Войти
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsernameTextBox.Text;
            string password = LoginPasswordBox.Password;

            var result = await _curatorRepository.LoginAsync(new Curator
            {
                Username = username,
                Password = password
            });

            if (result != null)
            {
                MainWindow mainWindow = new(_curatorRepository, _groupRepository, result);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверные имя пользователя или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Обработчик кнопки Зарегистрироваться
        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsernameTextBox.Text.Trim();
            string email = RegisterEmailTextBox.Text.Trim();
            string password = RegisterPasswordBox.Password;
            string confirmPassword = RegisterConfirmPasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Имя пользователя и пароль обязательны!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var curator = new Curator
            {
                Username = username,
                Password = password,
                // При регистрации можно добавить FirstName, LastName, Phone, GroupId
                FirstName = "Не указано",
                LastName = "Не указано",
                Phone = null,
                GroupId = 1 // Или брать из UI
            };

            try
            {
                var registeredUser = await _curatorRepository.RegisterAsync(curator);
                if (registeredUser != null)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Можно очистить поля или перейти к другому окну
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}