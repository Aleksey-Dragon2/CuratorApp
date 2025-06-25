using CuratorApp.Models;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetByGroupIdAsync(int groupId);
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(int studentId);
}
