using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuratorApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Number { get; set; }
        public DateOnly Birthday { get; set; }
        public string? address { get; set; }
        public Group? Group { get; set; }

    }
}
