using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    [Table("SubTasks")]
    public class SubTaskModel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        [Required]
        [MaxLength(250)]
        public required string Title { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [DefaultValue(false)]
        public bool IsCompleted { get; set; }
        [Required]
        public required DateTime DueDate { get; set; }
        [Required]
        public required DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Priority Priority { get; set; }
    }
}
