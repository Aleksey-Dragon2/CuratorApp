using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Models
{
    public class TemplateKeyword
    {
        public int Id { get; set; }

        public int DocumentTemplateId { get; set; }
        public DocumentTemplate DocumentTemplate { get; set; } = null!;

        [Required]
        public string Placeholder { get; set; } = null!;
    }

}
