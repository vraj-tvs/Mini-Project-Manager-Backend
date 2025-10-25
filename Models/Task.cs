using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagerAPI.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        [Required]
        public bool IsCompleted { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // New fields for scheduling
        public int? EstimatedHours { get; set; }
        
        [StringLength(1000)]
        public string? Dependencies { get; set; } // JSON string of dependency task titles

        // Foreign key
        [Required]
        public int ProjectId { get; set; }

        // Navigation properties
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; } = null!;
    }
}
