using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Application.Services;
using AppDTOs = AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Api.Services
{
    public class LeaveManagementService : ILeaveManagementService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<LeaveManagementService> _logger;
        private readonly ITenantContext _tenantContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public LeaveManagementService(
            AttendancePlatformDbContext context,
            ILogger<LeaveManagementService> logger,
            ITenantContext tenantContext,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<ApiResponse<AppDTOs.LeaveRequestDto>> CreateLeaveRequestAsync(AppDTOs.CreateLeaveRequestDto request, Guid userId)
        {
            try
            {
                var leaveRequest = new LeaveRequest
                {
                    UserId = userId,
                    LeaveTypeId = request.LeaveTypeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Reason = request.Reason,
                    Status = LeaveRequestStatus.Pending,
                    RequestedAt = _dateTimeProvider.UtcNow,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.LeaveRequests.Add(leaveRequest);
                await _context.SaveChangesAsync();

                var dto = await MapToLeaveRequestDto(leaveRequest);
                return ApiResponse<AppDTOs.LeaveRequestDto>.SuccessResult(dto, "Leave request created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request for user: {UserId}", userId);
                return ApiResponse<AppDTOs.LeaveRequestDto>.ErrorResult("An error occurred while creating leave request");
            }
        }

        public async Task<ApiResponse<AppDTOs.PermissionRequestDto>> CreatePermissionRequestAsync(AppDTOs.CreatePermissionRequestDto request, Guid userId)
        {
            try
            {
                var permissionRequest = new PermissionRequest
                {
                    UserId = userId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Reason = request.Reason,
                    Status = PermissionRequestStatus.Pending,
                    RequestedAt = _dateTimeProvider.UtcNow,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.PermissionRequests.Add(permissionRequest);
                await _context.SaveChangesAsync();

                var dto = await MapToPermissionRequestDto(permissionRequest);
                return ApiResponse<AppDTOs.PermissionRequestDto>.SuccessResult(dto, "Permission request created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission request for user: {UserId}", userId);
                return ApiResponse<AppDTOs.PermissionRequestDto>.ErrorResult("An error occurred while creating permission request");
            }
        }

        public async Task<ApiResponse<bool>> ApproveLeaveRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests.FindAsync(requestId);
                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Leave request not found");
                }

                leaveRequest.Status = approval.IsApproved ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
                leaveRequest.ApprovedAt = _dateTimeProvider.UtcNow;
                leaveRequest.ApprovedBy = approverId;
                leaveRequest.ApprovalNotes = approval.Comments;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Leave request processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving leave request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while processing leave request");
            }
        }

        public async Task<ApiResponse<bool>> ApprovePermissionRequestAsync(Guid requestId, ApprovalDto approval, Guid approverId)
        {
            try
            {
                var permissionRequest = await _context.PermissionRequests.FindAsync(requestId);
                if (permissionRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Permission request not found");
                }

                permissionRequest.Status = approval.IsApproved ? PermissionRequestStatus.Approved : PermissionRequestStatus.Rejected;
                permissionRequest.ApprovedAt = _dateTimeProvider.UtcNow;
                permissionRequest.ApprovedBy = approverId;
                permissionRequest.ApprovalNotes = approval.Comments;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Permission request processed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving permission request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while processing permission request");
            }
        }

        public async Task<ApiResponse<bool>> CancelLeaveRequestAsync(Guid requestId, Guid userId)
        {
            try
            {
                var leaveRequest = await _context.LeaveRequests
                    .FirstOrDefaultAsync(lr => lr.Id == requestId && lr.UserId == userId);

                if (leaveRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Leave request not found");
                }

                if (leaveRequest.Status != LeaveRequestStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Only pending requests can be cancelled");
                }

                leaveRequest.Status = LeaveRequestStatus.Cancelled;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Leave request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while cancelling leave request");
            }
        }

        public async Task<ApiResponse<bool>> CancelPermissionRequestAsync(Guid requestId, Guid userId)
        {
            try
            {
                var permissionRequest = await _context.PermissionRequests
                    .FirstOrDefaultAsync(pr => pr.Id == requestId && pr.UserId == userId);

                if (permissionRequest == null)
                {
                    return ApiResponse<bool>.ErrorResult("Permission request not found");
                }

                if (permissionRequest.Status != PermissionRequestStatus.Pending)
                {
                    return ApiResponse<bool>.ErrorResult("Only pending requests can be cancelled");
                }

                permissionRequest.Status = PermissionRequestStatus.Cancelled;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Permission request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling permission request: {RequestId}", requestId);
                return ApiResponse<bool>.ErrorResult("An error occurred while cancelling permission request");
            }
        }

        public async Task<ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>> GetUserLeaveRequestsAsync(Guid userId, int page, int pageSize)
        {
            try
            {
                var requests = await _context.LeaveRequests
                    .Where(lr => lr.UserId == userId)
                    .OrderByDescending(lr => lr.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<AppDTOs.LeaveRequestDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToLeaveRequestDto(request));
                }

                return ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave requests for user: {UserId}", userId);
                return ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>.ErrorResult("An error occurred while retrieving leave requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>> GetUserPermissionRequestsAsync(Guid userId, int page, int pageSize)
        {
            try
            {
                var requests = await _context.PermissionRequests
                    .Where(pr => pr.UserId == userId)
                    .OrderByDescending(pr => pr.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<AppDTOs.PermissionRequestDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToPermissionRequestDto(request));
                }

                return ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission requests for user: {UserId}", userId);
                return ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>.ErrorResult("An error occurred while retrieving permission requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>> GetPendingLeaveRequestsAsync(Guid managerId, int page, int pageSize)
        {
            try
            {
                var requests = await _context.LeaveRequests
                    .Where(lr => lr.Status == LeaveRequestStatus.Pending)
                    .OrderByDescending(lr => lr.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<AppDTOs.LeaveRequestDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToLeaveRequestDto(request));
                }

                return ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending leave requests");
                return ApiResponse<IEnumerable<AppDTOs.LeaveRequestDto>>.ErrorResult("An error occurred while retrieving pending leave requests");
            }
        }

        public async Task<ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>> GetPendingPermissionRequestsAsync(Guid managerId, int page, int pageSize)
        {
            try
            {
                var requests = await _context.PermissionRequests
                    .Where(pr => pr.Status == PermissionRequestStatus.Pending)
                    .OrderByDescending(pr => pr.RequestedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var dtos = new List<AppDTOs.PermissionRequestDto>();
                foreach (var request in requests)
                {
                    dtos.Add(await MapToPermissionRequestDto(request));
                }

                return ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending permission requests");
                return ApiResponse<IEnumerable<AppDTOs.PermissionRequestDto>>.ErrorResult("An error occurred while retrieving pending permission requests");
            }
        }

        public async Task<ApiResponse<AppDTOs.UserLeaveBalanceDto>> GetUserLeaveBalanceAsync(Guid userId)
        {
            try
            {
                var balance = await _context.UserLeaveBalances
                    .FirstOrDefaultAsync(ulb => ulb.UserId == userId);

                if (balance == null)
                {
                    return ApiResponse<AppDTOs.UserLeaveBalanceDto>.ErrorResult("Leave balance not found");
                }

                var dto = new AppDTOs.UserLeaveBalanceDto
                {
                    UserId = balance.UserId,
                    AnnualLeaveBalance = balance.AllocatedDays,
                    SickLeaveBalance = balance.UsedDays,
                    PersonalLeaveBalance = balance.RemainingDays,
                    MaternityLeaveBalance = balance.CarriedOverDays,
                    PaternityLeaveBalance = 0
                };

                return ApiResponse<AppDTOs.UserLeaveBalanceDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave balance for user: {UserId}", userId);
                return ApiResponse<AppDTOs.UserLeaveBalanceDto>.ErrorResult("An error occurred while retrieving leave balance");
            }
        }

        public async Task<ApiResponse<IEnumerable<AppDTOs.LeaveTypeDto>>> GetLeaveTypesAsync()
        {
            try
            {
                var leaveTypes = await _context.LeaveTypes.ToListAsync();
                var dtos = leaveTypes.Select(lt => new AppDTOs.LeaveTypeDto
                {
                    Id = lt.Id,
                    Name = lt.Name,
                    Description = lt.Description,
                    MaxDaysPerYear = lt.MaxDaysPerYear,
                    IsActive = lt.IsActive
                }).ToList();

                return ApiResponse<IEnumerable<AppDTOs.LeaveTypeDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave types");
                return ApiResponse<IEnumerable<AppDTOs.LeaveTypeDto>>.ErrorResult("An error occurred while retrieving leave types");
            }
        }

        public async Task<ApiResponse<AppDTOs.LeaveCalendarDto>> GetLeaveCalendarAsync(DateTime startDate, DateTime endDate, Guid? userId)
        {
            try
            {
                var query = _context.LeaveRequests
                    .Where(lr => lr.Status == LeaveRequestStatus.Approved &&
                                lr.StartDate <= endDate && lr.EndDate >= startDate);

                if (userId.HasValue)
                {
                    query = query.Where(lr => lr.UserId == userId.Value);
                }

                var leaveRequests = await query.ToListAsync();

                var calendar = new AppDTOs.LeaveCalendarDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    LeaveRequests = leaveRequests.Select(lr => new AppDTOs.LeaveRequestDto
                    {
                        Id = lr.Id,
                        UserId = lr.UserId,
                        LeaveTypeId = lr.LeaveTypeId,
                        StartDate = lr.StartDate,
                        EndDate = lr.EndDate,
                        Reason = lr.Reason,
                        Status = lr.Status.ToString(),
                        RequestDate = lr.RequestedAt
                    }).ToList()
                };

                return ApiResponse<AppDTOs.LeaveCalendarDto>.SuccessResult(calendar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leave calendar");
                return ApiResponse<AppDTOs.LeaveCalendarDto>.ErrorResult("An error occurred while retrieving leave calendar");
            }
        }

        public async Task<ApiResponse<bool>> UpdateLeaveBalanceAsync(Guid userId, AppDTOs.UpdateLeaveBalanceDto request)
        {
            try
            {
                var balance = await _context.UserLeaveBalances
                    .FirstOrDefaultAsync(ulb => ulb.UserId == userId);

                if (balance == null)
                {
                    balance = new UserLeaveBalance
                    {
                        UserId = userId,
                        TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                    };
                    _context.UserLeaveBalances.Add(balance);
                }

                balance.AllocatedDays = request.AnnualLeaveBalance;
                balance.UsedDays = request.SickLeaveBalance;
                balance.RemainingDays = request.PersonalLeaveBalance;
                balance.CarriedOverDays = request.MaternityLeaveBalance;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Leave balance updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leave balance for user: {UserId}", userId);
                return ApiResponse<bool>.ErrorResult("An error occurred while updating leave balance");
            }
        }

        public async Task<ApiResponse<AppDTOs.LeaveReportDto>> GenerateLeaveReportAsync(AppDTOs.LeaveReportRequestDto request)
        {
            try
            {
                var query = _context.LeaveRequests
                    .Where(lr => lr.StartDate >= request.StartDate && lr.EndDate <= request.EndDate);

                if (request.UserId.HasValue)
                {
                    query = query.Where(lr => lr.UserId == request.UserId.Value);
                }

                if (request.LeaveTypeId.HasValue)
                {
                    query = query.Where(lr => lr.LeaveTypeId == request.LeaveTypeId.Value);
                }

                var leaveRequests = await query.ToListAsync();

                var report = new AppDTOs.LeaveReportDto
                {
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    TotalRequests = leaveRequests.Count,
                    ApprovedRequests = leaveRequests.Count(lr => lr.Status == LeaveRequestStatus.Approved),
                    PendingRequests = leaveRequests.Count(lr => lr.Status == LeaveRequestStatus.Pending),
                    RejectedRequests = leaveRequests.Count(lr => lr.Status == LeaveRequestStatus.Rejected),
                    LeaveRequests = leaveRequests.Select(lr => new AppDTOs.LeaveRequestDto
                    {
                        Id = lr.Id,
                        UserId = lr.UserId,
                        LeaveTypeId = lr.LeaveTypeId,
                        StartDate = lr.StartDate,
                        EndDate = lr.EndDate,
                        Reason = lr.Reason,
                        Status = lr.Status.ToString(),
                        RequestDate = lr.RequestedAt
                    }).ToList()
                };

                return ApiResponse<AppDTOs.LeaveReportDto>.SuccessResult(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating leave report");
                return ApiResponse<AppDTOs.LeaveReportDto>.ErrorResult("An error occurred while generating leave report");
            }
        }

        private async Task<AppDTOs.LeaveRequestDto> MapToLeaveRequestDto(LeaveRequest request)
        {
            return new AppDTOs.LeaveRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                LeaveTypeId = request.LeaveTypeId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Reason = request.Reason,
                Status = request.Status.ToString(),
                RequestDate = request.RequestedAt,
                ApprovalDate = request.ApprovedAt,
                ApprovedById = request.ApprovedBy,
                ApprovalComments = request.ApprovalNotes
            };
        }

        private async Task<AppDTOs.PermissionRequestDto> MapToPermissionRequestDto(PermissionRequest request)
        {
            return new AppDTOs.PermissionRequestDto
            {
                Id = request.Id,
                UserId = request.UserId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Reason = request.Reason,
                Status = request.Status.ToString(),
                RequestDate = request.RequestedAt,
                ApprovalDate = request.ApprovedAt,
                ApprovedById = request.ApprovedBy,
                ApprovalComments = request.ApprovalNotes
            };
        }
    }
}
