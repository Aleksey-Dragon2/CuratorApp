using Microsoft.EntityFrameworkCore;
using CuratorApp.Models;
using Microsoft.Extensions.Configuration;

namespace CuratorApp.Data
{
    public class ApplicationContext : DbContext
    {
        DbSet<Curator> Curators { get; set; } = null!;
        DbSet<Student> Students { get; set; } = null!;
        DbSet<Group> Groups { get; set; } = null!;
        public ApplicationContext() { }

        public ApplicationContext(DbContextOptionsBuilder optionsBuilder)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        }
    }
}
