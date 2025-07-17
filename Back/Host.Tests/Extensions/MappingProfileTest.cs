using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Model.Dtos;
using Model.Models;
using NUnit.Framework;

namespace Host.Tests.Extensions;

[TestFixture]
public class MappingProfileTest
{
    private IMapper _mapper = null!;
    
    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
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
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.Id, Is.EqualTo(dto.Id));
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks, Is.Empty);
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
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
        };

        // Act
        var model = _mapper.Map<SubTaskModel>(dto);

        // Assert
        Assert.That(model.Id, Is.EqualTo(dto.Id));
        Assert.That(model.Title, Is.EqualTo(dto.Title));
        Assert.That(model.TaskId, Is.EqualTo(dto.TaskId));
        Assert.That(model.CreatedAt, Is.EqualTo(dto.CreatedAt));
        Assert.That(model.DueDate, Is.EqualTo(dto.DueDate));
    }

    [Test]
    public void MapTaskModel_WithValidData_ReturnsCorrectTaskDto()
    {
        // Arrange
        var model = new TaskModel
        {
            Id = 1,
            Title = "Test Task",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
        };

        // Act
        var dto = _mapper.Map<TaskDto>(model);

        // Assert
        Assert.That(dto.Id, Is.EqualTo(model.Id));
        Assert.That(dto.Title, Is.EqualTo(model.Title));
        Assert.That(dto.CreatedAt, Is.EqualTo(model.CreatedAt));
        Assert.That(dto.DueDate, Is.EqualTo(model.DueDate));
    }

    [Test]
    public void MapTaskDto_WithNullSubTasks_ReturnsModelWithEmptySubTasks()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
            SubTasks = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
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
    public void MapTaskDto_WithSubTasks_ReturnsModelWithMappedSubTasks()
    {
        // Arrange
        var dto = new TaskDto
        {
            Id = 1,
            Title = "Test Task",
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
                    DueDate = DateTime.UtcNow.AddDays(3),
                }
            }
        };

        // Act
        var model = _mapper.Map<TaskModel>(dto);

        // Assert
        Assert.That(model.SubTasks, Is.Not.Null);
        Assert.That(model.SubTasks.Count, Is.EqualTo(1));
        Assert.That(model.SubTasks[0].Title, Is.EqualTo("SubTask 1"));
        Assert.That(model.SubTasks[0].TaskId, Is.EqualTo(1));
    }
}
