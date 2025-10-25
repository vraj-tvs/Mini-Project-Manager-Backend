using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ProjectManagerAPI.DTOs;
using ProjectManagerAPI.Services;

namespace ProjectManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ITaskService _taskService;
        private readonly ISchedulingService _schedulingService;

        public ProjectsController(IProjectService projectService, ITaskService taskService, ISchedulingService schedulingService)
        {
            _projectService = projectService;
            _taskService = taskService;
            _schedulingService = schedulingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var userId = GetCurrentUserId();
            var projects = await _projectService.GetUserProjectsAsync(userId);
            return Ok(projects);
        }

        [HttpPost("{id}/tasks")]
        public async Task<IActionResult> CreateProjectTasks(int id, [FromBody] CreateTaskDto[] createTaskDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (createTaskDtos == null || createTaskDtos.Length == 0)
            {
                return BadRequest(new { message = "At least one task is required" });
            }

            var userId = GetCurrentUserId();
            var createdTasks = new List<TaskResponseDto>();

            foreach (var createTaskDto in createTaskDtos)
            {
                var task = await _taskService.CreateTaskAsync(createTaskDto, id, userId);

                if (task == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                createdTasks.Add(task);
            }

            return CreatedAtAction(nameof(GetProject), new { id = id }, createdTasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var userId = GetCurrentUserId();
            var project = await _projectService.GetProjectByIdAsync(id, userId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto createProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var project = await _projectService.CreateProjectAsync(createProjectDto, userId);

            return CreatedAtAction(nameof(GetProject), new { id = project!.Id }, project);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto updateProjectDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var project = await _projectService.UpdateProjectAsync(id, updateProjectDto, userId);

            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            return Ok(project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _projectService.DeleteProjectAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Project not found" });
            }

            return NoContent();
        }

        [HttpPost("{projectId}/schedule")]
        public async Task<IActionResult> ScheduleProjectTasks(int projectId, [FromBody] ScheduleRequestDto scheduleRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            
            // Verify project belongs to user
            var project = await _projectService.GetProjectByIdAsync(projectId, userId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            try
            {
                var scheduleResult = _schedulingService.ScheduleTasks(scheduleRequest);
                return Ok(scheduleResult);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while scheduling tasks" });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}
