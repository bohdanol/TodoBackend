using NUnit.Framework;
using NSubstitute;
using Microsoft.AspNetCore.Mvc;
using Model.Interfaces.Services;
using Model.Models;
using Model.Dtos;
using Model.Enums;
using Host.Controllers;

namespace Host.Tests.Controllers;

[TestFixture]
public class TaskControllerTests
{
    private ITaskService _taskService;
    private TaskController _controller;

    [SetUp]
    public void Setup()
    {
        _taskService = Substitute.For<ITaskService>();
        _controller = new TaskController(_taskService);
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var task = new TaskModel
        {
            Id = taskId,
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Medium,
            SubTasks = new List<SubTaskModel>()
        };
        _taskService.GetByIdAsync(taskId).Returns(task);

        // Act
        var result = await _controller.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(task));
    }

    [Test]
    public async Task GetByIdAsync_WithInvalidId_ReturnsBadResult()
    {
        // Arrange
        var taskId = 1;

        _taskService.GetByIdAsync(taskId).Returns((TaskModel?)null);

        // Act
        var result = await _controller.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetAllAsync_WithNoFilters_ReturnsOkResult()
    {
        // Arrange
        var tasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "Test Task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = Priority.Medium,
                SubTasks = new List<SubTaskModel>()
            }
        };
        _taskService.GetAllAsync(null, null).Returns(tasks);

        // Act
        var result = await _controller.GetAllAsync(null, null);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(tasks));
    }

    [Test]
    public async Task GetAllAsync_WithCompletedFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var completedTasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "Completed Task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                Priority = Priority.High,
                IsCompleted = true,
                SubTasks = new List<SubTaskModel>()
            }
        };
        _taskService.GetAllAsync(null, "true").Returns(completedTasks);

        // Act
        var result = await _controller.GetAllAsync("true", null);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(completedTasks));
        await _taskService.Received(1).GetAllAsync(null, "true");
    }

    [Test]
    public async Task AddTaskAsync_WithValidTask_ReturnsOkResult()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "New Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High
        };
        var createdTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            SubTasks = new List<SubTaskModel>()
        };
        _taskService.AddAsync(taskDto).Returns(createdTask);

        // Act
        var result = await _controller.AddTaskAsync(taskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(createdTask));
        await _taskService.Received(1).AddAsync(taskDto);
    }

    [Test]
    public async Task AddTaskAsync_WithInvalidTask_ReturnsBadRequest()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High
        };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.AddTaskAsync(taskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateTaskAsync_WithValidTask_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        var taskDto = new TaskDto
        {
            Id = taskId,
            Title = "Updated Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            UpdatedAt = DateTime.UtcNow
        };
        var updatedTask = new TaskModel
        {
            Id = taskId,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            UpdatedAt = taskDto.UpdatedAt,
            SubTasks = new List<SubTaskModel>()
        };
        _taskService.UpdateAsync(taskDto).Returns(updatedTask);

        // Act
        var result = await _controller.UpdateTaskAsync(taskId, taskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(updatedTask));
    }

    [Test]
    public async Task UpdateTaskAsync_WithMismatchedIds_ReturnsBadRequest()
    {
        // Arrange
        var taskId = 1;
        var taskDto = new TaskDto
        {
            Id = 2,
            Title = "Updated Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low
        };

        // Act
        var result = await _controller.UpdateTaskAsync(taskId, taskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task UpdateTaskAsync_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var taskId = 1;
        var taskDto = new TaskDto
        {
            Id = taskId,
            Title = "",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low
        };

        _controller.ModelState.AddModelError("Title", "Title is required");

        // Act
        var result = await _controller.UpdateTaskAsync(taskId, taskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task DeleteAsync_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var taskId = 1;
        _taskService.DeleteAsync(taskId).Returns(taskId);

        // Act
        var result = await _controller.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(taskId));
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var taskId = 999;
        _taskService.DeleteAsync(taskId).Returns((int?)null);

        // Act
        var result = await _controller.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetAllForTodayAsync_ReturnsOkResult()
    {
        // Arrange
        var todayTasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "Today Task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                Priority = Priority.High,
                SubTasks = new List<SubTaskModel>()
            }
        };
        _taskService.GetAllAsync(TaskRange.Today, null).Returns(todayTasks);

        // Act
        var result = await _controller.GetAllForTodayAsync();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(todayTasks));
        await _taskService.Received(1).GetAllAsync(TaskRange.Today, null);
    }

    [Test]
    public async Task GetAllForTomorrowAsync_ReturnsOkResult()
    {
        // Arrange
        var tomorrowTasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "Tomorrow Task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = Priority.Medium,
                SubTasks = new List<SubTaskModel>()
            }
        };
        _taskService.GetAllAsync(TaskRange.Tomorrow, null).Returns(tomorrowTasks);

        // Act
        var result = await _controller.GetAllForTomorrowAsync();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(tomorrowTasks));
        await _taskService.Received(1).GetAllAsync(TaskRange.Tomorrow, null);
    }

    [Test]
    public async Task GetAllForCurrentWeekAsync_ReturnsOkResult()
    {
        // Arrange
        var weekTasks = new List<TaskModel>
        {
            new TaskModel
            {
                Id = 1,
                Title = "Week Task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(3),
                Priority = Priority.Low,
                SubTasks = new List<SubTaskModel>()
            }
        };
        _taskService.GetAllAsync(TaskRange.Week, null).Returns(weekTasks);

        // Act
        var result = await _controller.GetAllForCurrentWeekAsync();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(weekTasks));
        await _taskService.Received(1).GetAllAsync(TaskRange.Week, null);
    }

    [TearDown]
    public void TearDown()
    {
        _controller = null;
    }
}