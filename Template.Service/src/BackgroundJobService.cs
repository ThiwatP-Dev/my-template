using Template.Database;
using Template.Database.Models;

namespace Template.Service.src;

public class BackgroundJobService(DatabaseContext dbContext)
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task CompleteJobAsync(Guid id)
    {
        var job = await _dbContext.BackgroundJobs.FindAsync(id);
        if (job is null)
        {
            return;
        }
        
        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        job.EndedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<Guid> RunBackgroundJobAsync()
    {
        var job = new BackgroundJob
        {
            StartedAt = DateTime.UtcNow
        };

        using var transaction = await _dbContext.Database.BeginTransactionAsync();

        await _dbContext.BackgroundJobs.AddAsync(job);

        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        await Task.Delay(5000); // Simulate work

        return job.Id;
    }
}