using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(
            IAttendanceService attendanceService,
            ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        [HttpPost("checkin")]
        public async Task<ActionResult<ApiResponse<Application.DTOs.AttendanceRecordDto>>> CheckIn([FromBody] Application.DTOs.CheckInRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<Application.DTOs.AttendanceRecordDto>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.CheckInAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<ApiResponse<Application.DTOs.AttendanceRecordDto>>> CheckOut([FromBody] Application.DTOs.CheckOutRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<Application.DTOs.AttendanceRecordDto>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.CheckOutAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my-attendance")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>>> GetMyAttendance(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.GetUserAttendanceAsync(userId.Value, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("today")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>>> GetTodayAttendance()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.GetTodayAttendanceAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("last")]
        public async Task<ActionResult<ApiResponse<Application.DTOs.AttendanceRecordDto?>>> GetLastAttendance()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<Application.DTOs.AttendanceRecordDto?>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.GetLastAttendanceAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("validate-geofence")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateGeofence([FromBody] LocationRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.ValidateGeofenceAsync(request.Latitude, request.Longitude, userId.Value);
            return Ok(result);
        }

        [HttpPost("validate-beacon")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateBeacon([FromBody] BeaconRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _attendanceService.ValidateBeaconAsync(request.BeaconId, userId.Value);
            return Ok(result);
        }

        [HttpGet("reports")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<PagedResult<Application.DTOs.AttendanceRecordDto>>>> GetAttendanceReports(
            [FromQuery] PagedRequest request,
            [FromQuery] Guid? userId = null)
        {
            var result = await _attendanceService.GetAttendanceReportsAsync(request, userId);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>>> GetUserAttendance(
            Guid userId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _attendanceService.GetUserAttendanceAsync(userId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("sub")?.Value ??
                             User.FindFirst("userId")?.Value;
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            
            return null;
        }
    }

    public class LocationRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class BeaconRequest
    {
        public string BeaconId { get; set; } = string.Empty;
    }
}
