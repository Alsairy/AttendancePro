using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Application.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveManagementController : ControllerBase
    {
        private readonly ILeaveManagementService _leaveManagementService;
        private readonly ILogger<LeaveManagementController> _logger;

        public LeaveManagementController(
            ILeaveManagementService leaveManagementService,
            ILogger<LeaveManagementController> logger)
        {
            _leaveManagementService = leaveManagementService;
            _logger = logger;
        }

        [HttpPost("leave-requests")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<LeaveRequestDto>>> CreateLeaveRequest([FromBody] CreateLeaveRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<LeaveRequestDto>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.CreateLeaveRequestAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("permission-requests")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<PermissionRequestDto>>> CreatePermissionRequest([FromBody] CreatePermissionRequestDto request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<PermissionRequestDto>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.CreatePermissionRequestAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("leave-requests/{requestId}/approve")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>>> ApproveLeaveRequest(Guid requestId, [FromBody] AttendancePlatform.Shared.Domain.DTOs.ApprovalDto approval)
        {
            var approverId = GetCurrentUserId();
            if (!approverId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.ApproveLeaveRequestAsync(requestId, approval, approverId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("permission-requests/{requestId}/approve")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>>> ApprovePermissionRequest(Guid requestId, [FromBody] AttendancePlatform.Shared.Domain.DTOs.ApprovalDto approval)
        {
            var approverId = GetCurrentUserId();
            if (!approverId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.ApprovePermissionRequestAsync(requestId, approval, approverId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("leave-requests/{requestId}")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>>> CancelLeaveRequest(Guid requestId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.CancelLeaveRequestAsync(requestId, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("permission-requests/{requestId}")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>>> CancelPermissionRequest(Guid requestId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.CancelPermissionRequestAsync(requestId, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("my-leave-requests")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<LeaveRequestDto>>>> GetMyLeaveRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<LeaveRequestDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.GetUserLeaveRequestsAsync(userId.Value, page, pageSize);
            return Ok(result);
        }

        [HttpGet("my-permission-requests")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<PermissionRequestDto>>>> GetMyPermissionRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<PermissionRequestDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.GetUserPermissionRequestsAsync(userId.Value, page, pageSize);
            return Ok(result);
        }

        [HttpGet("pending-leave-requests")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<LeaveRequestDto>>>> GetPendingLeaveRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var managerId = GetCurrentUserId();
            if (!managerId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<LeaveRequestDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.GetPendingLeaveRequestsAsync(managerId.Value, page, pageSize);
            return Ok(result);
        }

        [HttpGet("pending-permission-requests")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<PermissionRequestDto>>>> GetPendingPermissionRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var managerId = GetCurrentUserId();
            if (!managerId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<PermissionRequestDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.GetPendingPermissionRequestsAsync(managerId.Value, page, pageSize);
            return Ok(result);
        }

        [HttpGet("my-leave-balance")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<UserLeaveBalanceDto>>> GetMyLeaveBalance()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(AttendancePlatform.Shared.Domain.DTOs.ApiResponse<UserLeaveBalanceDto>.ErrorResult("User not authenticated"));
            }

            var result = await _leaveManagementService.GetUserLeaveBalanceAsync(userId.Value);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpGet("leave-types")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<IEnumerable<LeaveTypeDto>>>> GetLeaveTypes()
        {
            var result = await _leaveManagementService.GetLeaveTypesAsync();
            return Ok(result);
        }

        [HttpGet("leave-calendar")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<LeaveCalendarDto>>> GetLeaveCalendar(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] Guid? userId = null)
        {
            var result = await _leaveManagementService.GetLeaveCalendarAsync(startDate, endDate, userId);
            return Ok(result);
        }

        [HttpPut("users/{userId}/leave-balance")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<bool>>> UpdateLeaveBalance(Guid userId, [FromBody] UpdateLeaveBalanceDto request)
        {
            var result = await _leaveManagementService.UpdateLeaveBalanceAsync(userId, request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("reports")]
        [Authorize(Roles = "Admin,Manager,HR")]
        public async Task<ActionResult<AttendancePlatform.Shared.Domain.DTOs.ApiResponse<LeaveReportDto>>> GenerateLeaveReport([FromBody] LeaveReportRequestDto request)
        {
            var result = await _leaveManagementService.GenerateLeaveReportAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

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
}
