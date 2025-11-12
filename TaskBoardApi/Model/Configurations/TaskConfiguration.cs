using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskBoardApi.Model.Entities;

namespace TaskBoardApi.Model.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> entity)
        {
            entity.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(t => t.Description)
                .HasMaxLength(255);

            entity.Property(t => t.Deadline)
                .IsRequired()
                .HasColumnType("datetime2");

            entity.Property(t => t.IsCompleted)
                .IsRequired();
        }
    }
}
