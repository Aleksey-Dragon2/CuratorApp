using CuratorApp.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext? _context;
        private readonly DbSet<T>? _dbSet;
        
        public Repository(ApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet!.ToList()!;
        }

        public T Get(int id)
        {
            return _dbSet!.Find(id)!;
        }

        public void Create(T item)
        {
            _dbSet!.Add(item);
        }
        public void Update(T item)
        {
            _dbSet!.Update(item);
        }
        public void Delete(T item)
        {
            _dbSet!.Remove(item);
        }
        public void Save()
        {
            _context!.SaveChanges();
        }
        public void Dispose()
        {
            _context!.Dispose();
        }
    }
}
