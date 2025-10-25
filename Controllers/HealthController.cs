using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerAPI.Data;

namespace ProjectManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly TaskManagerContext _context;

        public HealthController(TaskManagerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                // Check database connectivity
                await _context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    status = "Healthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    database = "Connected"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    version = "1.0.0",
                    database = "Disconnected",
                    error = ex.Message
                });
            }
        }

        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            try
            {
                // Check if the application is ready to serve requests
                await _context.Database.CanConnectAsync();
                
                return Ok(new
                {
                    status = "Ready",
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return StatusCode(503, new
                {
                    status = "Not Ready",
                    timestamp = DateTime.UtcNow
                });
            }
        }

        [HttpGet("live")]
        public IActionResult Live()
        {
            // Simple liveness check - just return OK if the service is running
            return Ok(new
            {
                status = "Alive",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
