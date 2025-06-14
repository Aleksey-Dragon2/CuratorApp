using CuratorAPI.Models;

namespace CuratorAPI.Services
{
    public interface ICuratorService
    {
        Task<bool> RegisterAsync(Curator curator, string password);
        Task<Curator?> AuthenticateAsync(string username, string password);
        Task SaveRefreshTokenAsync(Curator curator, string refreshToken, DateTime expires);
        Task<bool> ValidateRefreshTokenAsync(string username, string refreshToken);
        Task RevokeRefreshTokenAsync(string username, string refreshToken);
    }

}
