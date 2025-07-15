using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Template.Database.Models;

namespace Template.Database.Configurations;

public class UserNotificationConfiguration : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.ToTable("UserNotifications");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SentStatus)
               .HasConversion<string>()
               .HasMaxLength(100);

        builder.HasIndex(x => x.TargetUserId);
    }
}