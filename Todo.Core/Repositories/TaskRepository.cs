using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Data;

namespace Todo.Core.Repositories
{
    public class TaskRepository : ITaskRepository
    {

        private readonly TodoDbContext _context;

        public TaskRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Models.Task task)
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

        public async Task<IEnumerable<Models.Task>> GetAllAsync()
        {
            return await _context.Task.ToListAsync();
        }

        public async Task<Models.Task> GetByIdAsync(int id)
        {
            return await _context.Task.FindAsync(id);
        }

        public async Task UpdateAsync(Models.Task task)
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
