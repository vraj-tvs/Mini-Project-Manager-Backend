using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.DTOs;
using ProjectManagerAPI.Models;

namespace ProjectManagerAPI.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(int projectId, int userId);
        Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);
        Task<TaskResponseDto?> CreateTaskAsync(CreateTaskDto createTaskDto, int projectId, int userId);
        Task<TaskResponseDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }

    public class TaskService : ITaskService
    {
        private readonly TaskManagerContext _context;

        public TaskService(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetProjectTasksAsync(int projectId, int userId)
        {
            // Verify project belongs to user
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return Enumerable.Empty<TaskResponseDto>();

            var tasks = await _context.Tasks
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.Project)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                DueDate = t.DueDate,
                IsCompleted = t.IsCompleted,
                CreatedAt = t.CreatedAt,
                ProjectId = t.ProjectId,
                ProjectTitle = t.Project.Title,
                EstimatedHours = t.EstimatedHours,
                Dependencies = t.Dependencies != null ? System.Text.Json.JsonSerializer.Deserialize<string[]>(t.Dependencies) : null
            });
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
                return null;

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectTitle = task.Project.Title,
                EstimatedHours = task.EstimatedHours,
                Dependencies = task.Dependencies != null ? System.Text.Json.JsonSerializer.Deserialize<string[]>(task.Dependencies) : null
            };
        }

        public async Task<TaskResponseDto?> CreateTaskAsync(CreateTaskDto createTaskDto, int projectId, int userId)
        {
            // Verify project belongs to user
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return null;

            var task = new Models.Task
            {
                Title = createTaskDto.Title,
                DueDate = createTaskDto.DueDate,
                ProjectId = projectId,
                CreatedAt = DateTime.UtcNow,
                EstimatedHours = createTaskDto.EstimatedHours,
                Dependencies = createTaskDto.Dependencies != null ? System.Text.Json.JsonSerializer.Serialize(createTaskDto.Dependencies) : null
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectTitle = project.Title,
                EstimatedHours = task.EstimatedHours,
                Dependencies = task.Dependencies != null ? System.Text.Json.JsonSerializer.Deserialize<string[]>(task.Dependencies) : null
            };
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, UpdateTaskDto updateTaskDto, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
                return null;

            task.Title = updateTaskDto.Title;
            task.DueDate = updateTaskDto.DueDate;
            task.IsCompleted = updateTaskDto.IsCompleted;
            task.EstimatedHours = updateTaskDto.EstimatedHours;
            task.Dependencies = updateTaskDto.Dependencies != null ? System.Text.Json.JsonSerializer.Serialize(updateTaskDto.Dependencies) : null;

            await _context.SaveChangesAsync();

            return new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt,
                ProjectId = task.ProjectId,
                ProjectTitle = task.Project.Title,
                EstimatedHours = task.EstimatedHours,
                Dependencies = task.Dependencies != null ? System.Text.Json.JsonSerializer.Deserialize<string[]>(task.Dependencies) : null
            };
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var task = await _context.Tasks
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.Project.UserId == userId);

            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
