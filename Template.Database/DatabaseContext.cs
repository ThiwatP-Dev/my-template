using Microsoft.EntityFrameworkCore;
using Template.Database.Configurations;
using Template.Database.Models;

namespace Template.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    // public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    // public DbSet<Learner> Learners { get; set; }
    // public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Institute> Institutes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        // modelBuilder.ApplyConfiguration(new LearnerConfiguration());
        // modelBuilder.ApplyConfiguration(new LecturerConfiguration());
        modelBuilder.ApplyConfiguration(new InstituteConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=THIWAT-ASUS\\SQLEXPRESS;Initial Catalog=plexus2;Persist Security Info=False;User ID=sa;Password=mrtoJGHToMLBaZdibrNf3pKc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
    }
}
