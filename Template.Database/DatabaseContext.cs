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
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Institute> Institutes { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<CourseLecturer> CourseLecturers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=THIWAT-ASUS\\SQLEXPRESS;Initial Catalog=template_db;Persist Security Info=False;User ID=sa;Password=mrtoJGHToMLBaZdibrNf3pKc;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }
    }
}
