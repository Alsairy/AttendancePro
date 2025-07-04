using Microsoft.AspNetCore.Mvc;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/iot")]
    public class IoTController : ControllerBase
    {
        private readonly ILogger<IoTController> _logger;

        public IoTController(ILogger<IoTController> logger)
        {
            _logger = logger;
        }

        [HttpGet("devices")]
        public async Task<IActionResult> GetIoTDevices()
        {
            try
            {
                var devices = new
                {
                    total_devices = 234,
                    online_devices = 228,
                    offline_devices = 6,
                    device_types = new
                    {
                        attendance_kiosks = 45,
                        environmental_sensors = 89,
                        security_cameras = 67,
                        access_control = 33
                    },
                    recent_alerts = new[]
                    {
                        new
                        {
                            device_id = "KIOSK_001",
                            alert = "Low battery",
                            timestamp = "2024-06-27T11:45:00Z",
                            severity = "Warning"
                        }
                    }
                };
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting IoT devices");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
