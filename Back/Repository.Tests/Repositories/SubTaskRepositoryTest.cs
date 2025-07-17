using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todo.Core.Data;
using Repository.Repositories;

namespace Repository.Tests.Repositories;

[TestFixture]
public class SubTaskRepositoryTest
{
    private MysqlDbContext _context;
    private SubTaskRepository _subTaskRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MysqlDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MysqlDbContext(options);
        _subTaskRepository = new SubTaskRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    #region getAllByTaskId Tests

    [Test]
    public async Task GetAllByTaskId_WithValidTaskId_ReturnsSubTasks()
    {
        // Arrange
        var taskId = 1;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsCompleted = false },
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsCompleted = true }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(taskId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(st => st.Id == 1), Is.True);
        Assert.That(result.Any(st => st.Id == 2), Is.True);
        Assert.That(result.All(st => st.TaskId == taskId), Is.True);
    }

    [Test]
    public async Task GetAllByTaskId_WithNonExistentTaskId_ReturnsEmptyList()
    {
        // Arrange
        var existingTaskId = 1;
        var nonExistentTaskId = 999;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = existingTaskId, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(nonExistentTaskId);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllByTaskId_WithZeroTaskId_ReturnsEmptyList()
    {
        // Arrange
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(0);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllByTaskId_WithNegativeTaskId_ReturnsEmptyList()
    {
        // Arrange
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(-1);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllByTaskId_WithMultipleTasksAndSubTasks_ReturnsOnlyMatchingTaskSubTasks()
    {
        // Arrange
        var task1Id = 1;
        var task2Id = 2;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = task1Id, Title = "Task 1 - SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = task1Id, Title = "Task 1 - SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) },
            new SubTaskModel { Id = 3, TaskId = task2Id, Title = "Task 2 - SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) },
            new SubTaskModel { Id = 4, TaskId = task2Id, Title = "Task 2 - SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(4) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(task1Id);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(st => st.TaskId == task1Id), Is.True);
        Assert.That(result.Any(st => st.Title == "Task 1 - SubTask 1"), Is.True);
        Assert.That(result.Any(st => st.Title == "Task 1 - SubTask 2"), Is.True);
    }

    [Test]
    public async Task GetAllByTaskId_WithDifferentPriorities_ReturnsAllSubTasks()
    {
        // Arrange
        var taskId = 1;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "Low Priority", Priority = Priority.Low, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "Medium Priority", Priority = Priority.Medium, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) },
            new SubTaskModel { Id = 3, TaskId = taskId, Title = "High Priority", Priority = Priority.High, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) },
            new SubTaskModel { Id = 4, TaskId = taskId, Title = "Urgent Priority", Priority = Priority.Urgent, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(4) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(taskId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(4));
        Assert.That(result.Any(st => st.Priority == Priority.Low), Is.True);
        Assert.That(result.Any(st => st.Priority == Priority.Medium), Is.True);
        Assert.That(result.Any(st => st.Priority == Priority.High), Is.True);
        Assert.That(result.Any(st => st.Priority == Priority.Urgent), Is.True);
    }

    [Test]
    public async Task GetAllByTaskId_WithCompletedAndIncompleteSubTasks_ReturnsAllSubTasks()
    {
        // Arrange
        var taskId = 1;
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = taskId, Title = "Incomplete SubTask", IsCompleted = false, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = taskId, Title = "Completed SubTask", IsCompleted = true, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.getAllByTaskId(taskId);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(st => st.IsCompleted == false), Is.True);
        Assert.That(result.Any(st => st.IsCompleted == true), Is.True);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_WithValidSubTask_ReturnsAddedSubTask()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "New SubTask",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        // Act
        var result = await _subTaskRepository.AddAsync(subTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.TaskId, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo("New SubTask"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        Assert.That(result.IsCompleted, Is.False);

        // Verify it was saved to database
        var savedSubTask = await _context.SubTasks.FindAsync(result.Id);
        Assert.That(savedSubTask, Is.Not.Null);
        Assert.That(savedSubTask.Title, Is.EqualTo("New SubTask"));
    }

    [Test]
    public async Task AddAsync_WithMinimalValidData_ReturnsAddedSubTask()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "T",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _subTaskRepository.AddAsync(subTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.TaskId, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo("T"));
        Assert.That(result.Description, Is.Null);
        Assert.That(result.Priority, Is.EqualTo(Priority.Low));
        Assert.That(result.IsCompleted, Is.False);
    }

    [Test]
    public async Task AddAsync_WithNullDescription_ReturnsAddedSubTask()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Test SubTask",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _subTaskRepository.AddAsync(subTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.Null);
        Assert.That(result.Title, Is.EqualTo("Test SubTask"));
    }

    [Test]
    public async Task AddAsync_WithDifferentPriorities_AddsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var subTask = new SubTaskModel
            {
                TaskId = 1,
                Title = $"SubTask with {priority} priority",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var result = await _subTaskRepository.AddAsync(subTask);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(priority), $"Priority {priority} should be saved correctly");
        }
    }

    [Test]
    public async Task AddAsync_WithCompletedStatus_ReturnsAddedSubTask()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Completed SubTask",
            Description = "Completed subtask description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var result = await _subTaskRepository.AddAsync(subTask);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.Title, Is.EqualTo("Completed SubTask"));
    }

    [Test]
    public async Task AddAsync_WithDifferentTaskIds_AddsCorrectly()
    {
        // Arrange
        var taskIds = new[] { 1, 100, int.MaxValue };

        foreach (var taskId in taskIds)
        {
            var subTask = new SubTaskModel
            {
                TaskId = taskId,
                Title = $"SubTask for Task {taskId}",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = await _subTaskRepository.AddAsync(subTask);

            // Assert
            Assert.That(result.TaskId, Is.EqualTo(taskId), $"TaskId {taskId} should be saved correctly");
        }
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidSubTask_ReturnsUpdatedSubTask()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Original SubTask",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            IsCompleted = false
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var updateSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Updated SubTask",
            Description = "Updated Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(2),
            Priority = Priority.High,
            IsCompleted = true
        };

        // Act
        var result = await _subTaskRepository.UpdateAsync(updateSubTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo("Updated SubTask"));
        Assert.That(result.Description, Is.EqualTo("Updated Description"));
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.UpdatedAt, Is.Not.Null);
        Assert.That(result.UpdatedAt.Value.Date, Is.EqualTo(DateTime.UtcNow.Date));
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentSubTask_ThrowsException()
    {
        // Arrange
        var nonExistentSubTask = new SubTaskModel
        {
            Id = 999,
            TaskId = 1,
            Title = "Non-existent SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () => await _subTaskRepository.UpdateAsync(nonExistentSubTask));
        Assert.That(exception.Message, Is.EqualTo("Task with ID 999 not found."));
    }

    [Test]
    public async Task UpdateAsync_WithCompletedSubTask_UpdatesCorrectly()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask to Complete",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = false
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var updateSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask to Complete",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var result = await _subTaskRepository.UpdateAsync(updateSubTask);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentPriority_UpdatesCorrectly()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Priority Change SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var updateSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Priority Change SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Urgent
        };

        // Act
        var result = await _subTaskRepository.UpdateAsync(updateSubTask);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(Priority.Urgent));
    }

    [Test]
    public async Task UpdateAsync_WithNullDescription_UpdatesCorrectly()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask with Description",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var updateSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask with Description",
            Description = null,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _subTaskRepository.UpdateAsync(updateSubTask);

        // Assert
        Assert.That(result.Description, Is.Null);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentTaskId_UpdatesCorrectly()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask to Move",
            Description = "SubTask moving to different task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var updateSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 2,
            Title = "SubTask to Move",
            Description = "SubTask moving to different task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _subTaskRepository.UpdateAsync(updateSubTask);

        // Assert
        // Note: The update method doesn't actually update TaskId in the current implementation
        // This test verifies the current behavior - TaskId is not updated
        Assert.That(result.TaskId, Is.EqualTo(1)); // Original TaskId remains unchanged
        Assert.That(result.Title, Is.EqualTo("SubTask to Move"));
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsDeletedId()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "SubTask to Delete",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.DeleteAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(1));
        
        // Verify it was deleted from database
        var deletedSubTask = await _context.SubTasks.FindAsync(1);
        Assert.That(deletedSubTask, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _subTaskRepository.DeleteAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithZeroId_ReturnsNull()
    {
        // Act
        var result = await _subTaskRepository.DeleteAsync(0);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNegativeId_ReturnsNull()
    {
        // Act
        var result = await _subTaskRepository.DeleteAsync(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithLargeId_ReturnsIdWhenExists()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            Id = int.MaxValue,
            TaskId = 1,
            Title = "SubTask with Large ID",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.DeleteAsync(int.MaxValue);

        // Assert
        Assert.That(result, Is.EqualTo(int.MaxValue));
        
        var deletedSubTask = await _context.SubTasks.FindAsync(int.MaxValue);
        Assert.That(deletedSubTask, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithOneOfMultipleSubTasks_DeletesOnlySpecifiedSubTask()
    {
        // Arrange
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) },
            new SubTaskModel { Id = 3, TaskId = 1, Title = "SubTask 3", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) }
        };
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _subTaskRepository.DeleteAsync(2);

        // Assert
        Assert.That(result, Is.EqualTo(2));
        
        // Verify only the specified subtask was deleted
        var deletedSubTask = await _context.SubTasks.FindAsync(2);
        Assert.That(deletedSubTask, Is.Null);
        
        var remainingSubTasks = await _context.SubTasks.ToListAsync();
        Assert.That(remainingSubTasks.Count, Is.EqualTo(2));
        Assert.That(remainingSubTasks.Any(st => st.Id == 1), Is.True);
        Assert.That(remainingSubTasks.Any(st => st.Id == 3), Is.True);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Test]
    public async Task GetAllByTaskId_WithNoSubTasks_ReturnsEmptyList()
    {
        // Act
        var result = await _subTaskRepository.getAllByTaskId(1);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task AddAsync_UpdateAsync_DeleteAsync_FullLifecycle_WorksCorrectly()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Lifecycle SubTask",
            Description = "Testing full lifecycle",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            IsCompleted = false
        };

        // Act & Assert - Add
        var addedSubTask = await _subTaskRepository.AddAsync(subTask);
        Assert.That(addedSubTask.Id, Is.GreaterThan(0));
        Assert.That(addedSubTask.Title, Is.EqualTo("Lifecycle SubTask"));

        // Act & Assert - Update
        addedSubTask.Title = "Updated Lifecycle SubTask";
        addedSubTask.Priority = Priority.High;
        addedSubTask.IsCompleted = true;
        
        var updatedSubTask = await _subTaskRepository.UpdateAsync(addedSubTask);
        Assert.That(updatedSubTask.Title, Is.EqualTo("Updated Lifecycle SubTask"));
        Assert.That(updatedSubTask.Priority, Is.EqualTo(Priority.High));
        Assert.That(updatedSubTask.IsCompleted, Is.True);
        Assert.That(updatedSubTask.UpdatedAt, Is.Not.Null);

        // Act & Assert - Delete
        var deletedId = await _subTaskRepository.DeleteAsync(addedSubTask.Id);
        Assert.That(deletedId, Is.EqualTo(addedSubTask.Id));

        // Verify deletion
        var deletedSubTask = await _context.SubTasks.FindAsync(addedSubTask.Id);
        Assert.That(deletedSubTask, Is.Null);
    }

    [Test]
    public async Task MultipleOperations_WithMultipleTasksAndSubTasks_WorksCorrectly()
    {
        // Arrange
        var subTasks = new List<SubTaskModel>();
        for (int taskId = 1; taskId <= 3; taskId++)
        {
            for (int i = 1; i <= 3; i++)
            {
                subTasks.Add(new SubTaskModel
                {
                    TaskId = taskId,
                    Title = $"Task {taskId} - SubTask {i}",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(i),
                    Priority = (Priority)(i % 4),
                    IsCompleted = i % 2 == 0
                });
            }
        }

        // Act
        foreach (var subTask in subTasks)
        {
            await _subTaskRepository.AddAsync(subTask);
        }

        // Assert
        var task1SubTasks = await _subTaskRepository.getAllByTaskId(1);
        Assert.That(task1SubTasks.Count(), Is.EqualTo(3));

        var task2SubTasks = await _subTaskRepository.getAllByTaskId(2);
        Assert.That(task2SubTasks.Count(), Is.EqualTo(3));

        var task3SubTasks = await _subTaskRepository.getAllByTaskId(3);
        Assert.That(task3SubTasks.Count(), Is.EqualTo(3));

        // Verify each task's subtasks have correct TaskId
        Assert.That(task1SubTasks.All(st => st.TaskId == 1), Is.True);
        Assert.That(task2SubTasks.All(st => st.TaskId == 2), Is.True);
        Assert.That(task3SubTasks.All(st => st.TaskId == 3), Is.True);
    }

    [Test]
    public async Task GetAllByTaskId_AfterAddingAndDeletingSubTasks_ReturnsCorrectSubTasks()
    {
        // Arrange
        var taskId = 1;
        var subTask1 = new SubTaskModel { TaskId = taskId, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) };
        var subTask2 = new SubTaskModel { TaskId = taskId, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) };
        var subTask3 = new SubTaskModel { TaskId = taskId, Title = "SubTask 3", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3) };

        // Add all subtasks
        var added1 = await _subTaskRepository.AddAsync(subTask1);
        var added2 = await _subTaskRepository.AddAsync(subTask2);
        var added3 = await _subTaskRepository.AddAsync(subTask3);

        // Verify all were added
        var allSubTasks = await _subTaskRepository.getAllByTaskId(taskId);
        Assert.That(allSubTasks.Count(), Is.EqualTo(3));

        // Delete one subtask
        await _subTaskRepository.DeleteAsync(added2.Id);

        // Act
        var remainingSubTasks = await _subTaskRepository.getAllByTaskId(taskId);

        // Assert
        Assert.That(remainingSubTasks.Count(), Is.EqualTo(2));
        Assert.That(remainingSubTasks.Any(st => st.Title == "SubTask 1"), Is.True);
        Assert.That(remainingSubTasks.Any(st => st.Title == "SubTask 3"), Is.True);
        Assert.That(remainingSubTasks.Any(st => st.Title == "SubTask 2"), Is.False);
    }

    [Test]
    public async Task UpdateAsync_WithAllPriorities_UpdatesCorrectly()
    {
        // Arrange
        var originalSubTask = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Priority Test SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low
        };
        _context.SubTasks.Add(originalSubTask);
        await _context.SaveChangesAsync();

        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            // Arrange
            var updateSubTask = new SubTaskModel
            {
                Id = 1,
                TaskId = 1,
                Title = "Priority Test SubTask",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var result = await _subTaskRepository.UpdateAsync(updateSubTask);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(priority), $"Priority {priority} should be updated correctly");
        }
    }

    #endregion
}
