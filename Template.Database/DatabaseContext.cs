using Microsoft.EntityFrameworkCore;
using Template.Database.Configurations;
using Template.Database.Models;
using Template.Database.Models.Localizations;

namespace Template.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext() { }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

    public required DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public required DbSet<Learner> Learners { get; set; }
    public required DbSet<Lecturer> Lecturers { get; set; }
    public required DbSet<Institute> Institutes { get; set; }
    public required DbSet<Course> Courses { get; set; }
    public required DbSet<CourseLecturer> CourseLecturers { get; set; }
    public required DbSet<LearningPath> LearningPaths { get; set; }
    public required DbSet<BlacklistedToken> BlacklistedTokens { get; set; }

    public required DbSet<InstituteLocalization> InstituteLocalizations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=template;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;MultiSubnetFailover=True",
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        }
    }
}
