using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Dtos;
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

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;

    public TaskService(ITaskRepository taskRepository, IMapper mapper)
    {
        _repository = taskRepository;
        _mapper = mapper;
    }

    public async Task<TaskModel> AddAsync(TaskDto task)
    {
        var taskModel = _mapper.Map<TaskModel>(task);
        var addedTask = await _repository.AddAsync(taskModel);
        return addedTask;
    }

    public async Task<int?> DeleteAsync(int id)
    {
        var deletedTask = await _repository.DeleteAsync(id);
        return deletedTask;
    }

    public async Task<IEnumerable<TaskModel>> GetAllAsync(TaskRange? range, String? isCompleted)
    {
        switch (range)
        {
            case TaskRange.Today:
                return await _repository.GetForToday();
            case TaskRange.Tomorrow:
                return await _repository.GetForTomorrow();
            case TaskRange.Week:
                return await _repository.GetForThisWeek();
            default:
                return await _repository.GetAllAsync(isCompleted);
        }
    }

    public Task<TaskModel> GetByIdAsync(int id)
    {
        var task = _repository.GetByIdAsync(id);
        return task;
    }

    public Task<TaskModel> UpdateAsync(TaskDto task)
    {
        var taskModel = _mapper.Map<TaskModel>(task);
        return _repository.UpdateAsync(taskModel);
    }
}
