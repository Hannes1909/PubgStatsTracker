using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace PubgStatsController.Sql.Models.Configuration
{
    public class UserModelConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(_user => _user.Id);

            builder.HasIndex(_user => _user.TwitchUserId).IsUnique();

            builder.Property(_user => _user.Id).IsRequired().HasColumnName("UserId").HasColumnType("BIGINT").ValueGeneratedOnAdd();
            builder.Property(_user => _user.TwitchUserId).IsRequired().HasColumnName("TwitchUserId").HasColumnType("VARCHAR(64)");
            builder.Property(_user => _user.Username).IsRequired().HasColumnType("Username").HasColumnType("VARCHAR(64)");
            builder.Property(_user => _user.DisplayName).IsRequired().HasColumnType("DisplayName").HasColumnType("VARCHAR(64)");
            builder.Property(_user => _user.ProfileImageUrl).IsRequired().HasColumnName("ProfileImageUrl").HasColumnType("VARCHAR(255)");
        }
    }
}
