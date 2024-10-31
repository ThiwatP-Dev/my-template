using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class LearningPathConfiguration : IEntityTypeConfiguration<LearningPath>
{
    public void Configure(EntityTypeBuilder<LearningPath> builder)
    {
        builder.ToTable("LearningPaths");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(1000);

        builder.OwnsOne(x => x.Properties, a =>
        {
            a.ToJson();
        });
    }
}