using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Interfaces.Repositories;
using Model.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<TaskModel> AddAsync(TaskModel task)
    {
        var addedTask = await taskRepository.AddAsync(task);
        return addedTask;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var deletedTask = await taskRepository.DeleteAsync(id);
        return deletedTask;
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync(TaskRange? range)
    {
        switch (range)
        {
            case TaskRange.Today:
                return await taskRepository.GetForToday();
            case TaskRange.Tomorrow:
                return await taskRepository.GetForTomorrow();
            case TaskRange.Week:
                return await taskRepository.GetForThisWeek();
            default:
                return await taskRepository.GetAllAsync();
        };
    }

    public Task<TaskModel> GetByIdAsync(int id)
    {
        var task = taskRepository.GetByIdAsync(id);
        return task;
    }

    public Task<TaskModel> UpdateAsync(TaskModel task)
    {
        var updatedTask = taskRepository.UpdateAsync(task);
        return updatedTask;
    }
}
