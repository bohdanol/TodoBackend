using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using Model.Enums;
using Model.Interfaces.Services;
using Model.Models;

namespace Host.Controllers;

[Route("api/todo-list/task")]
[ApiController]
public class TaskController(ITaskService taskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var task = await taskService.GetByIdAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskAsync(int id, TaskDto task)
    {

        Console.WriteLine(id);
        Console.WriteLine(task);
        if (task == null)
        {
            return BadRequest("Task data is required");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != task.Id)
        {
            return BadRequest("Route ID does not match task ID");
        }

        try
        {
            var updatedTask = await taskService.UpdateAsync(task);
            if (updatedTask == null)
            {
                return NotFound($"Task with ID {id} not found");
            }

            return Ok(updatedTask);
        }
        catch (Exception)
        {
            // Log the exception here
            return StatusCode(500, "An error occurred while updating the task");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskAsync(TaskDto task)
    {
        var addedTask = await taskService.AddAsync(task);
        return Ok(addedTask);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync([FromQuery] String? isCompleted)
    {
        var tasks = await taskService.GetAllAsync(null, isCompleted);
        return Ok(tasks);
    }

    [HttpGet("all/this-week")]
    public async Task<IActionResult> GetAllForCurrentWeekAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Week, null);
        return Ok(tasks);
    }

    [HttpGet("all/today")]
    public async Task<IActionResult> GetAllForTodayAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Today, null);
        return Ok(tasks);
    }

    [HttpGet("all/tomorrow")]
    public async Task<IActionResult> GetAllForTomorrowAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Tomorrow, null);
        return Ok(tasks);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deletedTaskId = await taskService.DeleteAsync(id);
        if (deletedTaskId == null)
        {
            return NotFound();
        }
        return Ok(deletedTaskId);
    }
}
