using System.ComponentModel.DataAnnotations;

namespace ProjectManagerAPI.DTOs
{
    public class CreateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }
        
        public int? EstimatedHours { get; set; }
        
        public string[]? Dependencies { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        [Required]
        public bool IsCompleted { get; set; }
        
        public int? EstimatedHours { get; set; }
        
        public string[]? Dependencies { get; set; }
    }

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public int? EstimatedHours { get; set; }
        public string[]? Dependencies { get; set; }
    }

    // New DTOs for scheduling
    public class TaskScheduleDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int EstimatedHours { get; set; }

        [Required]
        public string DueDate { get; set; } = string.Empty;

        [Required]
        public string[] Dependencies { get; set; } = Array.Empty<string>();
    }

    public class ScheduleRequestDto
    {
        [Required]
        public TaskScheduleDto[] Tasks { get; set; } = Array.Empty<TaskScheduleDto>();
    }

    public class ScheduleResponseDto
    {
        public string[] RecommendedOrder { get; set; } = Array.Empty<string>();
    }
}
