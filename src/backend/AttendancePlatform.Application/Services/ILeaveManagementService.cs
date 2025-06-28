using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Application.Services;

public interface ILeaveManagementService
{
    Task<ApiResponse<Application.DTOs.LeaveRequestDto>> CreateLeaveRequestAsync(Application.DTOs.CreateLeaveRequestDto request, Guid userId);
    Task<ApiResponse<Application.DTOs.PermissionRequestDto>> CreatePermissionRequestAsync(Application.DTOs.CreatePermissionRequestDto request, Guid userId);
    Task<ApiResponse<bool>> ApproveLeaveRequestAsync(Guid requestId, AttendancePlatform.Shared.Domain.DTOs.ApprovalDto approval, Guid approverId);
    Task<ApiResponse<bool>> ApprovePermissionRequestAsync(Guid requestId, AttendancePlatform.Shared.Domain.DTOs.ApprovalDto approval, Guid approverId);
    Task<ApiResponse<bool>> CancelLeaveRequestAsync(Guid requestId, Guid userId);
    Task<ApiResponse<bool>> CancelPermissionRequestAsync(Guid requestId, Guid userId);
    Task<ApiResponse<IEnumerable<Application.DTOs.LeaveRequestDto>>> GetUserLeaveRequestsAsync(Guid userId, int page, int pageSize);
    Task<ApiResponse<IEnumerable<Application.DTOs.PermissionRequestDto>>> GetUserPermissionRequestsAsync(Guid userId, int page, int pageSize);
    Task<ApiResponse<IEnumerable<Application.DTOs.LeaveRequestDto>>> GetPendingLeaveRequestsAsync(Guid managerId, int page, int pageSize);
    Task<ApiResponse<IEnumerable<Application.DTOs.PermissionRequestDto>>> GetPendingPermissionRequestsAsync(Guid managerId, int page, int pageSize);
    Task<ApiResponse<Application.DTOs.UserLeaveBalanceDto>> GetUserLeaveBalanceAsync(Guid userId);
    Task<ApiResponse<IEnumerable<Application.DTOs.LeaveTypeDto>>> GetLeaveTypesAsync();
    Task<ApiResponse<Application.DTOs.LeaveCalendarDto>> GetLeaveCalendarAsync(DateTime startDate, DateTime endDate, Guid? userId);
    Task<ApiResponse<bool>> UpdateLeaveBalanceAsync(Guid userId, Application.DTOs.UpdateLeaveBalanceDto request);
    Task<ApiResponse<Application.DTOs.LeaveReportDto>> GenerateLeaveReportAsync(Application.DTOs.LeaveReportRequestDto request);
}
