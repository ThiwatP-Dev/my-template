using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class LearnerConfiguration : IEntityTypeConfiguration<Learner>
{
    public void Configure(EntityTypeBuilder<Learner> builder)
    {
        builder.Property(x => x.Code)
               .HasMaxLength(500);

        builder.Property(x => x.Email)
               .HasMaxLength(500);

        builder.Property(x => x.PhoneNumber)
               .HasMaxLength(100);

        builder.HasIndex(x => x.Code)
               .IsUnique(true);
        
        builder.HasIndex(x => x.Email)
               .IsUnique(true);
    }
}