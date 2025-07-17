using Microsoft.EntityFrameworkCore;
using Model.Enums;
using Model.Models;
using Todo.Core.Data;
using Todo.Core.Repositories;

namespace Repository.Tests.Repositories;

[TestFixture]
public class TaskRepositoryTest
{
    private MysqlDbContext _context;
    private TaskRepository _taskRepository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<MysqlDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new MysqlDbContext(options);
        _taskRepository = new TaskRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    #region GetAllAsync Tests

    [Test]
    public async Task GetAllAsync_WithNoFilter_ReturnsAllTasks()
    {
        // Arrange
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsCompleted = false },
            new TaskModel { Id = 2, Title = "Task 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsCompleted = true }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetAllAsync(null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(t => t.Id == 1), Is.True);
        Assert.That(result.Any(t => t.Id == 2), Is.True);
    }

    [Test]
    public async Task GetAllAsync_WithCompletedFilter_ReturnsOnlyCompletedTasks()
    {
        // Arrange
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Incomplete Task", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1), IsCompleted = false },
            new TaskModel { Id = 2, Title = "Completed Task 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2), IsCompleted = true },
            new TaskModel { Id = 3, Title = "Completed Task 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(3), IsCompleted = true }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetAllAsync("true");

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.All(t => t.IsCompleted), Is.True);
        Assert.That(result.Any(t => t.Id == 2), Is.True);
        Assert.That(result.Any(t => t.Id == 3), Is.True);
    }

    [Test]
    public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Act
        var result = await _taskRepository.GetAllAsync(null);

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetAllAsync_WithSubTasks_ReturnsTasksWithSubTasks()
    {
        // Arrange
        var task = new TaskModel { Id = 1, Title = "Task with SubTasks", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) };
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        
        _context.Tasks.Add(task);
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetAllAsync(null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        var taskWithSubTasks = result.First();
        Assert.That(taskWithSubTasks.SubTasks.Count, Is.EqualTo(2));
        Assert.That(taskWithSubTasks.SubTasks.Any(st => st.Title == "SubTask 1"), Is.True);
        Assert.That(taskWithSubTasks.SubTasks.Any(st => st.Title == "SubTask 2"), Is.True);
    }

    #endregion

    #region GetByIdAsync Tests

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Medium,
            IsCompleted = false
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo("Test Task"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.Priority, Is.EqualTo(Priority.Medium));
        Assert.That(result.IsCompleted, Is.False);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.GetByIdAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_WithTaskHavingSubTasks_ReturnsTaskWithSubTasks()
    {
        // Arrange
        var task = new TaskModel { Id = 1, Title = "Task with SubTasks", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) };
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        
        _context.Tasks.Add(task);
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.SubTasks.Count, Is.EqualTo(2));
        Assert.That(result.SubTasks.Any(st => st.Title == "SubTask 1"), Is.True);
        Assert.That(result.SubTasks.Any(st => st.Title == "SubTask 2"), Is.True);
    }

    [Test]
    public async Task GetByIdAsync_WithZeroId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.GetByIdAsync(0);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByIdAsync_WithNegativeId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.GetByIdAsync(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    #endregion

    #region AddAsync Tests

    [Test]
    public async Task AddAsync_WithValidTask_ReturnsAddedTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "New Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        // Act
        var result = await _taskRepository.AddAsync(task);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Title, Is.EqualTo("New Task"));
        Assert.That(result.Description, Is.EqualTo("Test Description"));
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        Assert.That(result.IsCompleted, Is.False);

        // Verify it was saved to database
        var savedTask = await _context.Tasks.FindAsync(result.Id);
        Assert.That(savedTask, Is.Not.Null);
        Assert.That(savedTask.Title, Is.EqualTo("New Task"));
    }

    [Test]
    public async Task AddAsync_WithMinimalValidData_ReturnsAddedTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "T",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _taskRepository.AddAsync(task);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.GreaterThan(0));
        Assert.That(result.Title, Is.EqualTo("T"));
        Assert.That(result.Description, Is.Null);
        Assert.That(result.Priority, Is.EqualTo(Priority.Low));
        Assert.That(result.IsCompleted, Is.False);
    }

    [Test]
    public async Task AddAsync_WithNullDescription_ReturnsAddedTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test Task",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _taskRepository.AddAsync(task);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Description, Is.Null);
        Assert.That(result.Title, Is.EqualTo("Test Task"));
    }

    [Test]
    public async Task AddAsync_WithDifferentPriorities_AddsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var task = new TaskModel
            {
                Title = $"Task with {priority} priority",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var result = await _taskRepository.AddAsync(task);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(priority), $"Priority {priority} should be saved correctly");
        }
    }

    [Test]
    public async Task AddAsync_WithCompletedStatus_ReturnsAddedTask()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Completed Task",
            Description = "Completed task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var result = await _taskRepository.AddAsync(task);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.Title, Is.EqualTo("Completed Task"));
    }

    #endregion

    #region UpdateAsync Tests

    [Test]
    public async Task UpdateAsync_WithValidTask_ReturnsUpdatedTask()
    {
        // Arrange
        var originalTask = new TaskModel
        {
            Id = 1,
            Title = "Original Task",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            IsCompleted = false
        };
        _context.Tasks.Add(originalTask);
        await _context.SaveChangesAsync();

        var updateTask = new TaskModel
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(2),
            Priority = Priority.High,
            IsCompleted = true
        };

        // Act
        var result = await _taskRepository.UpdateAsync(updateTask);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Title, Is.EqualTo("Updated Task"));
        Assert.That(result.Description, Is.EqualTo("Updated Description"));
        Assert.That(result.Priority, Is.EqualTo(Priority.High));
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.UpdatedAt, Is.Not.Null);
        Assert.That(result.UpdatedAt.Value.Date, Is.EqualTo(DateTime.UtcNow.Date));
    }

    [Test]
    public async Task UpdateAsync_WithNonExistentTask_ThrowsException()
    {
        // Arrange
        var nonExistentTask = new TaskModel
        {
            Id = 999,
            Title = "Non-existent Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () => await _taskRepository.UpdateAsync(nonExistentTask));
        Assert.That(exception.Message, Is.EqualTo("Task with ID 999 not found."));
    }

    [Test]
    public async Task UpdateAsync_WithCompletedTask_UpdatesCorrectly()
    {
        // Arrange
        var originalTask = new TaskModel
        {
            Id = 1,
            Title = "Task to Complete",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = false
        };
        _context.Tasks.Add(originalTask);
        await _context.SaveChangesAsync();

        var updateTask = new TaskModel
        {
            Id = 1,
            Title = "Task to Complete",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var result = await _taskRepository.UpdateAsync(updateTask);

        // Assert
        Assert.That(result.IsCompleted, Is.True);
        Assert.That(result.UpdatedAt, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_WithDifferentPriority_UpdatesCorrectly()
    {
        // Arrange
        var originalTask = new TaskModel
        {
            Id = 1,
            Title = "Priority Change Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low
        };
        _context.Tasks.Add(originalTask);
        await _context.SaveChangesAsync();

        var updateTask = new TaskModel
        {
            Id = 1,
            Title = "Priority Change Task",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Urgent
        };

        // Act
        var result = await _taskRepository.UpdateAsync(updateTask);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(Priority.Urgent));
    }

    [Test]
    public async Task UpdateAsync_WithNullDescription_UpdatesCorrectly()
    {
        // Arrange
        var originalTask = new TaskModel
        {
            Id = 1,
            Title = "Task with Description",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.Tasks.Add(originalTask);
        await _context.SaveChangesAsync();

        var updateTask = new TaskModel
        {
            Id = 1,
            Title = "Task with Description",
            Description = null,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = await _taskRepository.UpdateAsync(updateTask);

        // Assert
        Assert.That(result.Description, Is.Null);
    }

    #endregion

    #region DeleteAsync Tests

    [Test]
    public async Task DeleteAsync_WithExistingId_ReturnsDeletedId()
    {
        // Arrange
        var task = new TaskModel
        {
            Id = 1,
            Title = "Task to Delete",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.DeleteAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(1));
        
        // Verify it was deleted from database
        var deletedTask = await _context.Tasks.FindAsync(1);
        Assert.That(deletedTask, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNonExistentId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.DeleteAsync(999);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithZeroId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.DeleteAsync(0);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithNegativeId_ReturnsNull()
    {
        // Act
        var result = await _taskRepository.DeleteAsync(-1);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task DeleteAsync_WithTaskHavingSubTasks_DeletesTaskAndSubTasks()
    {
        // Arrange
        var task = new TaskModel { Id = 1, Title = "Task with SubTasks", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) };
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = 1, Title = "SubTask 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        
        _context.Tasks.Add(task);
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.DeleteAsync(1);

        // Assert
        Assert.That(result, Is.EqualTo(1));
        
        // Verify task was deleted
        var deletedTask = await _context.Tasks.FindAsync(1);
        Assert.That(deletedTask, Is.Null);
        
        // Note: SubTasks might still exist depending on cascade delete configuration
        // This behavior depends on the EF configuration
    }

    #endregion

    #region GetForToday Tests

    [Test]
    public async Task GetForToday_WithTasksDueToday_ReturnsOnlyTodaysTasks()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task", CreatedAt = DateTime.UtcNow, DueDate = today },
            new TaskModel { Id = 2, Title = "Tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = today.AddDays(1) },
            new TaskModel { Id = 3, Title = "Yesterday Task", CreatedAt = DateTime.UtcNow, DueDate = today.AddDays(-1) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForToday();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Title, Is.EqualTo("Today Task"));
        Assert.That(result.First().DueDate.Date, Is.EqualTo(today));
    }

    [Test]
    public async Task GetForToday_WithNoTasksDueToday_ReturnsEmpty()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = today.AddDays(1) },
            new TaskModel { Id = 2, Title = "Yesterday Task", CreatedAt = DateTime.UtcNow, DueDate = today.AddDays(-1) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForToday();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetForToday_WithMultipleTasksDueToday_ReturnsAllTodaysTasks()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task 1", CreatedAt = DateTime.UtcNow, DueDate = today },
            new TaskModel { Id = 2, Title = "Today Task 2", CreatedAt = DateTime.UtcNow, DueDate = today.AddHours(10) },
            new TaskModel { Id = 3, Title = "Today Task 3", CreatedAt = DateTime.UtcNow, DueDate = today.AddHours(23) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForToday();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(t => t.DueDate.Date == today), Is.True);
    }

    #endregion

    #region GetForTomorrow Tests

    [Test]
    public async Task GetForTomorrow_WithTasksDueTomorrow_ReturnsOnlyTomorrowsTasks()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task", CreatedAt = DateTime.UtcNow, DueDate = today },
            new TaskModel { Id = 2, Title = "Tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = tomorrow },
            new TaskModel { Id = 3, Title = "Day After Tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = tomorrow.AddDays(1) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForTomorrow();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.First().Title, Is.EqualTo("Tomorrow Task"));
        Assert.That(result.First().DueDate.Date, Is.EqualTo(tomorrow));
    }

    [Test]
    public async Task GetForTomorrow_WithNoTasksDueTomorrow_ReturnsEmpty()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Today Task", CreatedAt = DateTime.UtcNow, DueDate = today },
            new TaskModel { Id = 2, Title = "Day After tomorrow Task", CreatedAt = DateTime.UtcNow, DueDate = today.AddDays(2) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForTomorrow();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetForTomorrow_WithMultipleTasksDueTomorrow_ReturnsAllTomorrowsTasks()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Tomorrow Task 1", CreatedAt = DateTime.UtcNow, DueDate = tomorrow },
            new TaskModel { Id = 2, Title = "Tomorrow Task 2", CreatedAt = DateTime.UtcNow, DueDate = tomorrow.AddHours(12) },
            new TaskModel { Id = 3, Title = "Tomorrow Task 3", CreatedAt = DateTime.UtcNow, DueDate = tomorrow.AddHours(23) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForTomorrow();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.All(t => t.DueDate.Date == tomorrow), Is.True);
    }

    #endregion

    #region GetForThisWeek Tests

    [Test]
    public async Task GetForThisWeek_WithTasksInCurrentWeek_ReturnsOnlyThisWeeksTasks()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var startOfWeek = today.AddDays(-daysFromMonday);
        var endOfWeek = startOfWeek.AddDays(6);

        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "This Week Task 1", CreatedAt = DateTime.UtcNow, DueDate = startOfWeek },
            new TaskModel { Id = 2, Title = "This Week Task 2", CreatedAt = DateTime.UtcNow, DueDate = endOfWeek },
            new TaskModel { Id = 3, Title = "Last Week Task", CreatedAt = DateTime.UtcNow, DueDate = startOfWeek.AddDays(-1) },
            new TaskModel { Id = 4, Title = "Next Week Task", CreatedAt = DateTime.UtcNow, DueDate = endOfWeek.AddDays(1) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForThisWeek();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Any(t => t.Title == "This Week Task 1"), Is.True);
        Assert.That(result.Any(t => t.Title == "This Week Task 2"), Is.True);
        Assert.That(result.All(t => t.DueDate.Date >= startOfWeek && t.DueDate.Date <= endOfWeek), Is.True);
    }

    [Test]
    public async Task GetForThisWeek_WithNoTasksInCurrentWeek_ReturnsEmpty()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var startOfWeek = today.AddDays(-daysFromMonday);
        var endOfWeek = startOfWeek.AddDays(6);

        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Last Week Task", CreatedAt = DateTime.UtcNow, DueDate = startOfWeek.AddDays(-1) },
            new TaskModel { Id = 2, Title = "Next Week Task", CreatedAt = DateTime.UtcNow, DueDate = endOfWeek.AddDays(1) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForThisWeek();

        // Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetForThisWeek_WithTasksOnWeekBoundaries_ReturnsTasksIncludingBoundaries()
    {
        // Arrange
        var today = DateTime.UtcNow.Date;
        var daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var startOfWeek = today.AddDays(-daysFromMonday);
        var endOfWeek = startOfWeek.AddDays(6);

        var tasks = new List<TaskModel>
        {
            new TaskModel { Id = 1, Title = "Monday Task", CreatedAt = DateTime.UtcNow, DueDate = startOfWeek },
            new TaskModel { Id = 2, Title = "Sunday Task", CreatedAt = DateTime.UtcNow, DueDate = endOfWeek },
            new TaskModel { Id = 3, Title = "Wednesday Task", CreatedAt = DateTime.UtcNow, DueDate = startOfWeek.AddDays(2) }
        };
        _context.Tasks.AddRange(tasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetForThisWeek();

        // Assert
        Assert.That(result.Count(), Is.EqualTo(3));
        Assert.That(result.Any(t => t.Title == "Monday Task"), Is.True);
        Assert.That(result.Any(t => t.Title == "Sunday Task"), Is.True);
        Assert.That(result.Any(t => t.Title == "Wednesday Task"), Is.True);
    }

    #endregion

    #region Edge Cases and Integration Tests

    [Test]
    public async Task GetAllAsync_WithMixedTasksAndSubTasks_ReturnsCorrectStructure()
    {
        // Arrange
        var task1 = new TaskModel { Id = 1, Title = "Task 1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) };
        var task2 = new TaskModel { Id = 2, Title = "Task 2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) };
        
        var subTasks = new List<SubTaskModel>
        {
            new SubTaskModel { Id = 1, TaskId = 1, Title = "SubTask 1-1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 2, TaskId = 1, Title = "SubTask 1-2", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(1) },
            new SubTaskModel { Id = 3, TaskId = 2, Title = "SubTask 2-1", CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(2) }
        };
        
        _context.Tasks.AddRange(task1, task2);
        _context.SubTasks.AddRange(subTasks);
        await _context.SaveChangesAsync();

        // Act
        var result = await _taskRepository.GetAllAsync(null);

        // Assert
        Assert.That(result.Count(), Is.EqualTo(2));
        
        var firstTask = result.First(t => t.Id == 1);
        Assert.That(firstTask.SubTasks.Count, Is.EqualTo(2));
        Assert.That(firstTask.SubTasks.All(st => st.TaskId == 1), Is.True);
        
        var secondTask = result.First(t => t.Id == 2);
        Assert.That(secondTask.SubTasks.Count, Is.EqualTo(1));
        Assert.That(secondTask.SubTasks.All(st => st.TaskId == 2), Is.True);
    }

    [Test]
    public async Task AddAsync_UpdateAsync_DeleteAsync_FullLifecycle_WorksCorrectly()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Lifecycle Task",
            Description = "Testing full lifecycle",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = Priority.Low,
            IsCompleted = false
        };

        // Act & Assert - Add
        var addedTask = await _taskRepository.AddAsync(task);
        Assert.That(addedTask.Id, Is.GreaterThan(0));
        Assert.That(addedTask.Title, Is.EqualTo("Lifecycle Task"));

        // Act & Assert - Update
        addedTask.Title = "Updated Lifecycle Task";
        addedTask.Priority = Priority.High;
        addedTask.IsCompleted = true;
        
        var updatedTask = await _taskRepository.UpdateAsync(addedTask);
        Assert.That(updatedTask.Title, Is.EqualTo("Updated Lifecycle Task"));
        Assert.That(updatedTask.Priority, Is.EqualTo(Priority.High));
        Assert.That(updatedTask.IsCompleted, Is.True);
        Assert.That(updatedTask.UpdatedAt, Is.Not.Null);

        // Act & Assert - Delete
        var deletedId = await _taskRepository.DeleteAsync(addedTask.Id);
        Assert.That(deletedId, Is.EqualTo(addedTask.Id));

        // Verify deletion
        var deletedTask = await _taskRepository.GetByIdAsync(addedTask.Id);
        Assert.That(deletedTask, Is.Null);
    }

    [Test]
    public async Task MultipleOperations_WithConcurrentAccess_WorksCorrectly()
    {
        // Arrange
        var tasks = new List<TaskModel>();
        for (int i = 1; i <= 10; i++)
        {
            tasks.Add(new TaskModel
            {
                Title = $"Task {i}",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(i),
                Priority = (Priority)(i % 4),
                IsCompleted = i % 2 == 0
            });
        }

        // Act
        foreach (var task in tasks)
        {
            await _taskRepository.AddAsync(task);
        }

        // Assert
        var allTasks = await _taskRepository.GetAllAsync(null);
        Assert.That(allTasks.Count(), Is.EqualTo(10));

        var completedTasks = await _taskRepository.GetAllAsync("true");
        Assert.That(completedTasks.Count(), Is.EqualTo(5));
        Assert.That(completedTasks.All(t => t.IsCompleted), Is.True);
    }

    #endregion
}
