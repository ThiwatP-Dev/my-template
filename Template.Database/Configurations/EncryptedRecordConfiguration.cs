using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;
using Template.Utility.Providers;

namespace Template.Database.Configurations;

public class EncryptedRecordConfiguration(string encryptionKey) : IEntityTypeConfiguration<EncryptedRecord>
{
    private readonly string _encryptionKey = encryptionKey;

    public void Configure(EntityTypeBuilder<EncryptedRecord> builder)
    {
        builder.ToTable("EncryptedRecords");
        builder.HasKey(x => x.Id);

        var converter = new EncryptedStringConverter(_encryptionKey);

        builder.Property(x => x.SensitiveData)
            .HasConversion(converter);
    }
}