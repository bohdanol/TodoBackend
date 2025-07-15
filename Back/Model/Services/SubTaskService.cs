using AutoMapper;
using Model.Dtos;
using Model.Interfaces.Repositories;
using Model.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Services;

public class SubTaskService : ISubTaskService
{

    private readonly ISubTaskRepository _repository;
    private readonly IMapper _mapper;

    public SubTaskService(ISubTaskRepository subTaskRepository, IMapper mapper)
    {
        _repository = subTaskRepository;
        _mapper = mapper;
    }

    public async Task<SubTaskModel> AddAsync(SubTaskDto subTask)
    {
        var subTaskModel = _mapper.Map<SubTaskModel>(subTask);
        return await _repository.AddAsync(subTaskModel);
    }

    public async Task<int?> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId)
    {
        return await _repository.getAllByTaskId(taskId);
    }

    public async Task<SubTaskModel> UpdateAsync(SubTaskDto subTask)
    {
        var subTaskModel = _mapper.Map<SubTaskModel>(subTask);
        return await _repository.UpdateAsync(subTaskModel);
    }
}
