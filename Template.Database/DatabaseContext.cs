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
    // public DbSet<Instructor> instructors { get; set; }
    public DbSet<Faculty> Faculties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        // modelBuilder.ApplyConfiguration(new LearnerConfiguration());
        // modelBuilder.ApplyConfiguration(new InstructorConfiguration());
        modelBuilder.ApplyConfiguration(new FacultyConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=THIWAT-ASUS\\SQLEXPRESS;Initial Catalog=plexus2;Persist Security Info=False;User ID=sa;Password=mrtoJGHToMLBaZdibrNf3pKc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
    }
}
