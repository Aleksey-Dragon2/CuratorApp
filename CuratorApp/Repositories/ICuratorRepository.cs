using CuratorApp.Models;
namespace CuratorApp.Repositories
{
    public interface ICuratorRepository
    {
        Task<Curator> LoginAsync(Curator curator);
        Task<Curator> RegisterAsync(Curator curator);
        Task<Curator> UpdateAsync(Curator curator);
        Task<List<Group>> GetGroupsAsync();
        Task<List<Group>> GetAvailableGroupsAsync(int curatorGroupId);

        Task<bool> DeleteAsync(int curatorId);
    }
}
