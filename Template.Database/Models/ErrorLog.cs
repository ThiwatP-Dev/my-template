namespace Template.Database.Models;

public class ErrorLog
{
    public long Id { get; set; }

    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    public string? StackTrace { get; set; }

    public required string RequestPath { get; set; }

    public required string RequestMethod { get; set; }

    public required string RequestHeaders { get; set; }

    public required string RequestBody { get; set; }

    public string? QueryParams { get; set; }

    public string? Message { get; set; }
}