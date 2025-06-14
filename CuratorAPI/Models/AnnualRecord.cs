using System.ComponentModel.DataAnnotations;

namespace CuratorAPI.Models
{
    public class AnnualRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int StudentId { get; set; }

        public Student Student { get; set; } = null!;

        [Required]
        public int SubjectId { get; set; }

        public Subject Subject { get; set; } = null!;

        [Required]
        public int CourseNumber { get; set; }

        public int? FinalGrade { get; set; }

        [Required]
        public int AbsenceCount { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
