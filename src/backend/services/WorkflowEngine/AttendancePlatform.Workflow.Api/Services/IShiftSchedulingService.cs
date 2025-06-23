using Hudur.Shared.Domain.Entities;

namespace AttendancePlatform.Workflow.Api.Services
{
    public interface IShiftSchedulingService
    {
        Task<ShiftTemplate> CreateShiftTemplateAsync(CreateShiftTemplateRequest request);
        Task<ShiftTemplate> UpdateShiftTemplateAsync(string templateId, UpdateShiftTemplateRequest request);
        Task<bool> DeleteShiftTemplateAsync(string templateId);
        Task<List<ShiftTemplate>> GetShiftTemplatesAsync(string tenantId);
        Task<ShiftTemplate?> GetShiftTemplateAsync(string templateId);

        Task<Shift> CreateShiftAsync(CreateShiftRequest request);
        Task<Shift> UpdateShiftAsync(string shiftId, UpdateShiftRequest request);
        Task<bool> DeleteShiftAsync(string shiftId);
        Task<List<Shift>> GetShiftsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Shift?> GetShiftAsync(string shiftId);

        Task<ShiftAssignment> AssignShiftAsync(AssignShiftRequest request);
        Task<bool> UnassignShiftAsync(string assignmentId);
        Task<List<ShiftAssignment>> GetShiftAssignmentsAsync(string tenantId, string? userId = null, DateTime? startDate = null, DateTime? endDate = null);
        Task<ShiftAssignment?> GetShiftAssignmentAsync(string assignmentId);

        Task<ShiftSwapRequest> CreateSwapRequestAsync(CreateSwapRequestRequest request);
        Task<ShiftSwapRequest> ProcessSwapRequestAsync(string requestId, ProcessSwapRequestRequest request);
        Task<List<ShiftSwapRequest>> GetSwapRequestsAsync(string tenantId, string? userId = null, SwapRequestStatus? status = null);
        Task<ShiftSwapRequest?> GetSwapRequestAsync(string requestId);

        Task<List<ShiftConflict>> DetectConflictsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<ShiftConflict> ResolveConflictAsync(string conflictId, ResolveConflictRequest request);
        Task<List<ShiftConflict>> GetConflictsAsync(string tenantId, ConflictStatus? status = null);

        Task<ShiftScheduleDto> GenerateScheduleAsync(GenerateScheduleRequest request);
        Task<List<ShiftAssignment>> AutoAssignShiftsAsync(AutoAssignRequest request);
        Task<ShiftCoverageReport> GetCoverageReportAsync(string tenantId, DateTime startDate, DateTime endDate);
        Task<List<ShiftMetrics>> GetShiftMetricsAsync(string tenantId, DateTime startDate, DateTime endDate);
    }

    public class CreateShiftTemplateRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public DayOfWeek[] WorkingDays { get; set; } = Array.Empty<DayOfWeek>();
        public ShiftType Type { get; set; }
        public int MaxEmployees { get; set; }
        public int MinEmployees { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? OvertimeMultiplier { get; set; }
        public bool AllowOvertime { get; set; }
        public bool RequiresApproval { get; set; }
    }

    public class UpdateShiftTemplateRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public DayOfWeek[]? WorkingDays { get; set; }
        public ShiftType? Type { get; set; }
        public int? MaxEmployees { get; set; }
        public int? MinEmployees { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? OvertimeMultiplier { get; set; }
        public bool? AllowOvertime { get; set; }
        public bool? RequiresApproval { get; set; }
        public bool? IsActive { get; set; }
    }

