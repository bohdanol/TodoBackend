using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Repositories;

public interface ITaskRepository
{
    Task<IEnumerable<TaskModel>> GetAllAsync(String? filter);
    Task<TaskModel> GetByIdAsync(int id);
    Task<TaskModel> AddAsync(TaskModel task);
    Task<TaskModel> UpdateAsync(TaskModel task);
    Task<IEnumerable<TaskModel>> GetForToday();
    Task<IEnumerable<TaskModel>> GetForTomorrow();
    Task<IEnumerable<TaskModel>> GetForThisWeek();
    Task<int?> DeleteAsync(int id);
}