using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> builder)
    {
        builder.ToTable("DeviceTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Platform)
               .HasConversion<string>()
               .HasMaxLength(100);

        builder.Property(x => x.Token)
               .HasMaxLength(500);

        builder.HasIndex(x => x.Token)
               .IsUnique();

        builder.HasIndex(x => new { x.UserId, x.IsActive });
    }
}