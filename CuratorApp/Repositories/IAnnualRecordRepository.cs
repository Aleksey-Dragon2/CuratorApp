using CuratorApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Repositories
{
    public interface IAnnualRecordRepository
    {
        Task<List<AnnualRecord>> GetByGroupIdAsync(int groupId);
        Task<List<AnnualRecord>> GetByStudentIdAsync(int studentId);
        Task<List<Student>> GetStudentsByGroupIdAsync(int groupId);
        Task<List<Subject>> GetAllSubjectsAsync();

        Task AddAsync(AnnualRecord record);
        Task UpdateAsync(AnnualRecord record);
        Task DeleteAsync(int id);
    }


}
