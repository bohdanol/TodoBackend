using Microsoft.EntityFrameworkCore;
using Todo.Core.Models;

namespace Todo.Core.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
        public DbSet<Models.Task> Task { get; set; }
        public DbSet<Models.SubTask> SubTask { get; set; }

        public required string DbPath { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubTask>()
                .HasOne(s => s.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TaskId);
        }
    }
}
