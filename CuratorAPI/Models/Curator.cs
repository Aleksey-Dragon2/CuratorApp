using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Models
{
    public class Curator
    {
        
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Number { get; set; }
        public Group? Group { get; set; }
    }
}
