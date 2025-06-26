using CuratorApp.Data;
using CuratorApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly ApplicationContext _context;

        public SubjectRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId)
        {
            return await _context.AnnualRecords
                .Where(ar => ar.Student.GroupId == groupId)
                .Select(ar => ar.Subject)
                .Distinct()
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subject subject)
        {
            var existing = await _context.Subjects.FindAsync(subject.Id);
            if (existing == null)
                throw new Exception("Предмет не найден");

            existing.Name = subject.Name;
            existing.CourseNumber = subject.CourseNumber;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return;

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
        }
    }
}
