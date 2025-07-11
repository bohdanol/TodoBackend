using Microsoft.EntityFrameworkCore;
using Model.Interfaces.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Data;

namespace Repository.Repositories;

public class SubTaskRepository : ISubTaskRepository
{
    private readonly MysqlDbContext _context;

    public SubTaskRepository(MysqlDbContext context)
    {
        _context = context;
    }

    public async Task<SubTaskModel> AddAsync(SubTaskModel subTask)
    {
        var addedSubTask = await _context.SubTasks.AddAsync(subTask);
        await _context.SaveChangesAsync();
        return subTask;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var subTask = await _context.SubTasks.FindAsync(id);
        if (subTask != null)
        {
            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
            return subTask.Id;
        }
        return null;
    }

    public async Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId)
    {
        var subTasks = await _context.SubTasks
            .Where(s => s.TaskId == taskId)
            .ToListAsync();

        return subTasks;
    }

    public async Task<SubTaskModel> UpdateAsync(SubTaskModel subTask)
    {
        var existingSubTask = await _context.SubTasks.FindAsync(subTask.Id);
        if (existingSubTask == null)
        {
            throw new Exception($"Task with ID {subTask.Id} not found.");
        }

        existingSubTask.Title = subTask.Title;
        existingSubTask.Description = subTask.Description;
        existingSubTask.IsCompleted = subTask.IsCompleted;
        existingSubTask.Priority = subTask.Priority;
        existingSubTask.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        
        return existingSubTask;
    }
}
