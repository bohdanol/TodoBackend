using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Repositories;

public interface ISubTaskRepository
{
    Task<IEnumerable<SubTaskModel>> getAllByTaskId(int taskId);
    Task<SubTaskModel> AddAsync(SubTaskModel subTask);
    Task<SubTaskModel> UpdateAsync(SubTaskModel subTask);
    Task<int?> DeleteAsync(int id);
}
