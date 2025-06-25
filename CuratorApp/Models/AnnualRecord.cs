using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
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
    }
}
