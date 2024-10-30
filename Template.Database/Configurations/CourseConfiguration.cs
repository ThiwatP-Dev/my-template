using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
               .HasMaxLength(500);
        
        builder.HasIndex(x => x.Code)
               .IsUnique();

        builder.Property(x => x.Name)
               .HasMaxLength(1000);

        builder.HasMany(x => x.Lecturers)
               .WithMany(x => x.Courses)
               .UsingEntity<CourseLecturer>(
                    l => l.HasOne(e => e.Lecturer).WithMany().HasForeignKey(e => e.LecturerId).OnDelete(DeleteBehavior.Restrict),
                    r => r.HasOne(e => e.Course).WithMany().HasForeignKey(e => e.CourseId).OnDelete(DeleteBehavior.Cascade)
               );
    }
}