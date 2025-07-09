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

        public async Task AddAsync(TaskModel task)
        {
            await _context.TaskModel.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.TaskModel.FindAsync(id);
            if (task != null) 
            { 
                _context.TaskModel.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            return await _context.TaskModel.ToListAsync();
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            return await _context.TaskModel.FindAsync(id);
        }

        public async Task UpdateAsync(TaskModel task)
        {
            var existingTask = await _context.TaskModel.FindAsync(task.Id);
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

        public void GetTasksForToday()
        {
            IQueryable<TaskModel> getQuery =
                from task in _context.TaskModel
                where task.DueDate.Date == DateTime.UtcNow.Date
                select task;
        }

        public void GetTasksForTomorrow()
        {
            IQueryable<TaskModel> getQuery =
                from task in _context.TaskModel
                where task.DueDate.Date == DateTime.UtcNow.AddDays(1).Date
                select task;
        }

        public static IQueryable<TaskModel> GetTasksForThisWeek()
        {
            var getQuery =
                from task in TaskModel
                where task.DueDate.Date >= DateTime.UtcNow.Date &&
                      task.DueDate.Date <= DateTime.UtcNow.AddDays(7).Date
                select task;
            return getQuery;
        }
    }
}
