using CuratorApp.Repositories;
using System.Printing;
using System.Windows;
using CuratorApp.Models;
using CuratorApp.Views;
using System.Text.RegularExpressions;
using System.Linq;
using Group = CuratorApp.Models.Group;

namespace CuratorApp
{
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

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = LoginUsernameTextBox.Text.Trim();
            string password = LoginPasswordBox.Password;

            // Валидация входа
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Введите имя пользователя", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка входа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = RegisterUsernameTextBox.Text.Trim();
            string password = RegisterPasswordBox.Password;
            string confirmPassword = RegisterConfirmPasswordBox.Password;
            string firstName = RegisterFirstNameTextBox.Text.Trim();
            string lastName = RegisterLastNameTextBox.Text.Trim();
            string phone = RegisterPhoneTextBox.Text.Trim();
            var selectedGroup = GroupComboBox.SelectedItem as Group;

            // Валидация регистрации
            var errors = new System.Text.StringBuilder();

            // Проверка имени пользователя
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
                errors.AppendLine("Имя пользователя должно содержать минимум 4 символа");
            else if (username.Length > 20)
                errors.AppendLine("Имя пользователя должно быть не длиннее 20 символов");
            else if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                errors.AppendLine("Имя пользователя может содержать только буквы, цифры и подчеркивание");

            // Проверка пароля
            if (string.IsNullOrWhiteSpace(password))
                errors.AppendLine("Введите пароль");
            else if (password.Length < 6)
                errors.AppendLine("Пароль должен содержать минимум 6 символов");
            else if (password.Length > 30)
                errors.AppendLine("Пароль должен быть не длиннее 30 символов");
            else if (!password.Any(char.IsDigit))
                errors.AppendLine("Пароль должен содержать хотя бы одну цифру");
            else if (!password.Any(char.IsUpper))
                errors.AppendLine("Пароль должен содержать хотя бы одну заглавную букву");

            if (password != confirmPassword)
                errors.AppendLine("Пароли не совпадают");

            // Проверка имени и фамилии
            if (string.IsNullOrWhiteSpace(firstName))
                errors.AppendLine("Введите имя");
            else if (firstName.Length > 50)
                errors.AppendLine("Имя слишком длинное (максимум 50 символов)");

            if (string.IsNullOrWhiteSpace(lastName))
                errors.AppendLine("Введите фамилию");
            else if (lastName.Length > 50)
                errors.AppendLine("Фамилия слишком длинная (максимум 50 символов)");

            // Проверка телефона
            if (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, @"^\+?[0-9\s\-\(\)]{10,15}$"))
                errors.AppendLine("Некорректный формат телефона");

            // Проверка группы
            if (selectedGroup == null)
                errors.AppendLine("Выберите группу");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var curator = new Curator
                {
                    Username = username,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = string.IsNullOrWhiteSpace(phone) ? null : phone,
                    GroupId = selectedGroup.Id
                };

                var registeredUser = await _curatorRepository.RegisterAsync(curator);
                if (registeredUser != null)
                {
                    MessageBox.Show("Регистрация прошла успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Очистка полей после успешной регистрации
                    RegisterUsernameTextBox.Clear();
                    RegisterPasswordBox.Clear();
                    RegisterConfirmPasswordBox.Clear();
                    RegisterFirstNameTextBox.Clear();
                    RegisterLastNameTextBox.Clear();
                    RegisterPhoneTextBox.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка регистрации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void LoadAvailableGroups()
        {
            try
            {
                var groups = await _curatorRepository.GetAvailableGroupsAsync(0);
                GroupComboBox.ItemsSource = groups;
                GroupComboBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки групп: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void CreateGroupButton_Click(object sender, RoutedEventArgs e)
        {
            string name = GroupNameTextBox.Text.Trim();
            string courseText = CourseNumberTextBox.Text.Trim();
            string specialization = SpecializationTextBox.Text.Trim();

            var errors = new System.Text.StringBuilder();

            // Валидация названия группы
            if (string.IsNullOrWhiteSpace(name))
                errors.AppendLine("Введите название группы");
            else if (name.Length > 20)
                errors.AppendLine("Название группы слишком длинное (максимум 20 символов)");

            // Валидация номера курса
            if (!int.TryParse(courseText, out int courseNumber) || courseNumber <= 0 || courseNumber > 6)
                errors.AppendLine("Некорректный номер курса (допустимо 1-6)");

            // Валидация специализации
            if (!string.IsNullOrWhiteSpace(specialization) && specialization.Length > 100)
                errors.AppendLine("Специализация слишком длинная (максимум 100 символов)");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var group = new Group
                {
                    Name = name,
                    CourseNumber = courseNumber,
                    Specialization = string.IsNullOrWhiteSpace(specialization) ? null : specialization
                };

                await _groupRepository.CreateAsync(group);
                MessageBox.Show("Группа успешно создана!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей и обновление списка групп
                GroupNameTextBox.Clear();
                CourseNumberTextBox.Clear();
                SpecializationTextBox.Clear();
                LoadAvailableGroups();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании группы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}