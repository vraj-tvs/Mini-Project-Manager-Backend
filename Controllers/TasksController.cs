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
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectTasks(int projectId)
        {
            var userId = GetCurrentUserId();
            var tasks = await _taskService.GetProjectTasksAsync(projectId, userId);
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var userId = GetCurrentUserId();
            var task = await _taskService.GetTaskByIdAsync(id, userId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }

        [HttpPost("project/{projectId}")]
        public async Task<IActionResult> CreateTask(int projectId, [FromBody] CreateTaskDto createTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var task = await _taskService.CreateTaskAsync(createTaskDto, projectId, userId);

            if (task == null)
            {
                return NotFound(new { message = "Project not found" });
            }

            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);

            if (task == null)
            {
                return NotFound(new { message = "Task not found" });
            }

            return Ok(task);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = GetCurrentUserId();
            var success = await _taskService.DeleteTaskAsync(id, userId);

            if (!success)
            {
                return NotFound(new { message = "Task not found" });
            }

            return NoContent();
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim!.Value);
        }
    }
}
