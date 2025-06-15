using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class DocumentTemplate
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string TemplatePath { get; set; } = null!;
    }

}
