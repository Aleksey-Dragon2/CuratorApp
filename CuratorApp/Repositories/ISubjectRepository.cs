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
    }
}