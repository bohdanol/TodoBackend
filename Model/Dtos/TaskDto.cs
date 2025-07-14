using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    [Required]
    [StringLength(250, MinimumLength = 1, ErrorMessage = "Title length should be between 1 and 250 characters")]
    public required string Title { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Description length should be between 1 and 500 characters")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "CreatedAt is required")]
    public required DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [Required(ErrorMessage = "DueDate is required")]
    public required DateTime DueDate { get; set; }
    [EnumDataType(typeof(Priority), ErrorMessage = "Invalid priority.")]
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public List<SubTaskDto>? SubTasks { get; set; }
}
