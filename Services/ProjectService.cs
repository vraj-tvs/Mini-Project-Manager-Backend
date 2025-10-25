using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;
using ProjectManagerAPI.DTOs;
using ProjectManagerAPI.Models;

namespace ProjectManagerAPI.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(int userId);
        Task<ProjectResponseDto?> GetProjectByIdAsync(int projectId, int userId);
        Task<ProjectResponseDto?> CreateProjectAsync(CreateProjectDto createProjectDto, int userId);
        Task<ProjectResponseDto?> UpdateProjectAsync(int projectId, UpdateProjectDto updateProjectDto, int userId);
        Task<bool> DeleteProjectAsync(int projectId, int userId);
    }

    public class ProjectService : IProjectService
    {
        private readonly TaskManagerContext _context;

        public ProjectService(TaskManagerContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetUserProjectsAsync(int userId)
        {
            var projects = await _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.Tasks)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                TaskCount = p.Tasks.Count,
                CompletedTaskCount = p.Tasks.Count(t => t.IsCompleted)
            });
        }

        public async Task<ProjectResponseDto?> GetProjectByIdAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return null;

            return new ProjectResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                TaskCount = project.Tasks.Count,
                CompletedTaskCount = project.Tasks.Count(t => t.IsCompleted)
            };
        }

        public async Task<ProjectResponseDto?> CreateProjectAsync(CreateProjectDto createProjectDto, int userId)
        {
            var project = new Project
            {
                Title = createProjectDto.Title,
                Description = createProjectDto.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return new ProjectResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                TaskCount = 0,
                CompletedTaskCount = 0
            };
        }

        public async Task<ProjectResponseDto?> UpdateProjectAsync(int projectId, UpdateProjectDto updateProjectDto, int userId)
        {
            var project = await _context.Projects
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return null;

            project.Title = updateProjectDto.Title;
            project.Description = updateProjectDto.Description;

            await _context.SaveChangesAsync();

            return new ProjectResponseDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                TaskCount = project.Tasks.Count,
                CompletedTaskCount = project.Tasks.Count(t => t.IsCompleted)
            };
        }

        public async Task<bool> DeleteProjectAsync(int projectId, int userId)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
