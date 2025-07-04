using Microsoft.AspNetCore.Mvc;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class ApiTestController : ControllerBase
    {
        private readonly ILogger<ApiTestController> _logger;

        public ApiTestController(ILogger<ApiTestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> TestEndpoint()
        {
            try
            {
                var response = new
                {
                    message = "Hudur Enterprise Platform API is running",
                    timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    comprehensive_services = "81+ business services implemented",
                    authentication = "JWT-based with role-based access control",
                    cors = "Configured for cross-origin requests"
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test endpoint");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
