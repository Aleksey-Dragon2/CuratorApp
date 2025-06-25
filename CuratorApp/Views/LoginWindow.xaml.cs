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
            LoadAvailableGroups();
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
                MainWindow mainWindow = new(_curatorRepository, _groupRepository, result, this);
                mainWindow.Show();
                this.Hide();
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
            string password = RegisterPasswordBox.Password;
            string confirmPassword = RegisterConfirmPasswordBox.Password;

            string firstName = RegisterFirstNameTextBox.Text.Trim();
            string lastName = RegisterLastNameTextBox.Text.Trim();
            string phone = RegisterPhoneTextBox.Text.Trim();

            var selectedGroup = GroupComboBox.SelectedItem as Group;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || selectedGroup == null ||
                string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                MessageBox.Show("Все обязательные поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                FirstName = firstName,
                LastName = lastName,
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                GroupId = selectedGroup.Id
            };

            try
            {
                var registeredUser = await _curatorRepository.RegisterAsync(curator);
                if (registeredUser != null)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadAvailableGroups()
        {
            var groups = await _curatorRepository.GetAvailableGroupsAsync(0); // 0 = без исключений
            GroupComboBox.ItemsSource = groups;
        }
        private async void CreateGroupButton_Click(object sender, RoutedEventArgs e)
        {
            string name = GroupNameTextBox.Text.Trim();
            string courseText = CourseNumberTextBox.Text.Trim();
            string specialization = SpecializationTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Название группы обязательно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(courseText, out int courseNumber) || courseNumber <= 0)
            {
                MessageBox.Show("Некорректный номер курса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var group = new Group
            {
                Name = name,
                CourseNumber = courseNumber,
                Specialization = string.IsNullOrWhiteSpace(specialization) ? null : specialization
            };

            try
            {
                await _groupRepository.CreateAsync(group);
                MessageBox.Show("Группа успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                GroupNameTextBox.Clear();
                CourseNumberTextBox.Clear();
                SpecializationTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}