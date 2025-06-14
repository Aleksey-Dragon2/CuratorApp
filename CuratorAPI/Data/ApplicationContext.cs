using Microsoft.EntityFrameworkCore;
using CuratorAPI.Models;

namespace CuratorApp.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Curator> Curators { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<AnnualRecord> AnnualRecords { get; set; } = null!;
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;
        public DbSet<GeneratedReport> GeneratedReports { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }
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

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.Curator)
                .WithMany(c => c.RefreshTokens)
                .HasForeignKey(rt => rt.CuratorId);
        }
    }
}
