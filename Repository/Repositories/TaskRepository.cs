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

        public async Task<IEnumerable<TaskModel>> GetAllAsync()
        {
            return await _context.TaskModel.ToListAsync();
        }

        public async Task<TaskModel> GetByIdAsync(int id)
        {
            return await _context.TaskModel.FindAsync(id);
        }

        public async Task<TaskModel> AddAsync(TaskModel task)
        {
            var savedTask = await _context.TaskModel.AddAsync(task);
            await _context.SaveChangesAsync();
            return savedTask.Entity;
        }

        public async Task<TaskModel> UpdateAsync(TaskModel task)
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

            var updatedTask = await _context.SaveChangesAsync();
            return existingTask;
        }

        public async Task<int?> DeleteAsync(int id)
        {
            var task = await _context.TaskModel.FindAsync(id);
            if (task != null)
            {
                _context.TaskModel.Remove(task);
                await _context.SaveChangesAsync();
                return task.Id;
            }
            return null;
        }

        public static IQueryable<TaskModel> GetTasksForTodayQuery(MysqlDbContext context)
        {
            var getQuery =
                from task in context.TaskModel
                where task.DueDate.Date == DateTime.UtcNow.Date
                select task;
            return getQuery;
        }

        public static IQueryable<TaskModel> GetTasksForTomorrowQuery(MysqlDbContext context)
        {
            var getQuery =
                from task in context.TaskModel
                where task.DueDate.Date == DateTime.UtcNow.AddDays(1).Date
                select task;
            return getQuery;
        }

        public static IQueryable<TaskModel> GetTasksForThisWeekQuery(MysqlDbContext context)
        {
            var getQuery =
                from task in context.TaskModel
                where task.DueDate.Date >= DateTime.UtcNow.Date &&
                      task.DueDate.Date <= DateTime.UtcNow.AddDays(7).Date
                select task;
            return getQuery;
        }

        public async Task<IEnumerable<TaskModel>> GetForToday()
        {
            return await _context.TaskModel
                            .Where(task => task.DueDate.Date == DateTime.UtcNow.Date)
                            .ToListAsync();
        }

        public async Task<IEnumerable<TaskModel>> GetForTomorrow()
        {
            return await _context.TaskModel
                            .Where(task => task.DueDate.Date == DateTime.UtcNow.AddDays(1).Date)
                            .ToListAsync();
        }

        public async Task<IEnumerable<TaskModel>> GetForThisWeek()
        {
            return await _context.TaskModel
                            .Where(task => task.DueDate.Date >= DateTime.UtcNow.Date &&
                                           task.DueDate.Date <= DateTime.UtcNow.AddDays(7).Date)
                            .ToListAsync();
        }
    }
}
