using CuratorApp.Models;
using CuratorApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Windows;
namespace CuratorApp.Repositories
{
    public class CuratorRepository : ICuratorRepository
    {
        private readonly ApplicationContext _context = null!;
        private static readonly PasswordHasher<object> _passwordHasher = new();
        public CuratorRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<Curator> RegisterAsync(Curator curator)
        {
            var user = await _context.Curators.FirstOrDefaultAsync(c => c.Username == curator.Username);
            if (user != null)
                throw new("Curator is exists");

            curator.Password = _passwordHasher.HashPassword(curator, curator.Password);
            await _context.Curators.AddAsync(curator);

            await _context.SaveChangesAsync();

            curator.Password = null!;
            return curator;
        }

        public async Task<Curator> LoginAsync(Curator curator)
        {
            var user = await _context.Curators.FirstOrDefaultAsync(c => c.Username == curator.Username);
            if (user == null)
            {
                return null!;
            }
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, curator.Password);

            if (result == PasswordVerificationResult.Success)
            {
                return user;
            }
            return null!;
        }

        public async Task<Curator> UpdateAsync(Curator curator)
        {
            // Найти существующего куратора по Id
            var user = await _context.Curators.FirstOrDefaultAsync(c => c.Id == curator.Id);
            if (user == null)
                throw new Exception("Curator not found");

            user.Username = curator.Username;
            user.FirstName = curator.FirstName;
            user.LastName = curator.LastName;
            user.Phone = curator.Phone;
            user.GroupId = curator.GroupId;

            await _context.SaveChangesAsync();

            user.Password = null!;
            return user;
        }

        public async Task<List<Group>> GetGroupsAsync()
        {
            return await _context.Groups
                .AsNoTracking()
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curator = await _context.Curators.FirstOrDefaultAsync(c => c.Id == id);
            if (curator == null)
                return false;

            _context.Curators.Remove(curator);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}