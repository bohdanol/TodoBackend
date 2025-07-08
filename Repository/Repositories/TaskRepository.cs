using Microsoft.EntityFrameworkCore;
using Model.Interfaces.Repositories;
using Model.Models;
using Todo.Core.Data;

namespace Todo.Core.Repositories
{
    public class TaskRepository : ITaskRepository
    {

        private readonly MysqlDbContext _context;

        public TaskRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Model.Models.TaskModel task)
        {
            await _context.Task.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.Task.FindAsync(id);
            if (task != null) 
            { 
                _context.Task.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Model.Models.TaskModel>> GetAllAsync()
        {
            return await _context.Task.ToListAsync();
        }

        public async Task<Model.Models.TaskModel> GetByIdAsync(int id)
        {
            return await _context.Task.FindAsync(id);
        }

        public async Task UpdateAsync(Model.Models.TaskModel task)
        {
            var existingTask = await _context.Task.FindAsync(task.Id);
            if (existingTask == null)
            {
                throw new Exception($"Task with ID {task.Id} not found.");
            }

            existingTask.Title = task.Title;
            existingTask.Description = task.Description;
            existingTask.IsCompleted = task.IsCompleted;
            existingTask.Priority = task.Priority;
            existingTask.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
