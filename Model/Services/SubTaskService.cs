using Model.Interfaces.Repositories;
using Model.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services;

public class SubTaskService(ISubTaskRepository subTaskRepository) : ISubTaskService
{
    public async Task<SubTaskModel> AddAsync(SubTaskModel subTask)
    {
        return await subTaskRepository.AddAsync(subTask);
    }

    public async Task<int?> DeleteAsync(int id)
    {
        return await subTaskRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId)
    {
        return await subTaskRepository.getAllByTaskId(taskId);
    }

    public async Task<SubTaskModel> UpdateAsync(SubTaskModel subTask)
    {
        return await subTaskRepository.UpdateAsync(subTask);
    }
}
