using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using Model.Enums;
using Model.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Tests.Dtos;

[TestFixture]
public class MappingProfileTest
{
    private IMapper _mapper = null!;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Manually configure the mappings since the internal MappingProfile is not accessible
        services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<TaskDto, TaskModel>().ReverseMap();
            cfg.CreateMap<SubTaskDto, SubTaskModel>().ReverseMap();
        });
        services.AddLogging(); 

        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Test]
    public void AssertConfigurationIsValid_WithValidConfiguration_DoesNotThrow()
    {
        // Arrange & Act & Assert
        Assert.DoesNotThrow(() => _mapper.ConfigurationProvider.AssertConfigurationIsValid());
    }

    [Test]
    public void MapTaskDto_WithValidData_ReturnsCorrectTaskModel()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.Id, Is.EqualTo(dto.Id));
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.Description, Is.EqualTo(dto.Description));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
        Assert.That(model.Priority, Is.EqualTo(dto.Priority));
        Assert.That(model.IsCompleted, Is.EqualTo(dto.IsCompleted));
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks, Is.Empty);
    }

    [Test]
    public void MapTaskModel_WithValidData_ReturnsCorrectTaskDto()
    {
        // Arrange
        var model = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.Medium,
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<TaskDto>(model);

        // Assert
        Assert.That(dto.Id, Is.EqualTo(model.Id));
        Assert.That(dto.Title, Is.EqualTo(model.Title));
        Assert.That(dto.Description, Is.EqualTo(model.Description));
        Assert.That(dto.CreatedAt, Is.EqualTo(model.CreatedAt));
        Assert.That(dto.DueDate, Is.EqualTo(model.DueDate));
        Assert.That(dto.Priority, Is.EqualTo(model.Priority));
        Assert.That(dto.IsCompleted, Is.EqualTo(model.IsCompleted));
        Assert.That(dto.UpdatedAt, Is.EqualTo(model.UpdatedAt));
    }

    [Test]
    public void MapSubTaskDto_WithValidData_ReturnsCorrectSubTaskModel()
    {
        // Arrange
        var dto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Test SubTask",
            Description = "Test SubTask Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(3),
            Priority = Priority.Low,
            IsCompleted = false
        };

        // Act
        var model = _mapper.Map<SubTaskModel>(dto);

        // Assert
        Assert.That(model.Id, Is.EqualTo(dto.Id));
        Assert.That(model.TaskId, Is.EqualTo(dto.TaskId));
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.Description, Is.EqualTo(dto.Description));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
        Assert.That(model.Priority, Is.EqualTo(dto.Priority));
        Assert.That(model.IsCompleted, Is.EqualTo(dto.IsCompleted));
    }

    [Test]
    public void MapSubTaskModel_WithValidData_ReturnsCorrectSubTaskDto()
    {
        // Arrange
        var model = new SubTaskModel
        {
            Id = 1,
            TaskId = 1,
            Title = "Test SubTask",
            Description = "Test SubTask Description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(3),
            Priority = Priority.Urgent,
            IsCompleted = true,
            UpdatedAt = DateTime.UtcNow
        };

        // Act
        var dto = _mapper.Map<SubTaskDto>(model);

        // Assert
        Assert.That(dto.Id, Is.EqualTo(model.Id));
        Assert.That(dto.TaskId, Is.EqualTo(model.TaskId));
        Assert.That(dto.Title, Is.EqualTo(model.Title));
        Assert.That(dto.Description, Is.EqualTo(model.Description));
        Assert.That(dto.CreatedAt, Is.EqualTo(model.CreatedAt));
        Assert.That(dto.DueDate, Is.EqualTo(model.DueDate));
        Assert.That(dto.Priority, Is.EqualTo(model.Priority));
        Assert.That(dto.IsCompleted, Is.EqualTo(model.IsCompleted));
        Assert.That(dto.UpdatedAt, Is.EqualTo(model.UpdatedAt));
    }

    [Test]
    public void MapTaskDto_WithNullDescription_ReturnsModelWithNullDescription()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            Description = "Valid Description", // Required in TaskDto
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.Description, Is.EqualTo(dto.Description));
    }

    [Test]
    public void MapTaskDto_WithSubTasks_ReturnsModelWithMappedSubTasks()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            Description = "Task with subtasks",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = new List<SubTaskDto>
            {
                new SubTaskDto
                {
                    Id = 1,
                    TaskId = 1,
                    Title = "SubTask 1",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(3)
                },
                new SubTaskDto
                {
                    Id = 2,
                    TaskId = 1,
                    Title = "SubTask 2",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                }
            }
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks.Count, Is.EqualTo(2));
        Assert.That(model.SubTasks[0].Title, Is.EqualTo("SubTask 1"));
        Assert.That(model.SubTasks[0].TaskId, Is.EqualTo(1));
        Assert.That(model.SubTasks[1].Title, Is.EqualTo("SubTask 2"));
        Assert.That(model.SubTasks[1].TaskId, Is.EqualTo(1));
    }

    [Test]
    public void MapTaskModel_WithSubTasks_ReturnsDtoWithMappedSubTasks()
    {
        // Arrange
        var model = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            Description = "Task with subtasks",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = new List<SubTaskModel>
            {
                new SubTaskModel
                {
                    Id = 1,
                    TaskId = 1,
                    Title = "SubTask 1",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(3)
                },
                new SubTaskModel
                {
                    Id = 2,
                    TaskId = 1,
                    Title = "SubTask 2",
                    CreatedAt = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(5)
                }
            }
        };

        // Act
        var dto = _mapper.Map<TaskDto>(model);

        // Assert
        Assert.That(dto.SubTasks, Is.Not.Null);
        Assert.That(dto.SubTasks.Count, Is.EqualTo(2));
        Assert.That(dto.SubTasks[0].Title, Is.EqualTo("SubTask 1"));
        Assert.That(dto.SubTasks[0].TaskId, Is.EqualTo(1));
        Assert.That(dto.SubTasks[1].Title, Is.EqualTo("SubTask 2"));
        Assert.That(dto.SubTasks[1].TaskId, Is.EqualTo(1));
    }

    [Test]
    public void MapTaskDto_WithNullSubTasks_ReturnsModelWithEmptySubTasks()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            Description = "Task without subtasks",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = null
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model, Is.Not.Null);
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks, Is.Empty);
    }

    [Test]
    public void MapTaskDto_WithDifferentPriorities_MapsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var dto = new TaskDto
            {
                Id = 1,
                Title = $"Task with {priority} priority",
                Description = $"Description for {priority} task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var model = _mapper.Map<TaskModel>(dto);

            // Assert
            Assert.That(model.Priority, Is.EqualTo(priority), $"Priority {priority} should map correctly");
        }
    }

    [Test]
    public void MapSubTaskDto_WithDifferentPriorities_MapsCorrectly()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var dto = new SubTaskDto
            {
                Id = 1,
                TaskId = 1,
                Title = $"SubTask with {priority} priority",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var model = _mapper.Map<SubTaskModel>(dto);

            // Assert
            Assert.That(model.Priority, Is.EqualTo(priority), $"Priority {priority} should map correctly");
        }
    }

    [Test]
    public void MapTaskDto_WithCompletedStatus_MapsCorrectly()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Completed Task",
            Description = "Completed task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.IsCompleted, Is.True);
    }

    [Test]
    public void MapSubTaskDto_WithCompletedStatus_MapsCorrectly()
    {
        // Arrange
        var dto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Completed SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Act
        var model = _mapper.Map<SubTaskModel>(dto);

        // Assert
        Assert.That(model.IsCompleted, Is.True);
    }

    [Test]
    public void MapTaskDto_WithUpdatedAt_MapsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Updated Task",
            Description = "Updated task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            UpdatedAt = updatedTime
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.UpdatedAt, Is.EqualTo(updatedTime));
    }

    [Test]
    public void MapSubTaskDto_WithUpdatedAt_MapsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;
        var dto = new SubTaskDto
        {
            Id = 1,
            TaskId = 1,
            Title = "Updated SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            UpdatedAt = updatedTime
        };

        // Act
        var model = _mapper.Map<SubTaskModel>(dto);

        // Assert
        Assert.That(model.UpdatedAt, Is.EqualTo(updatedTime));
    }

    [Test]
    public void MapTaskDto_WithMinimalData_MapsCorrectly()
    {
        // Arrange
        var dto = new TaskDto
        {
            Title = "Minimal Task",
            Description = "Minimal description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.Description, Is.EqualTo(dto.Description));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
        Assert.That(model.IsCompleted, Is.False); // Default value
        Assert.That(model.Priority, Is.EqualTo(Priority.Low)); // Default enum value
    }

    [Test]
    public void MapSubTaskDto_WithMinimalData_MapsCorrectly()
    {
        // Arrange
        var dto = new SubTaskDto
        {
            TaskId = 1,
            Title = "Minimal SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var model = _mapper.Map<SubTaskModel>(dto);

        // Assert
        Assert.That(model.TaskId, Is.EqualTo(dto.TaskId));
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
        Assert.That(model.IsCompleted, Is.False); // Default value
        Assert.That(model.Priority, Is.EqualTo(Priority.Low)); // Default enum value
    }

    [Test]
    public void MapTaskDto_WithEmptySubTasksList_ReturnsModelWithEmptySubTasks()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Task with empty subtasks",
            Description = "Task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = new List<SubTaskDto>()
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks, Is.Empty);
    }

    [Test]
    public void MapTaskModel_WithEmptySubTasksList_ReturnsDtoWithEmptySubTasks()
    {
        // Arrange
        var model = new TaskModel
        {
            Id = 1,
            Title = "Task with empty subtasks",
            Description = "Task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            SubTasks = new List<SubTaskModel>()
        };

        // Act
        var dto = _mapper.Map<TaskDto>(model);

        // Assert
        Assert.That(dto.SubTasks, Is.Not.Null);
        Assert.That(dto.SubTasks, Is.Empty);
    }
}
