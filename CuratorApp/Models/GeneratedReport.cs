using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class GeneratedReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        public Group Group { get; set; } = null!;

        [Required]
        public int CourseNumber { get; set; }

        [Required]
        public DateTime GeneratedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string ReportType { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string FilePath { get; set; } = null!;
    }
}
