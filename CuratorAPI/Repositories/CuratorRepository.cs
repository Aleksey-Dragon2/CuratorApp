using CuratorAPI.Models;
using CuratorApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CuratorAPI.Repositories
{
    public class CuratorRepository : ICuratorRepository
    {
        private readonly ApplicationContext _context;

        public CuratorRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Curator?> GetByUsernameAsync(string username)
        {
            return await _context.Curators
                .Include(c => c.RefreshTokens)
                .FirstOrDefaultAsync(c => c.Username == username);
        }

        public async Task AddAsync(Curator curator)
        {
            _context.Curators.Add(curator);
            await _context.SaveChangesAsync();
        }
    }

}
