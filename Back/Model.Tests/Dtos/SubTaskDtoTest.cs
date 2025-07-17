using Model.Dtos;
using Model.Enums;
using System.ComponentModel.DataAnnotations;

namespace Model.Tests.Dtos;

[TestFixture]
public class SubTaskDtoTest
{
    [Test]
    public void SubTaskDto_WithDefaultValues_ReturnsIsCompletedFalse()
    {
        // Arrange & Act
        var subTaskDto = new SubTaskDto
        {
            Title = "Test SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Assert
        Assert.IsFalse(subTaskDto.IsCompleted);
    }

    [Test]
    public void ValidateModel_WithValidData_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Id = 1,
            Title = "Valid SubTask Title",
            Description = "Valid subtask description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(3),
            Priority = Priority.Medium,
            IsCompleted = false,
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullTitle_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = null!,
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "",
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithTitleTooLong_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = new string('A', 251),
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthTitle_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = new string('A', 250),
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithEmptyDescription_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            Description = "",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithDescriptionTooLong_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            Description = new string('B', 501),
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthDescription_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            Description = new string('B', 500),
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullDescription_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            Description = null,
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithMinLengthDescription_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            Description = "A",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithZeroTaskId_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 0
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("TaskId")));
    }

    [Test]
    public void ValidateModel_WithNegativeTaskId_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = -1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("TaskId")));
    }

    [Test]
    public void ValidateModel_WithValidTaskId_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithMaxTaskId_PassesValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = int.MaxValue
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void SubTaskDto_WithDifferentPriorities_SetsCorrectly()
    {
        // Arrange & Act
        var urgentSubTaskDto = new SubTaskDto
        {
            Title = "Urgent SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddHours(2),
            TaskId = 1,
            Priority = Priority.Urgent
        };

        var lowSubTaskDto = new SubTaskDto
        {
            Title = "Low Priority SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(5),
            TaskId = 1,
            Priority = Priority.Low
        };

        // Assert
        Assert.AreEqual(Priority.Urgent, urgentSubTaskDto.Priority);
        Assert.AreEqual(Priority.Low, lowSubTaskDto.Priority);
    }

    [Test]
    public void SubTaskDto_WithCompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var completedSubTaskDto = new SubTaskDto
        {
            Title = "Completed SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            TaskId = 1,
            IsCompleted = true
        };

        // Assert
        Assert.IsTrue(completedSubTaskDto.IsCompleted);
    }

    [Test]
    public void SubTaskDto_WithUpdatedAt_SetsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;

        // Act
        var subTaskDto = new SubTaskDto
        {
            Title = "Updated SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1,
            UpdatedAt = updatedTime
        };

        // Assert
        Assert.AreEqual(updatedTime, subTaskDto.UpdatedAt);
    }

    [Test]
    public void SubTaskDto_WithIdProperty_SetsCorrectly()
    {
        // Arrange & Act
        var subTaskDto = new SubTaskDto
        {
            Id = 42,
            Title = "SubTask with ID",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1
        };

        // Assert
        Assert.AreEqual(42, subTaskDto.Id);
    }

    [Test]
    public void SubTaskDto_WithTaskIdProperty_SetsCorrectly()
    {
        // Arrange & Act
        var subTaskDto = new SubTaskDto
        {
            Title = "SubTask for Task 123",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 123
        };

        // Assert
        Assert.AreEqual(123, subTaskDto.TaskId);
    }

    [Test]
    public void ValidateModel_WithInvalidPriority_FailsValidation()
    {
        // Arrange
        var subTaskDto = new SubTaskDto
        {
            Title = "Valid Title",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            TaskId = 1,
            Priority = (Priority)999
        };

        // Act
        var results = ValidateModel(subTaskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Priority")));
    }

    [Test]
    public void ValidateModel_WithValidPriorities_PassesValidation()
    {
        // Arrange
        var priorities = new[] { Priority.Low, Priority.Medium, Priority.High, Priority.Urgent };

        foreach (var priority in priorities)
        {
            var subTaskDto = new SubTaskDto
            {
                Title = $"SubTask with {priority} priority",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                TaskId = 1,
                Priority = priority
            };

            // Act
            var results = ValidateModel(subTaskDto);

            // Assert
            Assert.IsEmpty(results, $"Validation should pass for Priority.{priority}");
        }
    }

    [Test]
    public void SubTaskDto_WithPastDueDate_SetsCorrectly()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var subTaskDto = new SubTaskDto
        {
            Title = "Overdue SubTask",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            DueDate = pastDate,
            TaskId = 1
        };

        // Assert
        Assert.AreEqual(pastDate, subTaskDto.DueDate);
        Assert.IsTrue(subTaskDto.DueDate < DateTime.UtcNow);
    }

    [Test]
    public void SubTaskDto_WithFutureDueDate_SetsCorrectly()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(10);

        // Act
        var subTaskDto = new SubTaskDto
        {
            Title = "Future SubTask",
            CreatedAt = DateTime.UtcNow,
            DueDate = futureDate,
            TaskId = 1
        };

        // Assert
        Assert.AreEqual(futureDate, subTaskDto.DueDate);
        Assert.IsTrue(subTaskDto.DueDate > DateTime.UtcNow);
    }

    [Test]
    public void SubTaskDto_WithSameDateCreatedAndDue_SetsCorrectly()
    {
        // Arrange
        var sameDate = DateTime.UtcNow;

        // Act
        var subTaskDto = new SubTaskDto
        {
            Title = "Same Date SubTask",
            CreatedAt = sameDate,
            DueDate = sameDate,
            TaskId = 1
        };

        // Assert
        Assert.AreEqual(sameDate, subTaskDto.CreatedAt);
        Assert.AreEqual(sameDate, subTaskDto.DueDate);
        Assert.AreEqual(subTaskDto.CreatedAt, subTaskDto.DueDate);
    }

    [Test]
    public void SubTaskDto_WithAllPropertiesSet_SetsCorrectly()
    {
        // Arrange
        var createdAt = DateTime.UtcNow.AddDays(-2);
        var updatedAt = DateTime.UtcNow.AddDays(-1);
        var dueDate = DateTime.UtcNow.AddDays(3);

        // Act
        var subTaskDto = new SubTaskDto
        {
            Id = 99,
            Title = "Complete SubTask",
            Description = "Full description of the subtask",
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            DueDate = dueDate,
            IsCompleted = true,
            Priority = Priority.High,
            TaskId = 42
        };

        // Assert
        Assert.AreEqual(99, subTaskDto.Id);
        Assert.AreEqual("Complete SubTask", subTaskDto.Title);
        Assert.AreEqual("Full description of the subtask", subTaskDto.Description);
        Assert.AreEqual(createdAt, subTaskDto.CreatedAt);
        Assert.AreEqual(updatedAt, subTaskDto.UpdatedAt);
        Assert.AreEqual(dueDate, subTaskDto.DueDate);
        Assert.IsTrue(subTaskDto.IsCompleted);
        Assert.AreEqual(Priority.High, subTaskDto.Priority);
        Assert.AreEqual(42, subTaskDto.TaskId);
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
