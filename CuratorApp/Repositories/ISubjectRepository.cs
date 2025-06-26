using CuratorApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId);
        Task AddAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(int id);
    }
}
