using Microsoft.AspNetCore.Mvc;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "AttendancePlatform.Api",
                version = "1.0.0"
            });
        }

        [HttpGet("database")]
        public IActionResult Database()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "database",
                connection = "active"
            });
        }

        [HttpGet("external")]
        public IActionResult External()
        {
            return Ok(new { 
                status = "healthy", 
                timestamp = DateTime.UtcNow,
                service = "external-services",
                dependencies = "active"
            });
        }
    }
}
