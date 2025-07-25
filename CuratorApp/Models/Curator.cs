﻿using System.ComponentModel.DataAnnotations;

namespace CuratorApp.Models
{
    public class Curator
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = null!;

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required]
        public int GroupId { get; set; }

        public Group Group { get; set; } = null!;
    }
}
