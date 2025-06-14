using CuratorApp.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Windows;


namespace CuratorApp.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly TokenStorageService _tokenStorage;
        private readonly string _baseUrl = "https://localhost:7155"; // Адрес API

        public AuthService()
        {
            _httpClient = new HttpClient();
            _tokenStorage = new TokenStorageService();
        }
        public bool IsAuthorized()
        {
            var tokens = _tokenStorage.LoadTokens();
            if (tokens == null || string.IsNullOrEmpty(tokens.AccessToken))
                return false;

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokens.AccessToken);
            return jwt.ValidTo > DateTime.UtcNow;
        }
        // 🔐 Вход в систему
        public async Task<Tokens?> LoginAsync(UserCredentials credentials)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", credentials);
            if (!response.IsSuccessStatusCode)
                return null;

            var tokens = await response.Content.ReadFromJsonAsync<Tokens>();
            if (tokens != null)
                _tokenStorage.SaveTokens(tokens);

            return tokens;
        }

        // 📝 Регистрация
        public async Task<bool> RegisterAsync(UserRegistrationRequest registrationData)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/register", registrationData);
            return response.IsSuccessStatusCode;
        }

        // 🔄 Обновление токена
        public async Task<Tokens?> RefreshTokenAsync()
        {
            var currentTokens = _tokenStorage.LoadTokens();
            if (currentTokens == null || currentTokens.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return null;
            }

            var request = new { refreshToken = currentTokens.RefreshToken };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/refresh", request);

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Loh");
                return null;
            }

            var newTokens = await response.Content.ReadFromJsonAsync<Tokens>();
            if (newTokens != null)
            {
                _tokenStorage.SaveTokens(newTokens);
                MessageBox.Show("ne log");
            }

            return newTokens;
        }

        // 🚪 Выход из системы
        public void Logout()
        {
            _tokenStorage.ClearTokens();
        }
        public Tokens? GetStoredTokens() => _tokenStorage.LoadTokens();
    }
}
