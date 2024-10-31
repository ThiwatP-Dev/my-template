namespace Template.Database.Models;

public class LearningPath
{
    public Guid Id { get; set; }

    public required string Name { get; set; }
    
    public LearningPathProperties? Properties { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}

public class LearningPathProperties
{
    public string? Description { get; set; }

    public string? Remark { get; set; }
}