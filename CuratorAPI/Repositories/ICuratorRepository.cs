using CuratorAPI.Models;

namespace CuratorAPI.Repositories
{
    public interface ICuratorRepository
    {
        Task<Curator?> GetByUsernameAsync(string username);
        Task AddAsync(Curator curator);
    }

}
