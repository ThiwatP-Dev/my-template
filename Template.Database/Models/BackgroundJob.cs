namespace Template.Database.Models;

public class BackgroundJob
{
    public Guid Id { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime? EndedAt { get; set; }
}