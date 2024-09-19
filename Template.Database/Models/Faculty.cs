using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Database.Models;

[Table("Faculties")]
public class Faculty
{
    public Guid Id { get; set; }

    public required string Code { get; set; }

    public required string Name { get; set; }

    public string? Logo { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}