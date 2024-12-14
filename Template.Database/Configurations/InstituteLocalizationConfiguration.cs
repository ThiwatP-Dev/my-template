using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models.Localizations;

namespace Template.Database.Configurations;

public class InstituteLocalizationConfiguration : IEntityTypeConfiguration<InstituteLocalization>
{
    public void Configure(EntityTypeBuilder<InstituteLocalization> builder)
    {
        builder.ToTable("Institutes", "localization");
        builder.HasKey(x => new { x.InstituteId, x.Language });

        builder.Property(x => x.Language)
               .HasConversion<string>()
               .HasMaxLength(100);
    }
}