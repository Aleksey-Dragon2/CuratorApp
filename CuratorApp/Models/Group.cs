using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        public int CourseNumber { get; set; }

        [MaxLength(100)]
        public string? Specialization { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();

        public ICollection<GeneratedReport> GeneratedReports { get; set; } = new List<GeneratedReport>();
    }
}
