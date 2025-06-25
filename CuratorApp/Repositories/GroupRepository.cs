using CuratorApp.Data;
using CuratorApp.Models;

using Microsoft.EntityFrameworkCore;

namespace CuratorApp.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationContext _context;
        public GroupRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task CreateAsync(Group group)
        {
            if (await _context.Groups.AnyAsync(g => g.Name == group.Name))
                throw new Exception("Группа с таким названием уже существует.");

            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }


        public async Task<Group?> GetByIdAsync(int id)
        {
            return await _context.Groups.FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
