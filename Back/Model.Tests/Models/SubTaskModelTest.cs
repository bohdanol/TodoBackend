using Model.Enums;
using Model.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Model.Tests.Models;

[TestFixture]
public class SubTaskModelTest
{
    [Test]
    public void SubTaskModel_WithDefaultValues_ReturnsIsCompletedFalse()
    {
        // Arrange & Act
        var subTask = new SubTaskModel
        { 
            CreatedAt = DateTime.Now,
            DueDate = DateTime.Now.AddDays(1),
            Title = "Test subtask",
        };

        // Assert
        Assert.IsFalse(subTask.IsCompleted);
    }

    [Test]
    public void ValidateModel_WithValidData_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Test subtask",
            Description = "Optional description",
            DueDate = DateTime.Now.AddDays(3),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullTitle_FailsValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = null!,
            DueDate = DateTime.Now.AddDays(2),
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithTitleTooLong_FailsValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = new string('A', 251),
            DueDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithDescriptionTooLong_FailsValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Valid",
            Description = new string('B', 501),
            DueDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void ValidateModel_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "",
            Description = "Some description",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthTitle_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = new string('A', 250), // Max allowed length
            Description = "Valid description",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithMaxLengthDescription_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Valid Title",
            Description = new string('B', 500), // Max allowed length
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullDescription_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Valid Title",
            Description = null,
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void SubTaskModel_WithDifferentPriorities_SetsCorrectly()
    {
        // Arrange & Act
        var highPrioritySubTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "High Priority SubTask",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        var lowPrioritySubTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Low Priority SubTask",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Assert
        Assert.AreEqual(Priority.High, highPrioritySubTask.Priority);
        Assert.AreEqual(Priority.Low, lowPrioritySubTask.Priority);
    }

    [Test]
    public void SubTaskModel_WithCompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var completedSubTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Completed SubTask",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            IsCompleted = true
        };

        // Assert
        Assert.IsTrue(completedSubTask.IsCompleted);
    }

    [Test]
    public void SubTaskModel_WithUpdatedAt_SetsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.Now;
        
        // Act
        var subTask = new SubTaskModel
        {
            TaskId = 1,
            Title = "Updated SubTask",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = updatedTime
        };

        // Assert
        Assert.AreEqual(updatedTime, subTask.UpdatedAt);
    }

    [Test]
    public void SubTaskModel_WithTaskId_SetsCorrectly()
    {
        // Arrange & Act
        var subTask = new SubTaskModel
        {
            TaskId = 42,
            Title = "SubTask for Task 42",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now
        };

        // Assert
        Assert.AreEqual(42, subTask.TaskId);
    }

    [Test]
    public void SubTaskModelClass_WithTableAttribute_ReturnsCorrectTableName()
    {
        // Arrange
        var type = typeof(SubTaskModel);

        // Act
        var tableAttribute = type.GetCustomAttribute<TableAttribute>();

        // Assert
        Assert.IsNotNull(tableAttribute, "TableAttribute not found on SubTaskModel class.");
        Assert.AreEqual("SubTasks", tableAttribute.Name);
    }

    [Test]
    public void ValidateModel_WithZeroTaskId_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = 0,
            Title = "Valid Title",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results); // TaskId validation might be handled at business logic level
    }

    [Test]
    public void ValidateModel_WithNegativeTaskId_PassesValidation()
    {
        // Arrange
        var subTask = new SubTaskModel
        {
            TaskId = -1,
            Title = "Valid Title",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(subTask);

        // Assert
        Assert.IsEmpty(results); // TaskId validation might be handled at business logic level
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
