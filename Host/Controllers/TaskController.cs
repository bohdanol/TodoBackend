using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPut]
    public async Task<IActionResult> UpdateTaskAsync(TaskModel task)
    {
        var updatedTask = await taskService.UpdateAsync(task);
        return Ok(updatedTask);
    }

    [HttpPost]
    public async Task<IActionResult> AddTaskAsync(TaskModel task)
    {
        var addedTask = await taskService.AddAsync(task);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = addedTask.Id }, addedTask);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync()
    {
        var tasks = await taskService.GetAllAsync(null);
        return Ok(tasks);
    }

    [HttpGet("all/current-week")]
    public async Task<IActionResult> GetAllForCurrentWeekAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Week);
        return Ok(tasks);
    }

    [HttpGet("all/today")]
    public async Task<IActionResult> GetAllForTodayAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Today);
        return Ok(tasks);
    }

    [HttpGet("all/tomorrow")]
    public async Task<IActionResult> GetAllForTomorrowAsync()
    {
        var tasks = await taskService.GetAllAsync(TaskRange.Tomorrow);
        return Ok(tasks);
    }
}
