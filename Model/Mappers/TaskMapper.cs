using Model.Dtos;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Mappers;

public static class TaskMapper
{
    public static TaskModel ToModel(TaskDto dto)
    {
        return new TaskModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            DueDate = dto.DueDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Priority = dto.Priority
        };
    }
}
