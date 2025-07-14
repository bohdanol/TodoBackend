using Microsoft.EntityFrameworkCore;
using Model.Dtos;
using Model.Enums;
using Model.Interfaces.Repositories;
using Model.Interfaces.Services;
using Model.Mappers;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services;

public class TaskService(ITaskRepository taskRepository) : ITaskService
{
    public async Task<TaskModel> AddAsync(TaskDto task)
    {
        var taskModel = TaskMapper.ToModel(task);
        var addedTask = await taskRepository.AddAsync(taskModel);
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

    public Task<TaskModel> UpdateAsync(TaskDto task)
    {
        var taskModel = TaskMapper.ToModel(task);
        return taskRepository.UpdateAsync(taskModel);
    }
}
