using Model.Dtos;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Model.Tests.Dtos;

[TestFixture]
public class TaskDtoTest
{
    [Test]
    public void TaskDto_WithDefaultValues_ReturnsIsCompletedFalse()
    {
        // Arrange & Act
        var taskDto = new TaskDto
        {
            Title = "Test Task",
            Description = "Valid description", // Required field
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Assert
        Assert.IsFalse(taskDto.IsCompleted);
    }

    [Test]
    public void ValidateModel_WithValidData_PassesValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Id = 1,
            Title = "Valid Task Title",
            Description = "Valid task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.High,
            IsCompleted = false
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullTitle_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = null!,
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "",
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithTitleTooLong_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = new string('A', 251), // Exceeds 250 character limit
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthTitle_PassesValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = new string('A', 250), // Exactly 250 characters
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullDescription_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = null, // Null description should fail because it's [Required]
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithEmptyDescription_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = "", // Empty description should fail because MinimumLength = 1
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithDescriptionTooLong_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = new string('B', 501), // Exceeds 500 character limit
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthDescription_PassesValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = new string('B', 500), // Exactly 500 characters
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithMinLengthDescription_PassesValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = "A", // Exactly 1 character (minimum allowed)
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var results = ValidateModel(taskDto);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void TaskDto_WithDifferentPriorities_SetsCorrectly()
    {
        // Arrange & Act
        var urgentTaskDto = new TaskDto
        {
            Title = "Urgent Task",
            Description = "Urgent task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddHours(1),
            Priority = Priority.Urgent
        };

        var lowTaskDto = new TaskDto
        {
            Title = "Low Priority Task",
            Description = "Low priority task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(7),
            Priority = Priority.Low
        };

        // Assert
        Assert.AreEqual(Priority.Urgent, urgentTaskDto.Priority);
        Assert.AreEqual(Priority.Low, lowTaskDto.Priority);
    }

    [Test]
    public void TaskDto_WithCompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var completedTaskDto = new TaskDto
        {
            Title = "Completed Task",
            Description = "Completed task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow,
            IsCompleted = true
        };

        // Assert
        Assert.IsTrue(completedTaskDto.IsCompleted);
    }

    [Test]
    public void TaskDto_WithUpdatedAt_SetsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.UtcNow;

        // Act
        var taskDto = new TaskDto
        {
            Title = "Updated Task",
            Description = "Updated task description",
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DueDate = DateTime.UtcNow.AddDays(1),
            UpdatedAt = updatedTime
        };

        // Assert
        Assert.AreEqual(updatedTime, taskDto.UpdatedAt);
    }

    [Test]
    public void TaskDto_WithSubTasks_SetsCorrectly()
    {
        // Arrange
        var subTasks = new List<SubTaskDto>
        {
            new SubTaskDto
            {
                Id = 1,
                TaskId = 1,
                Title = "SubTask 1",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1)
            },
            new SubTaskDto
            {
                Id = 2,
                TaskId = 1,
                Title = "SubTask 2",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(2)
            }
        };

        // Act
        var taskDto = new TaskDto
        {
            Title = "Task with SubTasks",
            Description = "Task with subtasks description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(3),
            SubTasks = subTasks
        };

        // Assert
        Assert.IsNotNull(taskDto.SubTasks);
        Assert.AreEqual(2, taskDto.SubTasks.Count);
        Assert.AreEqual("SubTask 1", taskDto.SubTasks[0].Title);
        Assert.AreEqual("SubTask 2", taskDto.SubTasks[1].Title);
    }

    [Test]
    public void TaskDto_WithNullSubTasks_SetsCorrectly()
    {
        // Arrange & Act
        var taskDto = new TaskDto
        {
            Title = "Task without SubTasks",
            Description = "Task without subtasks description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            SubTasks = null
        };

        // Assert
        Assert.IsNull(taskDto.SubTasks);
    }

    [Test]
    public void TaskDto_WithIdProperty_SetsCorrectly()
    {
        // Arrange & Act
        var taskDto = new TaskDto
        {
            Id = 42,
            Title = "Task with ID",
            Description = "Task with ID description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1)
        };

        // Assert
        Assert.AreEqual(42, taskDto.Id);
    }

    [Test]
    public void ValidateModel_WithInvalidPriority_FailsValidation()
    {
        // Arrange
        var taskDto = new TaskDto
        {
            Title = "Valid Title",
            Description = "Valid description",
            CreatedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(1),
            Priority = (Priority)999 // Invalid enum value
        };

        // Act
        var results = ValidateModel(taskDto);

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
            var taskDto = new TaskDto
            {
                Title = $"Task with {priority} priority",
                Description = $"Description for {priority} priority task",
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(1),
                Priority = priority
            };

            // Act
            var results = ValidateModel(taskDto);

            // Assert
            Assert.IsEmpty(results, $"Validation should pass for Priority.{priority}");
        }
    }

    [Test]
    public void TaskDto_WithPastDueDate_SetsCorrectly()
    {
        // Arrange
        var pastDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var taskDto = new TaskDto
        {
            Title = "Overdue Task",
            Description = "Overdue task description",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            DueDate = pastDate
        };

        // Assert
        Assert.AreEqual(pastDate, taskDto.DueDate);
        Assert.IsTrue(taskDto.DueDate < DateTime.UtcNow);
    }

    [Test]
    public void TaskDto_WithFutureDueDate_SetsCorrectly()
    {
        // Arrange
        var futureDate = DateTime.UtcNow.AddDays(30);

        // Act
        var taskDto = new TaskDto
        {
            Title = "Future Task",
            Description = "Future task description",
            CreatedAt = DateTime.UtcNow,
            DueDate = futureDate
        };

        // Assert
        Assert.AreEqual(futureDate, taskDto.DueDate);
        Assert.IsTrue(taskDto.DueDate > DateTime.UtcNow);
    }

    [Test]
    public void TaskDto_WithSameDateCreatedAndDue_SetsCorrectly()
    {
        // Arrange
        var sameDate = DateTime.UtcNow;

        // Act
        var taskDto = new TaskDto
        {
            Title = "Same Date Task",
            Description = "Same date task description",
            CreatedAt = sameDate,
            DueDate = sameDate
        };

        // Assert
        Assert.AreEqual(sameDate, taskDto.CreatedAt);
        Assert.AreEqual(sameDate, taskDto.DueDate);
        Assert.AreEqual(taskDto.CreatedAt, taskDto.DueDate);
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
