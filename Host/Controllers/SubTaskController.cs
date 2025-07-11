using Microsoft.AspNetCore.Mvc;
using Model.Interfaces.Services;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host.Controllers;

[Route("api/todo-list/sub-task")]
[ApiController]
public class SubTaskController(ISubTaskService subTaskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllByTaskIdAsync(int taskId)
    {
        var subTasks = await subTaskService.getAllByTaskId(taskId);
        return Ok(subTasks);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(SubTaskModel subTask)
    {
        var addedSubTask = await subTaskService.AddAsync(subTask);
        return Ok(addedSubTask);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(SubTaskModel subTask)
    {
        var updatedSubTask = await subTaskService.UpdateAsync(subTask);
        return Ok(updatedSubTask);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deletedId = await subTaskService.DeleteAsync(id);
        if (deletedId.HasValue)
        {
            return Ok(deletedId.Value);
        }
        return NotFound();
    }
}
