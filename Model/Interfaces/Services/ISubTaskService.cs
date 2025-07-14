using Model.Dtos;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Services;

public interface ISubTaskService
{
    Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId);
    Task<SubTaskModel> AddAsync(SubTaskDto subTask);
    Task<SubTaskModel> UpdateAsync(SubTaskDto subTask);
    Task<int?> DeleteAsync(int id);
}
