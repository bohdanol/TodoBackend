using Host.Controllers;
using Microsoft.AspNetCore.Mvc;
using Model.Dtos;
using Model.Enums;
using Model.Interfaces.Services;
using Model.Models;
using Model.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Host.Tests.Controllers;

[TestFixture]
public class SubTaskControllerTest
{
    private ISubTaskService _subTaskService;
    private SubTaskController _controller;

    [SetUp]
    public void Setup()
    {
        _subTaskService = Substitute.For<ISubTaskService>();
        _controller = new SubTaskController(_subTaskService);
    }

    #region GetAllByTaskIdAsync Tests

    [Test]
    public async Task GetAllByTaskIdAsync_WithValidTaskId_ReturnsOkWithSubTasks()
    {
        // Arrange
        var taskId = 1;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "SubTask 1", IsCompleted = false, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow},
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "SubTask 2", IsCompleted = true, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow }
        };

        _subTaskService.getAllByTaskId(taskId).Returns(subTasks);

        // Act
        var result = await _controller.GetAllByTaskIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(subTasks));
        await _subTaskService.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskIdAsync_WithNonExistentTaskId_ReturnsOkWithEmptyList()
    {
        // Arrange
        var taskId = 999;
        var emptySubTasks = new List<SubTaskModel>();

        _subTaskService.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _controller.GetAllByTaskIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(emptySubTasks));
        await _subTaskService.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskIdAsync_WithZeroTaskId_ReturnsOkWithEmptyList()
    {
        // Arrange
        var taskId = 0;
        var emptySubTasks = new List<SubTaskModel>();

        _subTaskService.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _controller.GetAllByTaskIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(emptySubTasks));
        await _subTaskService.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskIdAsync_WithNegativeTaskId_ReturnsOkWithEmptyList()
    {
        // Arrange
        var taskId = -1;
        var emptySubTasks = new List<SubTaskModel>();

        _subTaskService.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _controller.GetAllByTaskIdAsync(taskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(emptySubTasks));
        await _subTaskService.Received(1).getAllByTaskId(taskId);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_WithValidSubTask_ReturnsOkWithAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "New SubTask",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = subTaskDto.Priority
        };

        _subTaskService.AddAsync(subTaskDto).Returns(addedSubTask);

        // Act
        var result = await _controller.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(addedSubTask));
        await _subTaskService.Received(1).AddAsync(subTaskDto);
    }

    [Test]
    public async Task AddAsync_WithMinimalValidData_ReturnsOkWithAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "T",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };

        _subTaskService.AddAsync(subTaskDto).Returns(addedSubTask);

        // Act
        var result = await _controller.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(addedSubTask));
        await _subTaskService.Received(1).AddAsync(subTaskDto);
    }

    [Test]
    public async Task AddAsync_WithNullDescription_ReturnsOkWithAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Test SubTask",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = null,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };

        _subTaskService.AddAsync(subTaskDto).Returns(addedSubTask);

        // Act
        var result = await _controller.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(addedSubTask));
        await _subTaskService.Received(1).AddAsync(subTaskDto);
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidSubTask_ReturnsOkWithUpdatedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Updated SubTask",
            Description = "Updated Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(2),
            Priority = Priority.Low,
            IsCompleted = true
        };

        var updatedSubTask = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = subTaskDto.Priority,
            IsCompleted = subTaskDto.IsCompleted,
            UpdatedAt = DateTime.UtcNow
        };

        _subTaskService.UpdateAsync(subTaskDto).Returns(updatedSubTask);

        // Act
        var result = await _controller.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(updatedSubTask));
        await _subTaskService.Received(1).UpdateAsync(subTaskDto);
    }

    [Test]
    public async Task UpdateAsync_WithCompletedSubTask_ReturnsOkWithUpdatedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Completed SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        var updatedSubTask = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        _subTaskService.UpdateAsync(subTaskDto).Returns(updatedSubTask);

        // Act
        var result = await _controller.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(updatedSubTask));
        await _subTaskService.Received(1).UpdateAsync(subTaskDto);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentPriority_ReturnsOkWithUpdatedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "High Priority SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High
        };

        var updatedSubTask = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = Priority.High
        };

        _subTaskService.UpdateAsync(subTaskDto).Returns(updatedSubTask);

        // Act
        var result = await _controller.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(updatedSubTask));
        await _subTaskService.Received(1).UpdateAsync(subTaskDto);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsOkWithDeletedId()
    {
        // Arrange
        var subTaskId = 1;
        _subTaskService.DeleteAsync(subTaskId).Returns(subTaskId);

        // Act
        var result = await _controller.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(subTaskId));
        await _subTaskService.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var subTaskId = 999;
        _subTaskService.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _controller.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        await _subTaskService.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithZeroId_ReturnsNotFound()
    {
        // Arrange
        var subTaskId = 0;
        _subTaskService.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _controller.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        await _subTaskService.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithNegativeId_ReturnsNotFound()
    {
        // Arrange
        var subTaskId = -1;
        _subTaskService.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _controller.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundResult>());
        await _subTaskService.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithLargeId_ReturnsOkWhenExists()
    {
        // Arrange
        var subTaskId = int.MaxValue;
        _subTaskService.DeleteAsync(subTaskId).Returns(subTaskId);

        // Act
        var result = await _controller.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(subTaskId));
        await _subTaskService.Received(1).DeleteAsync(subTaskId);
    }

    #endregion

    #region Exception Handling Tests

    [Test]
    public void GetAllByTaskIdAsync_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var taskId = 1;
        _subTaskService.getAllByTaskId(taskId).Returns(Task.FromException<IEnumerable<SubTaskModel>>(new Exception("Service error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.GetAllByTaskIdAsync(taskId));
    }

    [Test]
    public void AddAsync_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Test SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _subTaskService.AddAsync(subTaskDto).Returns(Task.FromException<SubTaskModel>(new Exception("Service error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.AddAsync(subTaskDto));
    }

    [Test]
    public void UpdateAsync_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Test SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _subTaskService.UpdateAsync(subTaskDto).Returns(Task.FromException<SubTaskModel>(new Exception("Service error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.UpdateAsync(subTaskDto));
    }

    [Test]
    public void DeleteAsync_WhenServiceThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskId = 1;
        _subTaskService.DeleteAsync(subTaskId).Returns(Task.FromException<int?>(new Exception("Service error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _controller.DeleteAsync(subTaskId));
    }

    #endregion
}