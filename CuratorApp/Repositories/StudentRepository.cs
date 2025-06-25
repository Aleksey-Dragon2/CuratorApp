using CuratorApp.Data;
using CuratorApp.Models;
using Microsoft.EntityFrameworkCore;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationContext _context;
    public StudentRepository(ApplicationContext context) => _context = context;

    public async Task<IEnumerable<Student>> GetByGroupIdAsync(int groupId)
    {
        return await _context.Students
            .Where(s => s.GroupId == groupId)
            .AsNoTracking() // <=== КЛЮЧ
            .ToListAsync();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        var local = _context.Students.Local.FirstOrDefault(s => s.Id == student.Id);
        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        _context.Attach(student);
        _context.Entry(student).State = EntityState.Modified;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int studentId)
    {
        var student = await _context.Students.FindAsync(studentId);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
