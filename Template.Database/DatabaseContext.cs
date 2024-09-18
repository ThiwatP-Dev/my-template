using Microsoft.EntityFrameworkCore;
using Template.Database.Configurations;
using Template.Database.Models;

namespace Template.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Learner> Learners { get; set; }
    public DbSet<Instructor> instructors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new LearnerConfiguration());
        modelBuilder.ApplyConfiguration(new InstructorConfiguration());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=template;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;MultiSubnetFailover=True");
    }
}
