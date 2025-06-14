using Microsoft.EntityFrameworkCore;
using CuratorAPI.Models;

namespace CuratorApp.Data
{
    public class ApplicationContext : DbContext
    {
        DbSet<Curator> Curators { get; set; } = null!;
        DbSet<Group> Groups { get; set; } = null!;
        DbSet<Student> Students { get; set; } = null!;
        DbSet<Subject> Subjects { get; set; } = null!;
        DbSet<AnnualRecord> AnnualRecords { get; set; } = null!;
        DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;
        DbSet<GeneratedReport> GeneratedReports { get; set; } = null!;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Curator>()
                .HasOne(c => c.Group)
                .WithMany() 
                .HasForeignKey(c => c.GroupId)
                .IsRequired();

            modelBuilder.Entity<Curator>()
                .HasIndex(c => c.GroupId)
                .IsUnique();
        }
    }
}
