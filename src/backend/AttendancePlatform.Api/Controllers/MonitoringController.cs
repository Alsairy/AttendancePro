using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class MonitoringController : ControllerBase
    {
        private readonly IMonitoringService _monitoringService;

        public MonitoringController(IMonitoringService monitoringService)
        {
            _monitoringService = monitoringService;
        }

        [HttpGet("health")]
        public async Task<IActionResult> GetSystemHealth()
        {
            var health = await _monitoringService.GetSystemHealth();
            return Ok(health);
        }

        [HttpGet("performance")]
        public async Task<IActionResult> GetPerformanceMetrics()
        {
            var metrics = await _monitoringService.GetPerformanceMetrics();
            return Ok(metrics);
        }

        [HttpGet("security")]
        public async Task<IActionResult> GetSecurityMetrics()
        {
            var metrics = await _monitoringService.GetSecurityMetrics();
            return Ok(metrics);
        }

        [HttpPost("metric")]
        public async Task<IActionResult> LogPerformanceMetric([FromBody] MetricRequest request)
        {
            await _monitoringService.LogPerformanceMetric(request.Name, request.Value, request.Unit);
            return Ok(new { message = "Metric logged successfully" });
        }
    }

    public class MetricRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
        public string? Unit { get; set; }
    }
}
