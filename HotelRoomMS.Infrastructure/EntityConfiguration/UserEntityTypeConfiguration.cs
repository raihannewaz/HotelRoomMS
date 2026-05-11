//using Common.CustomIdentity.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;


//namespace BlogAppManage.Infrastructure.EntityConfiguration
//{
//    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
//    {
//        public void Configure(EntityTypeBuilder<User> builder)
//        {
//            builder.ToTable("Users");
//            builder.HasKey(u => u.Id);
//            builder.Property(u => u.FirstName).HasMaxLength(100);
//            builder.Property(u => u.LastName).HasMaxLength(100);
//            builder.Property(u => u.Email).HasMaxLength(100);
//            builder.Property(u => u.UserName).IsRequired().HasMaxLength(50);
//            builder.Property(u => u.PhoneNumber).HasMaxLength(12);
//            builder.Property(u => u.PhoneNumberConfirmed);
//            builder.Property(u => u.EmailConfirmed);
//            builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
//            builder.Property(u => u.IsActive).HasDefaultValue(true);
//            // Common properties
//            builder.Property(u => u.Created);
//            builder.Property(u => u.Modified);
//        }
//    }
//}
