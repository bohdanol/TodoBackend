using Model.Dtos;
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

public class SubTaskService(ISubTaskRepository subTaskRepository) : ISubTaskService
{
    public async Task<SubTaskModel> AddAsync(SubTaskDto subTask)
    {
        var subTaskModel = SubTaskMapper.ToModel(subTask);
        return await subTaskRepository.AddAsync(subTaskModel);
    }

    public async Task<int?> DeleteAsync(int id)
    {
        return await subTaskRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId)
    {
        return await subTaskRepository.getAllByTaskId(taskId);
    }

    public async Task<SubTaskModel> UpdateAsync(SubTaskDto subTask)
    {
        var subTaskModel = SubTaskMapper.ToModel(subTask);
        return await subTaskRepository.UpdateAsync(subTaskModel);
    }
}
