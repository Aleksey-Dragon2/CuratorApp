using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace CuratorApp.Services
{
    public class SessionService
    {
        private readonly System.Timers.Timer _inactivityTimer;
        private readonly TimeSpan _inactivityLimit = TimeSpan.FromMinutes(1);
        private readonly AuthService _authService;

        public event Action? SessionExpired;

        public SessionService()
        {
            _authService = new AuthService();

            _inactivityTimer = new System.Timers.Timer(_inactivityLimit.TotalMilliseconds);
            _inactivityTimer.Elapsed += (_, _) => OnSessionExpired();
            _inactivityTimer.AutoReset = false;
        }

        // Вызывается при любом действии пользователя
        public async Task NotifyActivityAsync()
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start();

            var tokens = _authService.GetStoredTokens();

            // Если access_token скоро истечет — обновим
            if (tokens != null && tokens.AccessTokenExpiration <= DateTime.UtcNow.AddMinutes(30))
            {
                await _authService.RefreshTokenAsync();
            }
        }

        private void OnSessionExpired()
        {
            _authService.Logout();
            SessionExpired?.Invoke(); // подписка на событие, можно переключить экран
        }

        public void Stop() => _inactivityTimer.Stop();
        public void Start() => _inactivityTimer.Start();
    }
}
