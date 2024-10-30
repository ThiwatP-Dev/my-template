using System.ComponentModel.DataAnnotations.Schema;

namespace Template.Database.Models;

public class CourseLecturer
{
    public Guid CourseId { get; set; }

    public Guid LecturerId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey(nameof(LecturerId))]
    public virtual Lecturer Lecturer { get; set; } = null!;
}