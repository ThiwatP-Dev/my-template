using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class CourseLecturerConfiguration : IEntityTypeConfiguration<CourseLecturer>
{
    public void Configure(EntityTypeBuilder<CourseLecturer> builder)
    {
        builder.ToTable("CourseLecturers");
        builder.HasKey(x => new { x.CourseId, x.LecturerId });
    }
}