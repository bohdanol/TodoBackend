using Microsoft.EntityFrameworkCore;
using Model.Models;

namespace Todo.Core.Data
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options) { }
        public DbSet<Model.Models.TaskModel> Task { get; set; }
        public DbSet<SubTaskModel> SubTask { get; set; }

        public required string DbPath { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubTaskModel>()
                .HasOne(s => s.Task)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(s => s.TaskId);
        }
    }
}
