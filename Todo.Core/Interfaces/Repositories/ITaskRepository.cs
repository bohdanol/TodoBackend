using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.Repositories;

public interface  ITaskRepository
{
    Task<Models.TaskModel> GetByIdAsync(int id);
    Task<IEnumerable<Models.TaskModel>> GetAllAsync();
    Task AddAsync(Models.TaskModel task);
    Task UpdateAsync(Models.TaskModel task);
    Task DeleteAsync(int id);
}