using System.Windows;
namespace CuratorApp
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        // Обработчик кнопки Войти
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsernameTextBox.Text;
            string password = LoginPasswordBox.Password;

            // TODO: Добавить логику аутентификации
            MessageBox.Show($"Попытка входа:\nПользователь: {username}\nПароль: {new string('*', password.Length)}");
        }

        // Обработчик кнопки Зарегистрироваться
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsernameTextBox.Text;
            string email = RegisterEmailTextBox.Text;
            string password = RegisterPasswordBox.Password;
            string confirmPassword = RegisterConfirmPasswordBox.Password;

            // TODO: Добавить логику регистрации
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Попытка регистрации:\nПользователь: {username}\nEmail: {email}\nПароль: {new string('*', password.Length)}");
        }
    }
}