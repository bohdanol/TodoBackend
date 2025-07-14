using Microsoft.EntityFrameworkCore;
using Model.Dtos;
using Model.Interfaces.Repositories;
using Model.Models;
using Todo.Core.Data;

namespace Todo.Core.Repositories;

public class TaskRepository : ITaskRepository
{

    private readonly MysqlDbContext _context;

    public TaskRepository(MysqlDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync()
    {
        var tasks = GetTaskWitSubTasksQuery(_context);
        return await tasks.ToListAsync();
    }

    public async Task<TaskModel> GetByIdAsync(int id)
    {
        var taskWithSubtaskQuery = GetTaskWitSubTasksQuery(_context);

        return await taskWithSubtaskQuery.Where(t => t.Id == id).SingleOrDefaultAsync(); 
    }

    public async Task<TaskModel> AddAsync(TaskModel task)
    {
        var savedTask = await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
        return savedTask.Entity;
    }

    public async Task<TaskModel> UpdateAsync(TaskModel task)
    {
        var existingTask = await _context.Tasks.FindAsync(task.Id);
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
        return existingTask;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return task.Id;
        }
        return null;
    }

    public async Task<IEnumerable<TaskModel>> GetForToday()
    {
        var taskWithSubTaskQuery = GetTaskWitSubTasksQuery(_context);
        return await taskWithSubTaskQuery
                        .Where(task => task.DueDate.Date == DateTime.UtcNow.Date)
                        .ToListAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetForTomorrow()
    {
        var taskWithSubTaskQuery = GetTaskWitSubTasksQuery(_context);
        return await taskWithSubTaskQuery
                        .Where(task => task.DueDate.Date == DateTime.UtcNow.AddDays(1).Date)
                        .ToListAsync();
    }

    public async Task<IEnumerable<TaskModel>> GetForThisWeek()
    {
        var taskWithSubTaskQuery = GetTaskWitSubTasksQuery(_context);
        return await taskWithSubTaskQuery
                        .Where(task => task.DueDate.Date >= DateTime.UtcNow.Date &&
                                       task.DueDate.Date <= DateTime.UtcNow.AddDays(7).Date)
                        .ToListAsync();
    }

    static IQueryable<TaskModel> GetTaskWitSubTasksQuery(MysqlDbContext context)
    {
        return context.Tasks.Include(t => t.SubTasks);
    }
}
