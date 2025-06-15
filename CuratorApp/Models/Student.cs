using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [MaxLength(100)]
        public string? MiddleName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public DateOnly Birthday { get; set; }

        [Required]
        public string Address { get; set; } = null!;

        [Required]
        public int GroupId { get; set; }

        public Group Group { get; set; } = null!;

        [Required]
        public int EnrollmentYear { get; set; }

        public ICollection<AnnualRecord> AnnualRecords { get; set; } = new List<AnnualRecord>();
    }
}
