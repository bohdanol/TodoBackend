using AutoMapper;
using Model.Dtos;
using Model.Enums;
using Model.Interfaces.Repositories;
using Model.Models;
using Model.Services;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Tests.Services;

[TestFixture]
public class TaskServiceTest
{
    private ITaskRepository _taskRepository;
    private IMapper _mapper;
    private TaskService _taskService;

    [SetUp]
    public void Setup()
    {
        _taskRepository = Substitute.For<ITaskRepository>();
        _mapper = Substitute.For<IMapper>();
        _taskService = new TaskService(_taskRepository, _mapper);
    }

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_WithNoRangeAndNoFilter_ReturnsAllTasks()
    {
        // Arrange
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new TaskModel { Id = 2, Title = "Task 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        _taskRepository.GetAllAsync(null).Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(null, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        Assert.That(result.Count(), Is.EqualTo(2));
        await _taskRepository.Received(1).GetAllAsync(null);
    }

    [Test]
    public async Task GetAllAsync_WithCompletedFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var isCompleted = "true";
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Completed Task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsCompleted = true }
        };
        _taskRepository.GetAllAsync(isCompleted).Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(null, isCompleted);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        Assert.That(result.Count(), Is.EqualTo(1));
        await _taskRepository.Received(1).GetAllAsync(isCompleted);
    }

    [Test]
    public async Task GetAllAsync_WithTodayRange_ReturnsTasksForToday()
    {
        // Arrange
        var range = TaskRange.Today;
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.Today.AddDays(1) }
        };
        _taskRepository.GetForToday().Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        Assert.That(result.Count(), Is.EqualTo(1));
        await _taskRepository.Received(1).GetForToday();
        await _taskRepository.DidNotReceive().GetAllAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GetAllAsync_WithTomorrowRange_ReturnsTasksForTomorrow()
    {
        // Arrange
        var range = TaskRange.Tomorrow;
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.Today.AddDays(2) }
        };
        _taskRepository.GetForTomorrow().Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        Assert.That(result.Count(), Is.EqualTo(1));
        await _taskRepository.Received(1).GetForTomorrow();
        await _taskRepository.DidNotReceive().GetAllAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GetAllAsync_WithWeekRange_ReturnsTasksForThisWeek()
    {
        // Arrange
        var range = TaskRange.Week;
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Week Task 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.Today.AddDays(3) },
            new TaskModel { Id = 2, Title = "Week Task 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.Today.AddDays(5) }
        };
        _taskRepository.GetForThisWeek().Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        Assert.That(result.Count(), Is.EqualTo(2));
        await _taskRepository.Received(1).GetForThisWeek();
        await _taskRepository.DidNotReceive().GetAllAsync(Arg.Any<string>());
    }

    [Test]
    public async Task GetAllAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var emptyTasks = new List<TaskModel>();
        _taskRepository.GetAllAsync(null).Returns(emptyTasks);

        // Act
        var result = await _taskService.GetAllAsync(null, null);

        // Assert
        Assert.That(result, Is.Empty);
        await _taskRepository.Received(1).GetAllAsync(null);
    }

    [Test]
    public async Task GetAllAsync_WithRangeAndFilter_UsesRangeAndIgnoresFilter()
    {
        // Arrange
        var range = TaskRange.Today;
        var isCompleted = "true";
        var expectedTasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.Today.AddDays(1) }
        };
        _taskRepository.GetForToday().Returns(expectedTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, isCompleted);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTasks));
        await _taskRepository.Received(1).GetForToday();
        await _taskRepository.DidNotReceive().GetAllAsync(Arg.Any<string>());
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        var taskId = 1;
        var expectedTask = new TaskModel
        {
            Id = taskId,
            Title = "Test Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Medium,
            IsCompleted = false
        };
        _taskRepository.GetByIdAsync(taskId).Returns(expectedTask);

        // Act
        var result = await _taskService.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedTask));
        Assert.That(result.Id, Is.EqualTo(taskId));
        await _taskRepository.Received(1).GetByIdAsync(taskId);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var taskId = 999;
        _taskRepository.GetByIdAsync(taskId).Returns((TaskModel)null);

        // Act
        var result = await _taskService.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).GetByIdAsync(taskId);
    }

    [Test]
    public async Task GetByIdAsync_WithZeroId_ReturnsNull()
    {
        // Arrange
        var taskId = 0;
        _taskRepository.GetByIdAsync(taskId).Returns((TaskModel)null);

        // Act
        var result = await _taskService.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).GetByIdAsync(taskId);
    }

    [Test]
    public async Task GetByIdAsync_WithNegativeId_ReturnsNull()
    {
        // Arrange
        var taskId = -1;
        _taskRepository.GetByIdAsync(taskId).Returns((TaskModel)null);

        // Act
        var result = await _taskService.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).GetByIdAsync(taskId);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_WithValidTask_ReturnsAddedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "New Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            IsCompleted = taskDto.IsCompleted
        };

        var addedTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            IsCompleted = taskDto.IsCompleted
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

        // Act
        var result = await _taskService.AddAsync(taskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedTask));
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo(taskDto.Title));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).AddAsync(mappedTaskModel);
    }

    [Test]
    public async Task AddAsync_WithMinimalValidData_ReturnsAddedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "T",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };

        var addedTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

        // Act
        var result = await _taskService.AddAsync(taskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedTask));
        Assert.That(result.Title, Is.EqualTo(taskDto.Title));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).AddAsync(mappedTaskModel);
    }

    [Test]
    public async Task AddAsync_WithNullDescription_ReturnsAddedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Test Task",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            Description = null,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };

        var addedTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            Description = null,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

        // Act
        var result = await _taskService.AddAsync(taskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedTask));
        Assert.That(result.Description, Is.Null);
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).AddAsync(mappedTaskModel);
    }

    [Test]
    public async Task AddAsync_WithDifferentPriorities_MapsAndAddsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var taskDto = new TaskDto
            {
                Title = $"Task with {priority} priority",
                Description = $"Description for {priority} task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            var mappedTaskModel = new TaskModel
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                CreatedAt = taskDto.CreatedAt,
                DueDate = taskDto.DueDate,
                Priority = priority
            };

            var addedTask = new TaskModel
            {
                Id = 1,
                Title = taskDto.Title,
                Description = taskDto.Description,
                CreatedAt = taskDto.CreatedAt,
                DueDate = taskDto.DueDate,
                Priority = priority
            };

            _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
            _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

            // Act
            var result = await _taskService.AddAsync(taskDto);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(priority), $"Priority {priority} should be mapped correctly");
            _mapper.Received().Map<TaskModel>(taskDto);
            await _taskRepository.Received().AddAsync(mappedTaskModel);

            // Reset for next iteration
            _mapper.ClearReceivedCalls();
            _taskRepository.ClearReceivedCalls();
        }
    }

    [Test]
    public async Task AddAsync_WithCompletedStatus_ReturnsAddedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Completed Task",
            Description = "Completed task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            IsCompleted = true
        };

        var addedTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            IsCompleted = true
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

        // Act
        var result = await _taskService.AddAsync(taskDto);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).AddAsync(mappedTaskModel);
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidTask_ReturnsUpdatedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(2),
            Priority = Priority.Low,
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        var mappedTaskModel = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            IsCompleted = taskDto.IsCompleted,
            UpdatedAt = taskDto.UpdatedAt
        };

        var updatedTask = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = taskDto.Priority,
            IsCompleted = taskDto.IsCompleted,
            UpdatedAt = DateTime.UtcNow
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.UpdateAsync(mappedTaskModel).Returns(updatedTask);

        // Act
        var result = await _taskService.UpdateAsync(taskDto);

        // Assert
        Assert.That(result, Is.EqualTo(updatedTask));
        Assert.That(result.Id, Is.EqualTo(taskDto.Id));
        Assert.That(result.Title, Is.EqualTo(taskDto.Title));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).UpdateAsync(mappedTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithCompletedTask_ReturnsUpdatedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Completed Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        var mappedTaskModel = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            IsCompleted = true
        };

        var updatedTask = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.UpdateAsync(mappedTaskModel).Returns(updatedTask);

        // Act
        var result = await _taskService.UpdateAsync(taskDto);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).UpdateAsync(mappedTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentPriority_ReturnsUpdatedTask()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "High Priority Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.High
        };

        var mappedTaskModel = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = Priority.High
        };

        var updatedTask = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            Priority = Priority.High
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.UpdateAsync(mappedTaskModel).Returns(updatedTask);

        // Act
        var result = await _taskService.UpdateAsync(taskDto);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).UpdateAsync(mappedTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithUpdatedAt_ReturnsUpdatedTask()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            UpdatedAt = updatedTime
        };

        var mappedTaskModel = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            UpdatedAt = updatedTime
        };

        var updatedTask = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            UpdatedAt = updatedTime
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.UpdateAsync(mappedTaskModel).Returns(updatedTask);

        // Act
        var result = await _taskService.UpdateAsync(taskDto);

        // Assert
        Assert.That(result.UpdatedAt, Is.EqualTo(updatedTime));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).UpdateAsync(mappedTaskModel);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsDeletedId()
    {
        // Arrange
        var taskId = 1;
        _taskRepository.DeleteAsync(taskId).Returns(taskId);

        // Act
        var result = await _taskService.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(taskId));
        await _taskRepository.Received(1).DeleteAsync(taskId);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var taskId = 999;
        _taskRepository.DeleteAsync(taskId).Returns((int?)null);

        // Act
        var result = await _taskService.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).DeleteAsync(taskId);
    }

    [Test]
    public async Task DeleteAsync_WithZeroId_ReturnsNull()
    {
        // Arrange
        var taskId = 0;
        _taskRepository.DeleteAsync(taskId).Returns((int?)null);

        // Act
        var result = await _taskService.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).DeleteAsync(taskId);
    }

    [Test]
    public async Task DeleteAsync_WithNegativeId_ReturnsNull()
    {
        // Arrange
        var taskId = -1;
        _taskRepository.DeleteAsync(taskId).Returns((int?)null);

        // Act
        var result = await _taskService.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
        await _taskRepository.Received(1).DeleteAsync(taskId);
    }

    [Test]
    public async Task DeleteAsync_WithLargeId_ReturnsIdWhenExists()
    {
        // Arrange
        var taskId = int.MaxValue;
        _taskRepository.DeleteAsync(taskId).Returns(taskId);

        // Act
        var result = await _taskService.DeleteAsync(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(taskId));
        await _taskRepository.Received(1).DeleteAsync(taskId);
    }

    #endregion

    #region Exception Handling Tests

    [Test]
    public void GetAllAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        _taskRepository.GetAllAsync(null).Returns(Task.FromException<IEnumerable<TaskModel>>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.GetAllAsync(null, null));
    }

    [Test]
    public void GetAllAsync_WithTodayRange_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        _taskRepository.GetForToday().Returns(Task.FromException<IEnumerable<TaskModel>>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.GetAllAsync(TaskRange.Today, null));
    }

    [Test]
    public void GetByIdAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var taskId = 1;
        _taskRepository.GetByIdAsync(taskId).Returns(Task.FromException<TaskModel>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.GetByIdAsync(taskId));
    }

    [Test]
    public void AddAsync_WhenMapperThrowsException_ThrowsException()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _mapper.When(x => x.Map<TaskModel>(taskDto)).Do(x => throw new Exception("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.AddAsync(taskDto));
    }

    [Test]
    public void AddAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };
        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(Task.FromException<TaskModel>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.AddAsync(taskDto));
    }

    [Test]
    public void UpdateAsync_WhenMapperThrowsException_ThrowsException()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _mapper.When(x => x.Map<TaskModel>(taskDto)).Do(x => throw new Exception("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.UpdateAsync(taskDto));
    }

    [Test]
    public void UpdateAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        var mappedTaskModel = new TaskModel
        {
            Id = taskDto.Id,
            Title = taskDto.Title,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate
        };
        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.UpdateAsync(mappedTaskModel).Returns(Task.FromException<TaskModel>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.UpdateAsync(taskDto));
    }

    [Test]
    public void DeleteAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var taskId = 1;
        _taskRepository.DeleteAsync(taskId).Returns(Task.FromException<int?>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _taskService.DeleteAsync(taskId));
    }

    #endregion

    #region Edge Cases and Additional Scenarios

    [Test]
    public async Task GetAllAsync_WithTodayRangeAndEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var range = TaskRange.Today;
        var emptyTasks = new List<TaskModel>();
        _taskRepository.GetForToday().Returns(emptyTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.Empty);
        await _taskRepository.Received(1).GetForToday();
    }

    [Test]
    public async Task GetAllAsync_WithTomorrowRangeAndEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var range = TaskRange.Tomorrow;
        var emptyTasks = new List<TaskModel>();
        _taskRepository.GetForTomorrow().Returns(emptyTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.Empty);
        await _taskRepository.Received(1).GetForTomorrow();
    }

    [Test]
    public async Task GetAllAsync_WithWeekRangeAndEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var range = TaskRange.Week;
        var emptyTasks = new List<TaskModel>();
        _taskRepository.GetForThisWeek().Returns(emptyTasks);

        // Act
        var result = await _taskService.GetAllAsync(range, null);

        // Assert
        Assert.That(result, Is.Empty);
        await _taskRepository.Received(1).GetForThisWeek();
    }

    [Test]
    public async Task AddAsync_WithTaskContainingSubTasks_ReturnsAddedTaskWithSubTasks()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Task with SubTasks",
            Description = "Task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = new List<SubTaskDto>
            {
                new SubTaskDto { TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) },
                new SubTaskDto { TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(5) }
            }
        };

        var mappedTaskModel = new TaskModel
        {
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            SubTasks = new List<SubTaskModel>
            {
                new SubTaskModel { TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) },
                new SubTaskModel { TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(5) }
            }
        };

        var addedTask = new TaskModel
        {
            Id = 1,
            Title = taskDto.Title,
            Description = taskDto.Description,
            CreatedAt = taskDto.CreatedAt,
            DueDate = taskDto.DueDate,
            SubTasks = mappedTaskModel.SubTasks
        };

        _mapper.Map<TaskModel>(taskDto).Returns(mappedTaskModel);
        _taskRepository.AddAsync(mappedTaskModel).Returns(addedTask);

        // Act
        var result = await _taskService.AddAsync(taskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedTask));
        Assert.That(result.SubTasks, Is.Not.Null);
        Assert.That(result.SubTasks.Count, Is.EqualTo(2));
        _mapper.Received(1).Map<TaskModel>(taskDto);
        await _taskRepository.Received(1).AddAsync(mappedTaskModel);
    }

    #endregion
}