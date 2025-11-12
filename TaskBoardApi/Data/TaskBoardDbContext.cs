using Microsoft.EntityFrameworkCore;
using TaskBoardApi.Model.Entities;

namespace TaskBoardApi.Data
{
    public class TaskBoardDbContext : DbContext
    {
        public TaskBoardDbContext(DbContextOptions<TaskBoardDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskBoardDbContext).Assembly);
        }


    }
}
