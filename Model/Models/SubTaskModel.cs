using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class SubTaskModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public required DateTime DueDate { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Priority Priority { get; set; } = Priority.Low;

        public required TaskModel Task { get; set; }
    }
}