    public class CreateShiftRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public string? TemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public DayOfWeek[] WorkingDays { get; set; } = Array.Empty<DayOfWeek>();
        public ShiftType Type { get; set; }
        public int MaxEmployees { get; set; }
        public int MinEmployees { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? OvertimeMultiplier { get; set; }
        public bool AllowOvertime { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }

    public class UpdateShiftRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public DayOfWeek[]? WorkingDays { get; set; }
        public ShiftType? Type { get; set; }
        public int? MaxEmployees { get; set; }
        public int? MinEmployees { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? OvertimeMultiplier { get; set; }
        public bool? AllowOvertime { get; set; }
        public bool? RequiresApproval { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public ShiftStatus? Status { get; set; }
    }

    public class AssignShiftRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public string ShiftId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string? Notes { get; set; }
        public string? AssignedBy { get; set; }
        public bool IsRecurring { get; set; }
        public RecurrencePattern? RecurrencePattern { get; set; }
    }

    public class CreateSwapRequestRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public string RequesterId { get; set; } = string.Empty;
        public string OriginalAssignmentId { get; set; } = string.Empty;
        public string? TargetUserId { get; set; }
        public string? TargetAssignmentId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public SwapType Type { get; set; }
    }

    public class ProcessSwapRequestRequest
    {
        public string ProcessedBy { get; set; } = string.Empty;
        public SwapRequestStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    public class ResolveConflictRequest
    {
        public string ResolvedBy { get; set; } = string.Empty;
        public string ResolutionNotes { get; set; } = string.Empty;
        public ConflictStatus Status { get; set; }
    }

    public class GenerateScheduleRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string>? DepartmentIds { get; set; }
        public List<string>? LocationIds { get; set; }
        public bool AutoAssign { get; set; } = false;
        public bool ResolveConflicts { get; set; } = true;
    }

    public class AutoAssignRequest
    {
        public string TenantId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> ShiftIds { get; set; } = new();
        public List<string>? PreferredUserIds { get; set; }
        public bool ConsiderAvailability { get; set; } = true;
        public bool ConsiderSkills { get; set; } = true;
        public bool BalanceWorkload { get; set; } = true;
    }

    public class ShiftScheduleDto
    {
        public string TenantId { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ShiftAssignmentDto> Assignments { get; set; } = new();
        public List<ShiftConflictDto> Conflicts { get; set; } = new();
        public ShiftCoverageReport Coverage { get; set; } = new();
    }

    public class ShiftAssignmentDto
    {
        public string Id { get; set; } = string.Empty;
        public string ShiftId { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ShiftAssignmentStatus Status { get; set; }
        public string? Department { get; set; }
        public string? Location { get; set; }
        public string? Notes { get; set; }
    }

    public class ShiftConflictDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string AssignmentId { get; set; } = string.Empty;
        public ConflictType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public ConflictSeverity Severity { get; set; }
        public ConflictStatus Status { get; set; }
        public DateTime DetectedAt { get; set; }
    }

    public class ShiftCoverageReport
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalShifts { get; set; }
        public int AssignedShifts { get; set; }
        public int UnassignedShifts { get; set; }
        public double CoveragePercentage { get; set; }
        public List<DepartmentCoverage> DepartmentCoverage { get; set; } = new();
        public List<LocationCoverage> LocationCoverage { get; set; } = new();
    }

    public class DepartmentCoverage
    {
        public string Department { get; set; } = string.Empty;
        public int TotalShifts { get; set; }
        public int AssignedShifts { get; set; }
        public double CoveragePercentage { get; set; }
    }

    public class LocationCoverage
    {
        public string Location { get; set; } = string.Empty;
        public int TotalShifts { get; set; }
        public int AssignedShifts { get; set; }
        public double CoveragePercentage { get; set; }
    }

    public class ShiftMetrics
    {
        public string Period { get; set; } = string.Empty;
        public int TotalShifts { get; set; }
        public int CompletedShifts { get; set; }
        public int MissedShifts { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageHoursPerShift { get; set; }
        public double OvertimeHours { get; set; }
        public int SwapRequests { get; set; }
        public int ConflictsDetected { get; set; }
        public int ConflictsResolved { get; set; }
    }
}
