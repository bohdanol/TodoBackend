using Model.Enums;
using Model.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace Tests.Models;

[TestFixture]
public class TaskModelTests
{
    [Test]
    public void TaskModel_WithInitialization_ReturnsEmptySubTasksList()
    {
        // Arrange & Act
        var task = new TaskModel
        { 
            Title = "Test Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
        };

        // Assert
        Assert.IsNotNull(task.SubTasks);
        Assert.IsEmpty(task.SubTasks);
    }

    [Test]
    public void TaskModel_WithDefaultValues_ReturnsIsCompletedFalse()
    {
        // Arrange & Act
        var task = new TaskModel
        {
            Title = "Test Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
        };

        // Assert
        Assert.IsFalse(task.IsCompleted);
    }

    [Test]
    public void ValidateModel_WithValidData_PassesValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Valid Title",
            Description = "Some description",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullTitle_FailsValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Description = "Some description",
            DueDate = DateTime.Now.AddDays(1),
            Title = null!,
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithTitleTooLong_FailsValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = new string('A', 251),
            Description = "desc",
            DueDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithDescriptionTooLong_FailsValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Test title",
            Description = new string('A', 501),
            DueDate = DateTime.Now,
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Description")));
    }

    [Test]
    public void SubTasksProperty_WithForeignKeyAttribute_ReturnsCorrectTaskId()
    {
        // Arrange
        var prop = typeof(TaskModel).GetProperty(nameof(TaskModel.SubTasks));

        // Act
        var attr = prop?.GetCustomAttribute<ForeignKeyAttribute>();

        // Assert
        Assert.IsNotNull(attr, "ForeignKeyAttribute not found on SubTasks property.");
        Assert.AreEqual("TaskId", attr.Name);
    }

    [Test]
    public void ValidateModel_WithEmptyTitle_FailsValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "",
            Description = "Some description",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsTrue(results.Any(r => r.MemberNames.Contains("Title")));
    }

    [Test]
    public void ValidateModel_WithMaxLengthTitle_PassesValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = new string('A', 250), // Max allowed length
            Description = "Valid description",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithMaxLengthDescription_PassesValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Valid Title",
            Description = new string('B', 500), // Max allowed length
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void ValidateModel_WithNullDescription_PassesValidation()
    {
        // Arrange
        var task = new TaskModel
        {
            Title = "Valid Title",
            Description = null,
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Medium
        };

        // Act
        var results = ValidateModel(task);

        // Assert
        Assert.IsEmpty(results);
    }

    [Test]
    public void TaskModel_WithDifferentPriorities_SetsCorrectly()
    {
        // Arrange & Act
        var highPriorityTask = new TaskModel
        {
            Title = "High Priority Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.High
        };

        var lowPriorityTask = new TaskModel
        {
            Title = "Low Priority Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            Priority = Priority.Low
        };

        // Assert
        Assert.AreEqual(Priority.High, highPriorityTask.Priority);
        Assert.AreEqual(Priority.Low, lowPriorityTask.Priority);
    }

    [Test]
    public void TaskModel_WithCompletedStatus_SetsCorrectly()
    {
        // Arrange & Act
        var completedTask = new TaskModel
        {
            Title = "Completed Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now,
            IsCompleted = true
        };

        // Assert
        Assert.IsTrue(completedTask.IsCompleted);
    }

    [Test]
    public void TaskModel_WithUpdatedAt_SetsCorrectly()
    {
        // Arrange
        var updatedTime = DateTime.Now;
        
        // Act
        var task = new TaskModel
        {
            Title = "Updated Task",
            DueDate = DateTime.Now.AddDays(1),
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = updatedTime
        };

        // Assert
        Assert.AreEqual(updatedTime, task.UpdatedAt);
    }

    private List<ValidationResult> ValidateModel(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        Validator.TryValidateObject(model, context, results, validateAllProperties: true);
        return results;
    }
}
