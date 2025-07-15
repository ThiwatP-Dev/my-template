using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Template.Database.Configs;
using Template.Database.Configurations;
using Template.Database.Models;
using Template.Database.Models.Localizations;

namespace Template.Database;

public class DatabaseContext : DbContext
{
    private readonly string _encryptionKey;

    public DatabaseContext()
    {
        _encryptionKey = string.Empty;
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options,
                           IOptions<CryptoConfiguration> cryptoOptions) : base(options)
    {
        _encryptionKey = cryptoOptions.Value.SecretKey;
    }

    public required DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public required DbSet<Learner> Learners { get; set; }
    public required DbSet<Lecturer> Lecturers { get; set; }
    public required DbSet<Institute> Institutes { get; set; }
    public required DbSet<Course> Courses { get; set; }
    public required DbSet<CourseLecturer> CourseLecturers { get; set; }
    public required DbSet<LearningPath> LearningPaths { get; set; }
    public required DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
    public required DbSet<BackgroundJob> BackgroundJobs { get; set; }

    public required DbSet<InstituteLocalization> InstituteLocalizations { get; set; }
    public required DbSet<EmailLog> EmailLogs { get; set; }
    public required DbSet<Notification> Notifications { get; set; }
    public required DbSet<UserNotification> UserNotifications { get; set; }
    public required DbSet<DeviceToken> DeviceTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfiguration).Assembly);
        modelBuilder.ApplyConfiguration(new EncryptedRecordConfiguration(_encryptionKey));
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
