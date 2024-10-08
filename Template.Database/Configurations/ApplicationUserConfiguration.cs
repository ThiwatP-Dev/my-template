using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username)
               .HasMaxLength(500);

        builder.HasIndex(x => x.Username)
               .IsUnique(true);

        builder.Property(x => x.Role)
               .HasConversion<string>()
               .HasMaxLength(100);
    }
}