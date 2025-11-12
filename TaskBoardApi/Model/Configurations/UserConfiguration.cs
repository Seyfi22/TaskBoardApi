using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoardApi.Model.Entities;

namespace TaskBoardApi.Model.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.Property(u => u.Fullname)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(u => u.Email).IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired()  
                .HasMaxLength(100);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>();

            entity.HasMany(u => u.Tasks)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
