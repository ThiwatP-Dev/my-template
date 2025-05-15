using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class InstituteConfiguration : IEntityTypeConfiguration<Institute>
{
    public void Configure(EntityTypeBuilder<Institute> builder)
    {
        builder.ToTable("Institutes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
               .HasMaxLength(1000);
        
        builder.OwnsOne(x => x.Resource, cb =>
        {
            cb.ToJson();
            cb.Property(p => p.Type)
              .HasConversion<string>()
              .HasMaxLength(100);
        });
    }
}