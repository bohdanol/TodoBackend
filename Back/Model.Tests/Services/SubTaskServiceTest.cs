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
public class SubTaskServiceTest
{
    private ISubTaskRepository _subTaskRepository;
    private IMapper _mapper;
    private SubTaskService _subTaskService;

    [SetUp]
    public void Setup()
    {
        _subTaskRepository = Substitute.For<ISubTaskRepository>();
        _mapper = Substitute.For<IMapper>();
        _subTaskService = new SubTaskService(_subTaskRepository, _mapper);
    }

    #region getAllByTaskId Tests

    [Test]
    public async Task GetAllByTaskId_WithValidTaskId_ReturnsSubTasks()
    {
        // Arrange
        var taskId = 1;
        var expectedSubTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        _subTaskRepository.getAllByTaskId(taskId).Returns(expectedSubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedSubTasks));
        Assert.That(result.Count(), Is.EqualTo(2));
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskId_WithNonExistentTaskId_ReturnsEmptyList()
    {
        // Arrange
        var taskId = 999;
        var emptySubTasks = new List<SubTaskModel>();
        _subTaskRepository.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.Empty);
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskId_WithZeroTaskId_ReturnsEmptyList()
    {
        // Arrange
        var taskId = 0;
        var emptySubTasks = new List<SubTaskModel>();
        _subTaskRepository.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.Empty);
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskId_WithNegativeTaskId_ReturnsEmptyList()
    {
        // Arrange
        var taskId = -1;
        var emptySubTasks = new List<SubTaskModel>();
        _subTaskRepository.getAllByTaskId(taskId).Returns(emptySubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.Empty);
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskId_WithLargeTaskId_ReturnsSubTasks()
    {
        // Arrange
        var taskId = int.MaxValue;
        var expectedSubTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "SubTask for large task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
        };
        _subTaskRepository.getAllByTaskId(taskId).Returns(expectedSubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedSubTasks));
        Assert.That(result.Count(), Is.EqualTo(1));
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    [Test]
    public async Task GetAllByTaskId_WithMultipleSubTasks_ReturnsAllSubTasks()
    {
        // Arrange
        var taskId = 1;
        var expectedSubTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "SubTask 1", Priority = Priority.High, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "SubTask 2", Priority = Priority.Medium, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) },
            new SubTaskModel { Id = 3, TaskId = taskId, Title = "SubTask 3", Priority = Priority.Low, IsCompleted = true, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) }
        };
        _subTaskRepository.getAllByTaskId(taskId).Returns(expectedSubTasks);

        // Act
        var result = await _subTaskService.getAllByTaskId(taskId);

        // Assert
        Assert.That(result, Is.EqualTo(expectedSubTasks));
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Any(st => st.IsCompleted), Is.True);
        await _subTaskRepository.Received(1).getAllByTaskId(taskId);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_WithValidSubTask_ReturnsAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "New SubTask",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = subTaskDto.Priority,
            IsCompleted = subTaskDto.IsCompleted
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = subTaskDto.Priority,
            IsCompleted = subTaskDto.IsCompleted
        };

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

        // Act
        var result = await _subTaskService.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedSubTask));
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.TaskId, Is.EqualTo(subTaskDto.TaskId));
        Assert.That(result.Title, Is.EqualTo(subTaskDto.Title));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).AddAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task AddAsync_WithMinimalValidData_ReturnsAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "T",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

        // Act
        var result = await _subTaskService.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedSubTask));
        Assert.That(result.TaskId, Is.EqualTo(subTaskDto.TaskId));
        Assert.That(result.Title, Is.EqualTo(subTaskDto.Title));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).AddAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task AddAsync_WithNullDescription_ReturnsAddedSubTask()
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

        var mappedSubTaskModel = new SubTaskModel
        {
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = null,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
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

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

        // Act
        var result = await _subTaskService.AddAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.EqualTo(addedSubTask));
        Assert.That(result.Description, Is.Null);
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).AddAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task AddAsync_WithDifferentPriorities_MapsAndAddsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var subTaskDto = new SubTaskDto
            {
                TaskId = 1,
                Title = $"SubTask with {priority} priority",
                Description = $"Description for {priority} subtask",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            var mappedSubTaskModel = new SubTaskModel
            {
                TaskId = subTaskDto.TaskId,
                Title = subTaskDto.Title,
                Description = subTaskDto.Description,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate,
                Priority = priority
            };

            var addedSubTask = new SubTaskModel
            {
                Id = 1,
                TaskId = subTaskDto.TaskId,
                Title = subTaskDto.Title,
                Description = subTaskDto.Description,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate,
                Priority = priority
            };

            _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
            _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

            // Act
            var result = await _subTaskService.AddAsync(subTaskDto);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(priority), $"Priority {priority} should be mapped correctly");
            _mapper.Received().Map<SubTaskModel>(subTaskDto);
            await _subTaskRepository.Received().AddAsync(mappedSubTaskModel);

            // Reset for next iteration
            _mapper.ClearReceivedCalls();
            _subTaskRepository.ClearReceivedCalls();
        }
    }

    [Test]
    public async Task AddAsync_WithCompletedStatus_ReturnsAddedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Completed SubTask",
            Description = "Completed subtask description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            IsCompleted = true
        };

        var addedSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            IsCompleted = true
        };

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

        // Act
        var result = await _subTaskService.AddAsync(subTaskDto);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).AddAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task AddAsync_WithDifferentTaskIds_MapsAndAddsCorrectly()
    {
        // Arrange
        var taskIds = new[] { 1, 100, int.MaxValue };

        foreach (var taskId in taskIds)
        {
            var subTaskDto = new SubTaskDto
            {
                TaskId = taskId,
                Title = $"SubTask for Task {taskId}",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            var mappedSubTaskModel = new SubTaskModel
            {
                TaskId = taskId,
                Title = subTaskDto.Title,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate
            };

            var addedSubTask = new SubTaskModel
            {
                Id = 1,
                TaskId = taskId,
                Title = subTaskDto.Title,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate
            };

            _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
            _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(addedSubTask);

            // Act
            var result = await _subTaskService.AddAsync(subTaskDto);

            // Assert
            Assert.That(result.TaskId, Is.EqualTo(taskId), $"TaskId {taskId} should be mapped correctly");
            _mapper.Received().Map<SubTaskModel>(subTaskDto);
            await _subTaskRepository.Received().AddAsync(mappedSubTaskModel);

            // Reset for next iteration
            _mapper.ClearReceivedCalls();
            _subTaskRepository.ClearReceivedCalls();
        }
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidSubTask_ReturnsUpdatedSubTask()
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
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            Priority = subTaskDto.Priority,
            IsCompleted = subTaskDto.IsCompleted,
            UpdatedAt = subTaskDto.UpdatedAt
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

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

        // Act
        var result = await _subTaskService.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result, Is.EqualTo(updatedSubTask));
        Assert.That(result.Id, Is.EqualTo(subTaskDto.Id));
        Assert.That(result.TaskId, Is.EqualTo(subTaskDto.TaskId));
        Assert.That(result.Title, Is.EqualTo(subTaskDto.Title));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).UpdateAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithCompletedSubTask_ReturnsUpdatedSubTask()
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

        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
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

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

        // Act
        var result = await _subTaskService.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).UpdateAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentPriority_ReturnsUpdatedSubTask()
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

        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
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

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

        // Act
        var result = await _subTaskService.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).UpdateAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithUpdatedAt_ReturnsUpdatedSubTask()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Updated SubTask",
            Description = "Updated subtask description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            UpdatedAt = updatedTime
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            UpdatedAt = updatedTime
        };

        var updatedSubTask = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            UpdatedAt = updatedTime
        };

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

        // Act
        var result = await _subTaskService.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result.UpdatedAt, Is.EqualTo(updatedTime));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).UpdateAsync(mappedSubTaskModel);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentTaskId_ReturnsUpdatedSubTask()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            TaskId = 2,
            Title = "Moved SubTask",
            Description = "SubTask moved to different task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = 2,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };

        var updatedSubTask = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = 2,
            Title = subTaskDto.Title,
            Description = subTaskDto.Description,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate,
            UpdatedAt = DateTime.UtcNow
        };

        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

        // Act
        var result = await _subTaskService.UpdateAsync(subTaskDto);

        // Assert
        Assert.That(result.TaskId, Is.EqualTo(2));
        _mapper.Received(1).Map<SubTaskModel>(subTaskDto);
        await _subTaskRepository.Received(1).UpdateAsync(mappedSubTaskModel);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsDeletedId()
    {
        // Arrange
        var subTaskId = 1;
        _subTaskRepository.DeleteAsync(subTaskId).Returns(subTaskId);

        // Act
        var result = await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.EqualTo(subTaskId));
        await _subTaskRepository.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var subTaskId = 999;
        _subTaskRepository.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.Null);
        await _subTaskRepository.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithZeroId_ReturnsNull()
    {
        // Arrange
        var subTaskId = 0;
        _subTaskRepository.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.Null);
        await _subTaskRepository.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithNegativeId_ReturnsNull()
    {
        // Arrange
        var subTaskId = -1;
        _subTaskRepository.DeleteAsync(subTaskId).Returns((int?)null);

        // Act
        var result = await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.Null);
        await _subTaskRepository.Received(1).DeleteAsync(subTaskId);
    }

    [Test]
    public async Task DeleteAsync_WithLargeId_ReturnsIdWhenExists()
    {
        // Arrange
        var subTaskId = int.MaxValue;
        _subTaskRepository.DeleteAsync(subTaskId).Returns(subTaskId);

        // Act
        var result = await _subTaskService.DeleteAsync(subTaskId);

        // Assert
        Assert.That(result, Is.EqualTo(subTaskId));
        await _subTaskRepository.Received(1).DeleteAsync(subTaskId);
    }

    #endregion

    #region Exception Handling Tests

    [Test]
    public void GetAllByTaskId_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var taskId = 1;
        _subTaskRepository.getAllByTaskId(taskId).Returns(Task.FromException<IEnumerable<SubTaskModel>>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.getAllByTaskId(taskId));
    }

    [Test]
    public void AddAsync_WhenMapperThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Test SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _mapper.When(x => x.Map<SubTaskModel>(subTaskDto)).Do(x => throw new Exception("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.AddAsync(subTaskDto));
    }

    [Test]
    public void AddAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Test SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        var mappedSubTaskModel = new SubTaskModel
        {
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };
        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.AddAsync(mappedSubTaskModel).Returns(Task.FromException<SubTaskModel>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.AddAsync(subTaskDto));
    }

    [Test]
    public void UpdateAsync_WhenMapperThrowsException_ThrowsException()
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
        _mapper.When(x => x.Map<SubTaskModel>(subTaskDto)).Do(x => throw new Exception("Mapper error"));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.UpdateAsync(subTaskDto));
    }

    [Test]
    public void UpdateAsync_WhenRepositoryThrowsException_ThrowsException()
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
        var mappedSubTaskModel = new SubTaskModel
        {
            Id = subTaskDto.Id,
            TaskId = subTaskDto.TaskId,
            Title = subTaskDto.Title,
            CreatedAt = subTaskDto.CreatedAt,
            DueDate = subTaskDto.DueDate
        };
        _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
        _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(Task.FromException<SubTaskModel>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.UpdateAsync(subTaskDto));
    }

    [Test]
    public void DeleteAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var subTaskId = 1;
        _subTaskRepository.DeleteAsync(subTaskId).Returns(Task.FromException<int?>(new Exception("Repository error")));

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _subTaskService.DeleteAsync(subTaskId));
    }

    #endregion

    #region Edge Cases and Additional Scenarios

    [Test]
    public async Task GetAllByTaskId_WithVariousTaskIds_CallsRepositoryCorrectly()
    {
        // Arrange
        var taskIds = new[] { 1, 50, 100, int.MaxValue };

        foreach (var taskId in taskIds)
        {
            var expectedSubTasks = new List<SubTaskModel>
            {
                new SubTaskModel { Id = 1, TaskId = taskId, Title = $"SubTask for {taskId}", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
            };
            _subTaskRepository.getAllByTaskId(taskId).Returns(expectedSubTasks);

            // Act
            var result = await _subTaskService.getAllByTaskId(taskId);

            // Assert
            Assert.That(result.First().TaskId, Is.EqualTo(taskId));
            await _subTaskRepository.Received().getAllByTaskId(taskId);

            // Reset for next iteration
            _subTaskRepository.ClearReceivedCalls();
        }
    }

    [Test]
    public async Task AddAsync_WithSubTaskFromDifferentPriorities_AllMappedCorrectly()
    {
        // Arrange
        var subTaskDtos = new[]
        {
            new SubTaskDto { TaskId = 1, Title = "Low Priority", Priority = Priority.Low, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskDto { TaskId = 1, Title = "Medium Priority", Priority = Priority.Medium, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskDto { TaskId = 1, Title = "High Priority", Priority = Priority.High, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskDto { TaskId = 1, Title = "Urgent Priority", Priority = Priority.Urgent, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
        };

        foreach (var dto in subTaskDtos)
        {
            var mappedModel = new SubTaskModel
            {
                TaskId = dto.TaskId,
                Title = dto.Title,
                Priority = dto.Priority,
                CreatedAt = dto.CreatedAt,
                DueDate = dto.DueDate
            };

            var addedModel = new SubTaskModel
            {
                Id = 1,
                TaskId = dto.TaskId,
                Title = dto.Title,
                Priority = dto.Priority,
                CreatedAt = dto.CreatedAt,
                DueDate = dto.DueDate
            };

            _mapper.Map<SubTaskModel>(dto).Returns(mappedModel);
            _subTaskRepository.AddAsync(mappedModel).Returns(addedModel);

            // Act
            var result = await _subTaskService.AddAsync(dto);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(dto.Priority));
            _mapper.Received().Map<SubTaskModel>(dto);
            await _subTaskRepository.Received().AddAsync(mappedModel);

            // Reset for next iteration
            _mapper.ClearReceivedCalls();
            _subTaskRepository.ClearReceivedCalls();
        }
    }

    [Test]
    public async Task UpdateAsync_WithStatusChanges_MapsCorrectly()
    {
        // Arrange
        var completionStatuses = new[] { true, false };

        foreach (var isCompleted in completionStatuses)
        {
            var subTaskDto = new SubTaskDto
            {
                Id = 1,
                TaskId = 1,
                Title = $"SubTask {(isCompleted ? "Completed" : "Incomplete")}",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(1),
                IsCompleted = isCompleted
            };

            var mappedSubTaskModel = new SubTaskModel
            {
                Id = subTaskDto.Id,
                TaskId = subTaskDto.TaskId,
                Title = subTaskDto.Title,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate,
                IsCompleted = isCompleted
            };

            var updatedSubTask = new SubTaskModel
            {
                Id = subTaskDto.Id,
                TaskId = subTaskDto.TaskId,
                Title = subTaskDto.Title,
                CreatedAt = subTaskDto.CreatedAt,
                DueDate = subTaskDto.DueDate,
                IsCompleted = isCompleted,
                UpdatedAt = DateTime.UtcNow
            };

            _mapper.Map<SubTaskModel>(subTaskDto).Returns(mappedSubTaskModel);
            _subTaskRepository.UpdateAsync(mappedSubTaskModel).Returns(updatedSubTask);

            // Act
            var result = await _subTaskService.UpdateAsync(subTaskDto);

            // Assert
            Assert.That(result.IsCompleted, Is.EqualTo(isCompleted));
            _mapper.Received().Map<SubTaskModel>(subTaskDto);
            await _subTaskRepository.Received().UpdateAsync(mappedSubTaskModel);

            // Reset for next iteration
            _mapper.ClearReceivedCalls();
            _subTaskRepository.ClearReceivedCalls();
        }
    }

    [Test]
    public async Task DeleteAsync_WithMultipleDeleteOperations_CallsRepositoryForEach()
    {
        // Arrange
        var subTaskIds = new[] { 1, 2, 3, 4, 5 };

        foreach (var id in subTaskIds)
        {
            _subTaskRepository.DeleteAsync(id).Returns(id);

            // Act
            var result = await _subTaskService.DeleteAsync(id);

            // Assert
            Assert.That(result, Is.EqualTo(id));
            await _subTaskRepository.Received().DeleteAsync(id);
        }

        // Verify all calls were made
        await _subTaskRepository.Received(subTaskIds.Length).DeleteAsync(Arg.Any<int>());
    }

    #endregion
}