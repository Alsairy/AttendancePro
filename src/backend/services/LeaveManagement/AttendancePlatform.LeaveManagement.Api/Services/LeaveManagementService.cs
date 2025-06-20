using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.LeaveManagement.Api.Services
{
    public interface ILeaveManagementService
    {
        Task<ApiResponse<LeaveRequestDto>> CreateLeaveRequestAsync(CreateLeaveRequestDto request, Guid userId);
        Task<ApiResponse<PermissionRequestDto>> CreatePermissionRequestAsync(CreatePermissionRequestDto request, Guid userId);
        Task<ApiResponse<bool>> ApproveLeaveRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId);
        Task<ApiResponse<bool>> ApprovePermissionRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId);
        Task<ApiResponse<bool>> CancelLeaveRequestAsync(Guid requestId, Guid userId);
        Task<ApiResponse<bool>> CancelPermissionRequestAsync(Guid requestId, Guid userId);
        Task<ApiResponse<IEnumerable<LeaveRequestDto>>> GetUserLeaveRequestsAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<ApiResponse<IEnumerable<PermissionRequestDto>>> GetUserPermissionRequestsAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<ApiResponse<IEnumerable<LeaveRequestDto>>> GetPendingLeaveRequestsAsync(Guid managerId, int page = 1, int pageSize = 20);
        Task<ApiResponse<IEnumerable<PermissionRequestDto>>> GetPendingPermissionRequestsAsync(Guid managerId, int page = 1, int pageSize = 20);
        Task<ApiResponse<UserLeaveBalanceDto>> GetUserLeaveBalanceAsync(Guid userId);
        Task<ApiResponse<IEnumerable<LeaveTypeDto>>> GetLeaveTypesAsync();
        Task<ApiResponse<LeaveCalendarDto>> GetLeaveCalendarAsync(DateTime startDate, DateTime endDate, Guid? userId = null);
        Task<ApiResponse<bool>> UpdateLeaveBalanceAsync(Guid userId, UpdateLeaveBalanceDto request);
        Task<ApiResponse<LeaveReportDto>> GenerateLeaveReportAsync(LeaveReportRequestDto request);
    }

    public class LeaveManagementService : ILeaveManagementService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<LeaveManagementService> _logger;
        private readonly ITenantContext _tenantContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICurrentUserService _currentUserService;

        public LeaveManagementService(
            AttendancePlatformDbContext context,
            ILogger<LeaveManagementService> logger,
            ITenantContext tenantContext,
            IDateTimeProvider dateTimeProvider,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
            _dateTimeProvider = dateTimeProvider;
            _currentUserService = currentUserService;
        }

        public async Task<ApiResponse<LeaveRequestDto>> CreateLeaveRequestAsync(CreateLeaveRequestDto request, Guid userId)
        {
            try
            {
                // Validate request
                if (request.StartDate >= request.EndDate)
                {
                    return ApiResponse<LeaveRequestDto>.ErrorResult("End date must be after start date");
                }

                if (request.StartDate < _dateTimeProvider.UtcNow.Date)
                {
                    return ApiResponse<LeaveRequestDto>.ErrorResult("Cannot request leave for past dates");
                }

                // Check if leave type exists
                var leaveType = await _context.LeaveTypes
                    .FirstOrDefaultAsync(lt => lt.Id == request.LeaveTypeId && !lt.IsDeleted);

                if (leaveType == null)
                {
                    return ApiResponse<LeaveRequestDto>.ErrorResult("Invalid leave type");
                }

                // Calculate leave days
                var leaveDays = CalculateWorkingDays(request.StartDate, request.EndDate);
                
                // Check leave balance
                var leaveBalance = await GetOrCreateUserLeaveBalance(userId, leaveType.Id);
                if (leaveBalance.RemainingDays < leaveDays)
                {
                    return ApiResponse<LeaveRequestDto>.ErrorResult($"Insufficient leave balance. Available: {leaveBalance.RemainingDays} days, Requested: {leaveDays} days");
                }

                // Check for overlapping requests
                var hasOverlap = await _context.LeaveRequests
                    .AnyAsync(lr => lr.UserId == userId && 
                                   lr.Status != LeaveStatus.Rejected && 
                                   lr.Status != LeaveStatus.Cancelled &&
                                   !lr.IsDeleted &&
                                   ((request.StartDate >= lr.StartDate && request.StartDate <= lr.EndDate) ||
                                    (request.EndDate >= lr.StartDate && request.EndDate <= lr.EndDate) ||
                                    (request.StartDate <= lr.StartDate && request.EndDate >= lr.EndDate)));

                if (hasOverlap)
                {
                    return ApiResponse<LeaveRequestDto>.ErrorResult("Leave request overlaps with existing request");
                }

                // Get user's manager
                var user = await _context.Users
                    .Include(u => u.Manager)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // Create leave request
                var leaveRequest = new LeaveRequest
                {
                    UserId = userId,
                    LeaveTypeId = request.LeaveTypeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    DaysRequested = leaveDays,
                    Reason = request.Reason,
                    Status = LeaveStatus.Pending,
                    RequestDate = _dateTimeProvider.UtcNow,
                    ManagerId = user?.ManagerId,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();

                // Create approval record if manager exists
                if (leaveRequest.ManagerId.HasValue)
                {
                    var approval = new LeaveApproval
                    {
                        LeaveRequestId = leaveRequest.Id,
                        ApproverId = leaveRequest.ManagerId.Value,
                        Level = 1,
                        Status = ApprovalStatus.Pending,
                        TenantId = _tenantContext.TenantId.Value
                    };

                    _context.LeaveApprovals.Add(approval);
                    await _context.SaveChangesAsync();
                }

                var dto = await MapToLeaveRequestDto(leaveRequest);
                return ApiResponse<LeaveRequestDto>.SuccessResult(dto, "Leave request created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request for user: {UserId}", userId);
                return ApiResponse<LeaveRequestDto>.ErrorResult("An error occurred while creating the leave request");
            }
        }

        public async Task<ApiResponse<PermissionRequestDto>> CreatePermissionRequestAsync(CreatePermissionRequestDto request, Guid userId)
        {
            try
            {
                // Validate request
                if (request.StartTime >= request.EndTime)
                {
                    return ApiResponse<PermissionRequestDto>.ErrorResult("End time must be after start time");
                }

                if (request.Date < _dateTimeProvider.UtcNow.Date)
                {
                    return ApiResponse<PermissionRequestDto>.ErrorResult("Cannot request permission for past dates");
                }

                // Check for overlapping permission requests
                var hasOverlap = await _context.PermissionRequests
                    .AnyAsync(pr => pr.UserId == userId && 
                                   pr.Date == request.Date &&
                                   pr.Status != PermissionStatus.Rejected && 
                                   pr.Status != PermissionStatus.Cancelled &&
                                   !pr.IsDeleted &&
                                   ((request.StartTime >= pr.StartTime && request.StartTime < pr.EndTime) ||
                                    (request.EndTime > pr.StartTime && request.EndTime <= pr.EndTime) ||
                                    (request.StartTime <= pr.StartTime && request.EndTime >= pr.EndTime)));

                if (hasOverlap)
                {
                    return ApiResponse<PermissionRequestDto>.ErrorResult("Permission request overlaps with existing request");
                }

                // Get user's manager
                var user = await _context.Users
                    .Include(u => u.Manager)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                // Calculate duration in hours
                var duration = (request.EndTime - request.StartTime).TotalHours;

                // Create permission request
                var permissionRequest = new PermissionRequest
                {
                    UserId = userId,
                    Date = request.Date,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    DurationHours = duration,
                    Reason = request.Reason,
                    Status = PermissionStatus.Pending,
                    RequestDate = _dateTimeProvider.UtcNow,
                    ManagerId = user?.ManagerId,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.PermissionRequests.Add(permissionRequest);
                await _context.SaveChangesAsync();

                var dto = await MapToPermissionRequestDto(permissionRequest);
                return ApiResponse<PermissionRequestDto>.SuccessResult(dto, "Permission request created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission request for user: {UserId}", userId);
                return ApiResponse<PermissionRequestDto>.ErrorResult("An error occurred while creating the permission request");
            }
        }

        public async Task<ApiResponse<bool>> ApproveLeaveRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests
                    .Include(lr => lr.User)
                    .Include(lr => lr.LeaveType)
                    .FirstOrDefaultAsync(lr => lr.Id == requestId && !lr.IsDeleted);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Leave request not found");
                }

                if (leaveRequest.Status != LeaveStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Leave request is not in pending status");
                }

                // Update leave request status
                leaveRequest.Status = approval.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
                leaveRequest.UpdatedAt = _dateTimeProvider.UtcNow;

                // Update approval record
                var approvalRecord = await _context.LeaveApprovals
                    .FirstOrDefaultAsync(la => la.LeaveRequestId == requestId && la.ApproverId == approverId);

                if (approvalRecord != null)
                {
                    approvalRecord.Status = approval.IsApproved ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
                    approvalRecord.ApprovalDate = _dateTimeProvider.UtcNow;
                    approvalRecord.Comments = approval.Comments;
                    approvalRecord.UpdatedAt = _dateTimeProvider.UtcNow;
                }

                // If approved, deduct from leave balance
                if (approval.IsApproved)
                {
                    var leaveBalance = await _context.UserLeaveBalances
                        .FirstOrDefaultAsync(ulb => ulb.UserId == leaveRequest.UserId && 
                                                   ulb.LeaveTypeId == leaveRequest.LeaveTypeId);

                    if (leaveBalance != null)
                    {
                        leaveBalance.UsedDays += leaveRequest.DaysRequested;
                        leaveBalance.RemainingDays -= leaveRequest.DaysRequested;
                        leaveBalance.UpdatedAt = _dateTimeProvider.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();

                var action = approval.IsApproved ? "approved" : "rejected";
                return ApiResponse<bool>.SuccessResult(true, $"Leave request {action} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while processing the approval");
            }
        }

        public async Task<ApiResponse<bool>> ApprovePermissionRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId)
        {
            try
            {
                var permissionRequest = await _context.PermissionRequests
                    .Include(pr => pr.User)
                    .FirstOrDefaultAsync(pr => pr.Id == requestId && !pr.IsDeleted);

                if (permissionRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Permission request not found");
                }

                if (permissionRequest.Status != PermissionStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Permission request is not in pending status");
                }

                // Update permission request status
                permissionRequest.Status = approval.IsApproved ? PermissionStatus.Approved : PermissionStatus.Rejected;
                permissionRequest.ApprovalDate = _dateTimeProvider.UtcNow;
                permissionRequest.ApprovalComments = approval.Comments;
                permissionRequest.UpdatedAt = _dateTimeProvider.UtcNow;

                await _context.SaveChangesAsync();

                var action = approval.IsApproved ? "approved" : "rejected";
                return ApiResponse<bool>.SuccessResult(true, $"Permission request {action} successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving permission request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while processing the approval");
            }
        }

        public async Task<ApiResponse<bool>> CancelLeaveRequestAsync(Guid requestId, Guid userId)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.Id == requestId && lr.UserId == userId && !lr.IsDeleted);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Leave request not found");
                }

                if (leaveRequest.Status != LeaveStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Only pending leave requests can be cancelled");
                }

                leaveRequest.Status = LeaveStatus.Cancelled;
                leaveRequest.UpdatedAt = _dateTimeProvider.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Leave request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while cancelling the leave request");
            }
        }

        public async Task<ApiResponse<bool>> CancelPermissionRequestAsync(Guid requestId, Guid userId)
        {
            try
            {
                var permissionRequest = await _context.PermissionRequests
                    .FirstOrDefaultAsync(pr => pr.Id == requestId && pr.UserId == userId && !pr.IsDeleted);

                if (permissionRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Permission request not found");
                }

                if (permissionRequest.Status != PermissionStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Only pending permission requests can be cancelled");
                }

                permissionRequest.Status = PermissionStatus.Cancelled;
                permissionRequest.UpdatedAt = _dateTimeProvider.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Permission request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling permission request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while cancelling the permission request");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDto>>> GetUserLeaveRequestsAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var leaveRequests = await _context.LeaveRequests
                    .Include(lr => lr.LeaveType)
                    .Include(lr => lr.User)
                    .Include(lr => lr.Manager)
                    .Where(lr => lr.UserId == userId && !lr.IsDeleted)
                    .OrderByDescending(lr => lr.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<LeaveRequestDto>();
                foreach (var request in leaveRequests)
                {
                    dtos.Add(await MapToLeaveRequestDto(request));
                }

                return ApiResponse<IEnumerable<LeaveRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave requests for user: {UserId}", userId);
                return ApiResponse<IEnumerable<LeaveRequestDto>>.ErrorResult("An error occurred while retrieving leave requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<PermissionRequestDto>>> GetUserPermissionRequestsAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            try
            {
                var permissionRequests = await _context.PermissionRequests
                    .Include(pr => pr.User)
                    .Include(pr => pr.Manager)
                    .Where(pr => pr.UserId == userId && !pr.IsDeleted)
                    .OrderByDescending(pr => pr.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = permissionRequests.Select(MapToPermissionRequestDto).ToList();
                return ApiResponse<IEnumerable<PermissionRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission requests for user: {UserId}", userId);
                return ApiResponse<IEnumerable<PermissionRequestDto>>.ErrorResult("An error occurred while retrieving permission requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveRequestDto>>> GetPendingLeaveRequestsAsync(Guid managerId, int page = 1, int pageSize = 20)
        {
            try
            {
                var leaveRequests = await _context.LeaveRequests
                    .Include(lr => lr.LeaveType)
                    .Include(lr => lr.User)
                    .Where(lr => lr.ManagerId == managerId && 
                                lr.Status == LeaveStatus.Pending && 
                                !lr.IsDeleted)
                    .OrderBy(lr => lr.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<LeaveRequestDto>();
                foreach (var request in leaveRequests)
                {
                    dtos.Add(await MapToLeaveRequestDto(request));
                }

                return ApiResponse<IEnumerable<LeaveRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending leave requests for manager: {ManagerId}", managerId);
                return ApiResponse<IEnumerable<LeaveRequestDto>>.ErrorResult("An error occurred while retrieving pending leave requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<PermissionRequestDto>>> GetPendingPermissionRequestsAsync(Guid managerId, int page = 1, int pageSize = 20)
        {
            try
            {
                var permissionRequests = await _context.PermissionRequests
                    .Include(pr => pr.User)
                    .Where(pr => pr.ManagerId == managerId && 
                                pr.Status == PermissionStatus.Pending && 
                                !pr.IsDeleted)
                    .OrderBy(pr => pr.RequestDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = permissionRequests.Select(MapToPermissionRequestDto).ToList();
                return ApiResponse<IEnumerable<PermissionRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending permission requests for manager: {ManagerId}", managerId);
                return ApiResponse<IEnumerable<PermissionRequestDto>>.ErrorResult("An error occurred while retrieving pending permission requests");
            }
        }

        public async Task<ApiResponse<UserLeaveBalanceDto>> GetUserLeaveBalanceAsync(Guid userId)
        {
            try
            {
                var leaveBalances = await _context.UserLeaveBalances
                    .Include(ulb => ulb.LeaveType)
                    .Where(ulb => ulb.UserId == userId && !ulb.IsDeleted)
                    .ToListAsync();

                var balanceDtos = leaveBalances.Select(lb => new LeaveBalanceDto
                {
                    LeaveTypeId = lb.LeaveTypeId,
                    LeaveTypeName = lb.LeaveType?.Name ?? "Unknown",
                    AllocatedDays = lb.AllocatedDays,
                    UsedDays = lb.UsedDays,
                    RemainingDays = lb.RemainingDays,
                    CarryForwardDays = lb.CarryForwardDays,
                    Year = lb.Year
                }).ToList();

                var dto = new UserLeaveBalanceDto
                {
                    UserId = userId,
                    Year = DateTime.UtcNow.Year,
                    LeaveBalances = balanceDtos
                };

                return ApiResponse<UserLeaveBalanceDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave balance for user: {UserId}", userId);
                return ApiResponse<UserLeaveBalanceDto>.ErrorResult("An error occurred while retrieving leave balance");
            }
        }

        public async Task<ApiResponse<IEnumerable<LeaveTypeDto>>> GetLeaveTypesAsync()
        {
            try
            {
                var leaveTypes = await _context.LeaveTypes
                    .Where(lt => !lt.IsDeleted)
                    .OrderBy(lt => lt.Name)
                    .ToListAsync();

                var dtos = leaveTypes.Select(lt => new LeaveTypeDto
                {
                    Id = lt.Id,
                    Name = lt.Name,
                    Description = lt.Description,
                    MaxDaysPerYear = lt.MaxDaysPerYear,
                    IsCarryForwardAllowed = lt.IsCarryForwardAllowed,
                    MaxCarryForwardDays = lt.MaxCarryForwardDays,
                    RequiresApproval = lt.RequiresApproval,
                    IsActive = lt.IsActive
                }).ToList();

                return ApiResponse<IEnumerable<LeaveTypeDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave types");
                return ApiResponse<IEnumerable<LeaveTypeDto>>.ErrorResult("An error occurred while retrieving leave types");
            }
        }

        public async Task<ApiResponse<LeaveCalendarDto>> GetLeaveCalendarAsync(DateTime startDate, DateTime endDate, Guid? userId = null)
        {
            try
            {
                var query = _context.LeaveRequests
                    .Include(lr => lr.User)
                    .Include(lr => lr.LeaveType)
                    .Where(lr => lr.Status == LeaveStatus.Approved && 
                                !lr.IsDeleted &&
                                lr.StartDate <= endDate && 
                                lr.EndDate >= startDate);

                if (userId.HasValue)
                {
                    query = query.Where(lr => lr.UserId == userId.Value);
                }

                var leaveRequests = await query.ToListAsync();

                var calendarItems = leaveRequests.Select(lr => new LeaveCalendarItemDto
                {
                    Id = lr.Id,
                    UserId = lr.UserId,
                    UserName = $"{lr.User?.FirstName} {lr.User?.LastName}",
                    LeaveTypeName = lr.LeaveType?.Name ?? "Unknown",
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    DaysRequested = lr.DaysRequested,
                    Reason = lr.Reason
                }).ToList();

                var dto = new LeaveCalendarDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    LeaveItems = calendarItems
                };

                return ApiResponse<LeaveCalendarDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave calendar");
                return ApiResponse<LeaveCalendarDto>.ErrorResult("An error occurred while retrieving leave calendar");
            }
        }

        public async Task<ApiResponse<bool>> UpdateLeaveBalanceAsync(Guid userId, UpdateLeaveBalanceDto request)
        {
            try
            {
                var leaveBalance = await _context.UserLeaveBalances
                    .FirstOrDefaultAsync(ulb => ulb.UserId == userId && 
                                               ulb.LeaveTypeId == request.LeaveTypeId &&
                                               ulb.Year == request.Year);

                if (leaveBalance == null)
                {
                    // Create new balance
                    leaveBalance = new UserLeaveBalance
                    {
                        UserId = userId,
                        LeaveTypeId = request.LeaveTypeId,
                        Year = request.Year,
                        AllocatedDays = request.AllocatedDays,
                        UsedDays = 0,
                        RemainingDays = request.AllocatedDays,
                        CarryForwardDays = request.CarryForwardDays,
                        TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                    };

                    _context.UserLeaveBalances.Add(leaveBalance);
                }
                else
                {
                    // Update existing balance
                    leaveBalance.AllocatedDays = request.AllocatedDays;
                    leaveBalance.CarryForwardDays = request.CarryForwardDays;
                    leaveBalance.RemainingDays = request.AllocatedDays + request.CarryForwardDays - leaveBalance.UsedDays;
                    leaveBalance.UpdatedAt = _dateTimeProvider.UtcNow;
                }

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Leave balance updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leave balance for user: {UserId}", userId);
                return ApiResponse<bool>.ErrorResult("An error occurred while updating leave balance");
            }
        }

        public async Task<ApiResponse<LeaveReportDto>> GenerateLeaveReportAsync(LeaveReportRequestDto request)
        {
            try
            {
                var query = _context.LeaveRequests
                    .Include(lr => lr.User)
                    .Include(lr => lr.LeaveType)
                    .Where(lr => !lr.IsDeleted &&
                                lr.StartDate >= request.StartDate &&
                                lr.EndDate <= request.EndDate);

                if (request.UserId.HasValue)
                {
                    query = query.Where(lr => lr.UserId == request.UserId.Value);
                }

                if (request.LeaveTypeId.HasValue)
                {
                    query = query.Where(lr => lr.LeaveTypeId == request.LeaveTypeId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(lr => lr.Status == request.Status.Value);
                }

                var leaveRequests = await query.ToListAsync();

                var reportItems = leaveRequests.Select(lr => new LeaveReportItemDto
                {
                    UserId = lr.UserId,
                    UserName = $"{lr.User?.FirstName} {lr.User?.LastName}",
                    EmployeeId = lr.User?.EmployeeId,
                    LeaveTypeName = lr.LeaveType?.Name ?? "Unknown",
                    StartDate = lr.StartDate,
                    EndDate = lr.EndDate,
                    DaysRequested = lr.DaysRequested,
                    Status = lr.Status.ToString(),
                    RequestDate = lr.RequestDate,
                    Reason = lr.Reason
                }).ToList();

                var dto = new LeaveReportDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalRequests = reportItems.Count,
                    TotalDaysRequested = reportItems.Sum(ri => ri.DaysRequested),
                    ReportItems = reportItems,
                    GeneratedAt = _dateTimeProvider.UtcNow
                };

                return ApiResponse<LeaveReportDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating leave report");
                return ApiResponse<LeaveReportDto>.ErrorResult("An error occurred while generating the leave report");
            }
        }

        private async Task<UserLeaveBalance> GetOrCreateUserLeaveBalance(Guid userId, Guid leaveTypeId)
        {
            var currentYear = DateTime.UtcNow.Year;
            var leaveBalance = await _context.UserLeaveBalances
                .FirstOrDefaultAsync(ulb => ulb.UserId == userId && 
                                           ulb.LeaveTypeId == leaveTypeId && 
                                           ulb.Year == currentYear);

            if (leaveBalance == null)
            {
                // Get default allocation from tenant settings or leave type
                var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
                var defaultAllocation = leaveType?.MaxDaysPerYear ?? 21;

                leaveBalance = new UserLeaveBalance
                {
                    UserId = userId,
                    LeaveTypeId = leaveTypeId,
                    Year = currentYear,
                    AllocatedDays = defaultAllocation,
                    UsedDays = 0,
                    RemainingDays = defaultAllocation,
                    CarryForwardDays = 0,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.UserLeaveBalances.Add(leaveBalance);
                await _context.SaveChangesAsync();
            }

            return leaveBalance;
        }

        private int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            int workingDays = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

        private async Task<LeaveRequestDto> MapToLeaveRequestDto(LeaveRequest leaveRequest)
        {
            var approvals = await _context.LeaveApprovals
                .Include(la => la.Approver)
                .Where(la => la.LeaveRequestId == leaveRequest.Id)
                .ToListAsync();

            return new LeaveRequestDto
            {
                Id = leaveRequest.Id,
                UserId = leaveRequest.UserId,
                UserName = $"{leaveRequest.User?.FirstName} {leaveRequest.User?.LastName}",
                LeaveTypeName = leaveRequest.LeaveType?.Name ?? "Unknown",
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                TotalDays = leaveRequest.TotalDays,
                Reason = leaveRequest.Reason,
                Status = leaveRequest.Status.ToString(),
                RequestedAt = leaveRequest.RequestedAt,
                ApprovedByName = leaveRequest.ApprovedByUser != null ? $"{leaveRequest.ApprovedByUser.FirstName} {leaveRequest.ApprovedByUser.LastName}" : null,
                ApprovedAt = leaveRequest.ApprovedAt,
                ApprovalNotes = leaveRequest.ApprovalNotes,
                IsEmergency = leaveRequest.IsEmergency,
                AttachmentUrls = !string.IsNullOrEmpty(leaveRequest.AttachmentUrls) ? 
                    System.Text.Json.JsonSerializer.Deserialize<IEnumerable<string>>(leaveRequest.AttachmentUrls) : null
            };
        }

        private PermissionRequestDto MapToPermissionRequestDto(PermissionRequest permissionRequest)
        {
            return new PermissionRequestDto
            {
                Id = permissionRequest.Id,
                UserId = permissionRequest.UserId,
                UserName = $"{permissionRequest.User?.FirstName} {permissionRequest.User?.LastName}",
                StartTime = permissionRequest.StartTime,
                EndTime = permissionRequest.EndTime,
                DurationMinutes = permissionRequest.DurationMinutes,
                Reason = permissionRequest.Reason,
                Status = permissionRequest.Status.ToString(),
                RequestedAt = permissionRequest.RequestedAt,
                ApprovedAt = permissionRequest.ApprovedAt,
                ApprovedByName = permissionRequest.ApprovedByUser != null ? $"{permissionRequest.ApprovedByUser.FirstName} {permissionRequest.ApprovedByUser.LastName}" : null,
                IsEmergency = permissionRequest.IsEmergency
            };
        }
    }
}

