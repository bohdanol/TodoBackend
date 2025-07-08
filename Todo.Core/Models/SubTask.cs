using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Core.Models.Enums;

namespace Todo.Core.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Priority Priority { get; set; } = Priority.Low;

        public required Task Task { get; set; }
    }
}
