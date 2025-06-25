using CuratorApp.Data;
using CuratorApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public class AnnualRecordRepository : IAnnualRecordRepository
    {
        private readonly ApplicationContext _context;

        public AnnualRecordRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<AnnualRecord>> GetByGroupIdAsync(int groupId)
        {
            return await _context.AnnualRecords
                .Include(r => r.Student)
                .Include(r => r.Subject)
                .Where(r => r.Student.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<AnnualRecord>> GetByStudentIdAsync(int studentId)
        {
            return await _context.AnnualRecords
                .Include(r => r.Subject)
                .Where(r => r.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Student>> GetStudentsByGroupIdAsync(int groupId)
        {
            return await _context.Students
                .Where(s => s.GroupId == groupId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(AnnualRecord record)
        {
            _context.AnnualRecords.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AnnualRecord record)
        {
            _context.AnnualRecords.Update(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var record = await _context.AnnualRecords.FindAsync(id);
            if (record != null)
            {
                _context.AnnualRecords.Remove(record);
                await _context.SaveChangesAsync();
            }
        }
    }
}
