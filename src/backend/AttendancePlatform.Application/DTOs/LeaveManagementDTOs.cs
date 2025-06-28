using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Application.DTOs
{
    public class CreateLeaveRequestDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid LeaveTypeId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public bool IsHalfDay { get; set; }
        public string? AttachmentUrl { get; set; }
    }

    public class UpdateLeaveRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public bool? IsHalfDay { get; set; }
        public string? AttachmentUrl { get; set; }
    }

    public class ApproveLeaveRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }
        
        [Required]
        public Guid ApproverId { get; set; }
        
        public string? Comments { get; set; }
        public bool IsApproved { get; set; }
    }

    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRequested { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public bool IsHalfDay { get; set; }
        public string? AttachmentUrl { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovalComments { get; set; }
    }

    public class CreatePermissionRequestDto
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        public string? Reason { get; set; }
        public string? Comments { get; set; }
    }

    public class PermissionRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Comments { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public Guid? ApprovedById { get; set; }
        public string? ApprovalComments { get; set; }
    }

    public class UpdateLeaveBalanceDto
    {
        public Guid UserId { get; set; }
        public int AnnualLeaveBalance { get; set; }
        public int SickLeaveBalance { get; set; }
        public int PersonalLeaveBalance { get; set; }
        public int MaternityLeaveBalance { get; set; }
        public int PaternityLeaveBalance { get; set; }
        public int NewBalance { get; set; }
        public string? Reason { get; set; }
    }

    public class LeaveBalanceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int AnnualLeaveBalance { get; set; }
        public int SickLeaveBalance { get; set; }
        public int PersonalLeaveBalance { get; set; }
        public int MaternityLeaveBalance { get; set; }
        public int PaternityLeaveBalance { get; set; }
        public int AnnualLeaveUsed { get; set; }
        public int SickLeaveUsed { get; set; }
        public int PersonalLeaveUsed { get; set; }
        public DateTime LastUpdated { get; set; }
        public int TotalLeaveBalance { get; set; }
        public int UsedLeaveBalance { get; set; }
        public int RemainingLeaveBalance { get; set; }
    }

    public class UserLeaveBalanceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int TotalLeaveBalance { get; set; }
        public int UsedLeaveBalance { get; set; }
        public int RemainingLeaveBalance { get; set; }
        public DateTime LastUpdated { get; set; }
        public int AnnualLeaveBalance { get; set; }
        public int SickLeaveBalance { get; set; }
        public int PersonalLeaveBalance { get; set; }
        public int MaternityLeaveBalance { get; set; }
        public int PaternityLeaveBalance { get; set; }
    }

    public class LeaveCalendarDto
    {
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<LeaveRequestDto> Requests { get; set; } = new();
        public List<LeaveRequestDto> LeaveRequests { get; set; } = new();
    }

    public class LeaveReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequests { get; set; }
        public List<LeaveRequestDto> LeaveRequests { get; set; } = new();
    }

    public class LeaveTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxDaysPerYear { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsActive { get; set; }
        public string Color { get; set; } = string.Empty;
    }

    public class LeaveReportRequestDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid? UserId { get; set; }
        public Guid? DepartmentId { get; set; }
        public string? Status { get; set; }
        public Guid? LeaveTypeId { get; set; }
    }

    public class ApprovalDto
    {
        public string ApprovalNotes { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime ApprovalDate { get; set; } = DateTime.UtcNow;
    }
}
