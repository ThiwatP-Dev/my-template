using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedToken>
{
    public void Configure(EntityTypeBuilder<BlacklistedToken> builder)
    {
        builder.ToTable("BlacklistedTokens");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Token)
               .HasMaxLength(4000);

        builder.HasIndex(x => x.Token)
               .IsUnique();
    }
}