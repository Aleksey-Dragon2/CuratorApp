using CuratorApp.Models;

namespace CuratorApp.Repositories
{
    public interface IGroupRepository
    {
        Task<Group?> GetByIdAsync(int id);

    }
}
