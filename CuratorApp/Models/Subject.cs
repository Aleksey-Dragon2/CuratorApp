using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public int CourseNumber { get; set; }

        public ICollection<AnnualRecord> AnnualRecords { get; set; } = new List<AnnualRecord>();
    }

}
