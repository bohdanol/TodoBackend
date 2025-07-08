using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todo.Core.Repositories;

public interface  ITaskRepository
{
    Task<Models.Task> GetByIdAsync(int id);
    Task<IEnumerable<Models.Task>> GetAllAsync();
    Task AddAsync(Models.Task task);
    Task UpdateAsync(Models.Task task);
    Task DeleteAsync(int id);
}