using Model.Dtos;
using Model.Enums;
using Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Mappers;

public class SubTaskMapper
{
    public static SubTaskModel ToModel(SubTaskDto dto)
    {
        return new SubTaskModel
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted,
            DueDate = dto.DueDate,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt,
            Priority = dto.Priority,
            TaskId = dto.TaskId
        };
    }
}
