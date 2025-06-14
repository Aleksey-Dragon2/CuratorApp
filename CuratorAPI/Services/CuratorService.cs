using CuratorAPI.Models;
using CuratorAPI.Repositories;
using CuratorApp.Data;
using BCrypt.Net;


namespace CuratorAPI.Services
{
    public class CuratorService : ICuratorService
    {
        private readonly ICuratorRepository _curatorRepository;
        private readonly ApplicationContext _context;

        public CuratorService(ICuratorRepository curatorRepository, ApplicationContext context)
        {
            _curatorRepository = curatorRepository;
            _context = context;
        }

        public async Task<bool> RegisterAsync(Curator curator, string password)
        {
            var existing = await _curatorRepository.GetByUsernameAsync(curator.Username);
            if (existing != null) return false;

            curator.Password = BCrypt.Net.BCrypt.HashPassword(password);

            await _curatorRepository.AddAsync(curator);
            return true;
        }

        public async Task<Curator?> AuthenticateAsync(string username, string password)
        {
            var curator = await _curatorRepository.GetByUsernameAsync(username);
            if (curator == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, curator.Password))
                return null;

            return curator;
        }

        public async Task SaveRefreshTokenAsync(Curator curator, string refreshToken, DateTime expires)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                Expires = expires,
                CuratorId = curator.Id,
                IsRevoked = false
            };
            curator.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ValidateRefreshTokenAsync(string username, string refreshToken)
        {
            var curator = await _curatorRepository.GetByUsernameAsync(username);
            if (curator == null) return false;

            var token = curator.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken && !t.IsRevoked && t.Expires > DateTime.UtcNow);
            return token != null;
        }

        public async Task RevokeRefreshTokenAsync(string username, string refreshToken)
        {
            var curator = await _curatorRepository.GetByUsernameAsync(username);
            if (curator == null) return;

            var token = curator.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }
    }

}
