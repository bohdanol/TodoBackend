using Model.Dtos;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskModel>> GetAllAsync(TaskRange? range);
    Task<TaskModel> GetByIdAsync(int id);
    Task<TaskModel> AddAsync(TaskDto task);
    Task<TaskModel> UpdateAsync(TaskDto task);
    Task<int?> DeleteAsync(int id);
}
