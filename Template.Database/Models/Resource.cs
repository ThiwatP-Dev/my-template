using Template.Database.Enums;

namespace Template.Database.Models;

public class Resource
{
    public required string Path { get; set; }

    public required string Name { get; set; }

    public DateTime Date { get; set; }

    public FileType Type { get; set; }

    public long Size { get; set; }

    public double? Duration { get; set; }
}