using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Workflow.Api.Services
{
    public class ShiftSchedulingService : IShiftSchedulingService
    {
        private readonly IRepository<ShiftTemplate> _shiftTemplateRepository;
        private readonly IRepository<Shift> _shiftRepository;
        private readonly IRepository<ShiftAssignment> _assignmentRepository;
        private readonly IRepository<ShiftSwapRequest> _swapRequestRepository;
        private readonly IRepository<ShiftConflict> _conflictRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<ShiftSchedulingService> _logger;
        private readonly ICacheService _cacheService;

        public ShiftSchedulingService(
            IRepository<ShiftTemplate> shiftTemplateRepository,
            IRepository<Shift> shiftRepository,
            IRepository<ShiftAssignment> assignmentRepository,
            IRepository<ShiftSwapRequest> swapRequestRepository,
            IRepository<ShiftConflict> conflictRepository,
            IRepository<User> userRepository,
            ILogger<ShiftSchedulingService> logger,
            ICacheService cacheService)
        {
            _shiftTemplateRepository = shiftTemplateRepository;
            _shiftRepository = shiftRepository;
            _assignmentRepository = assignmentRepository;
            _swapRequestRepository = swapRequestRepository;
            _conflictRepository = conflictRepository;
            _userRepository = userRepository;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<ShiftTemplate> CreateShiftTemplateAsync(CreateShiftTemplateRequest request)
        {
            try
            {
                var template = new ShiftTemplate
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    BreakDuration = request.BreakDuration,
                    WorkingDays = request.WorkingDays,
                    Type = request.Type,
                    MaxEmployees = request.MaxEmployees,
                    MinEmployees = request.MinEmployees,
                    Department = request.Department,
                    Location = request.Location,
                    HourlyRate = request.HourlyRate,
                    OvertimeMultiplier = request.OvertimeMultiplier,
                    AllowOvertime = request.AllowOvertime,
                    RequiresApproval = request.RequiresApproval,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _shiftTemplateRepository.AddAsync(template);
                _logger.LogInformation("Shift template {TemplateId} created successfully", template.Id);

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift template");
                throw;
            }
        }

        public async Task<ShiftTemplate> UpdateShiftTemplateAsync(string templateId, UpdateShiftTemplateRequest request)
        {
            try
            {
                var template = await _shiftTemplateRepository.GetByIdAsync(templateId);
                if (template == null)
                {
                    throw new ArgumentException("Shift template not found");
                }

                if (request.Name != null) template.Name = request.Name;
                if (request.Description != null) template.Description = request.Description;
                if (request.StartTime.HasValue) template.StartTime = request.StartTime.Value;
                if (request.EndTime.HasValue) template.EndTime = request.EndTime.Value;
                if (request.BreakDuration.HasValue) template.BreakDuration = request.BreakDuration;
                if (request.WorkingDays != null) template.WorkingDays = request.WorkingDays;
                if (request.Type.HasValue) template.Type = request.Type.Value;
                if (request.MaxEmployees.HasValue) template.MaxEmployees = request.MaxEmployees.Value;
                if (request.MinEmployees.HasValue) template.MinEmployees = request.MinEmployees.Value;
                if (request.Department != null) template.Department = request.Department;
                if (request.Location != null) template.Location = request.Location;
                if (request.HourlyRate.HasValue) template.HourlyRate = request.HourlyRate;
                if (request.OvertimeMultiplier.HasValue) template.OvertimeMultiplier = request.OvertimeMultiplier;
                if (request.AllowOvertime.HasValue) template.AllowOvertime = request.AllowOvertime.Value;
                if (request.RequiresApproval.HasValue) template.RequiresApproval = request.RequiresApproval.Value;
                if (request.IsActive.HasValue) template.IsActive = request.IsActive.Value;

                template.UpdatedAt = DateTime.UtcNow;
                await _shiftTemplateRepository.UpdateAsync(template);

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<bool> DeleteShiftTemplateAsync(string templateId)
        {
            try
            {
                var template = await _shiftTemplateRepository.GetByIdAsync(templateId);
                if (template == null)
                {
                    return false;
                }

                await _shiftTemplateRepository.DeleteAsync(template);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shift template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<List<ShiftTemplate>> GetShiftTemplatesAsync(string tenantId)
        {
            try
            {
                return await _shiftTemplateRepository.Query()
                    .Where(t => t.TenantId == tenantId)
                    .OrderBy(t => t.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift templates for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ShiftTemplate?> GetShiftTemplateAsync(string templateId)
        {
            try
            {
                return await _shiftTemplateRepository.GetByIdAsync(templateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<Shift> CreateShiftAsync(CreateShiftRequest request)
        {
            try
            {
                var shift = new Shift
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = request.TenantId,
                    TemplateId = request.TemplateId,
                    Name = request.Name,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    BreakDuration = request.BreakDuration,
                    WorkingDays = request.WorkingDays,
                    Type = request.Type,
                    MaxEmployees = request.MaxEmployees,
                    MinEmployees = request.MinEmployees,
                    Department = request.Department,
                    Location = request.Location,
                    HourlyRate = request.HourlyRate,
                    OvertimeMultiplier = request.OvertimeMultiplier,
                    AllowOvertime = request.AllowOvertime,
                    RequiresApproval = request.RequiresApproval,
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = request.EffectiveTo,
                    Status = ShiftStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                await _shiftRepository.AddAsync(shift);
                _logger.LogInformation("Shift {ShiftId} created successfully", shift.Id);

                return shift;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift");
                throw;
            }
        }

        public async Task<Shift> UpdateShiftAsync(string shiftId, UpdateShiftRequest request)
        {
            try
            {
                var shift = await _shiftRepository.GetByIdAsync(shiftId);
                if (shift == null)
                {
                    throw new ArgumentException("Shift not found");
                }

                if (request.Name != null) shift.Name = request.Name;
                if (request.Description != null) shift.Description = request.Description;
                if (request.StartTime.HasValue) shift.StartTime = request.StartTime.Value;
                if (request.EndTime.HasValue) shift.EndTime = request.EndTime.Value;
                if (request.BreakDuration.HasValue) shift.BreakDuration = request.BreakDuration;
                if (request.WorkingDays != null) shift.WorkingDays = request.WorkingDays;
                if (request.Type.HasValue) shift.Type = request.Type.Value;
                if (request.MaxEmployees.HasValue) shift.MaxEmployees = request.MaxEmployees.Value;
                if (request.MinEmployees.HasValue) shift.MinEmployees = request.MinEmployees.Value;
                if (request.Department != null) shift.Department = request.Department;
                if (request.Location != null) shift.Location = request.Location;
                if (request.HourlyRate.HasValue) shift.HourlyRate = request.HourlyRate;
                if (request.OvertimeMultiplier.HasValue) shift.OvertimeMultiplier = request.OvertimeMultiplier;
                if (request.AllowOvertime.HasValue) shift.AllowOvertime = request.AllowOvertime.Value;
                if (request.RequiresApproval.HasValue) shift.RequiresApproval = request.RequiresApproval.Value;
                if (request.EffectiveFrom.HasValue) shift.EffectiveFrom = request.EffectiveFrom.Value;
                if (request.EffectiveTo.HasValue) shift.EffectiveTo = request.EffectiveTo;
                if (request.Status.HasValue) shift.Status = request.Status.Value;

                shift.UpdatedAt = DateTime.UtcNow;
                await _shiftRepository.UpdateAsync(shift);

                return shift;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift {ShiftId}", shiftId);
                throw;
            }
        }

        public async Task<bool> DeleteShiftAsync(string shiftId)
        {
            try
            {
                var shift = await _shiftRepository.GetByIdAsync(shiftId);
                if (shift == null)
                {
                    return false;
                }

                await _shiftRepository.DeleteAsync(shift);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shift {ShiftId}", shiftId);
                throw;
            }
        }

        public async Task<List<Shift>> GetShiftsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _shiftRepository.Query()
                    .Where(s => s.TenantId == tenantId);

                if (startDate.HasValue)
                {
                    query = query.Where(s => s.EffectiveFrom >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(s => s.EffectiveTo == null || s.EffectiveTo <= endDate.Value);
                }

                return await query.OrderBy(s => s.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shifts for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<Shift?> GetShiftAsync(string shiftId)
        {
            try
            {
                return await _shiftRepository.GetByIdAsync(shiftId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift {ShiftId}", shiftId);
                throw;
            }
        }

        public async Task<ShiftAssignment> AssignShiftAsync(AssignShiftRequest request)
        {
            try
            {
                var assignment = new ShiftAssignment
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = request.TenantId,
                    ShiftId = request.ShiftId,
                    UserId = request.UserId,
                    ScheduledDate = request.ScheduledDate,
                    Status = ShiftAssignmentStatus.Scheduled,
                    Notes = request.Notes,
                    AssignedBy = request.AssignedBy,
                    IsRecurring = request.IsRecurring,
                    RecurrencePattern = request.RecurrencePattern,
                    CreatedAt = DateTime.UtcNow
                };

                await _assignmentRepository.AddAsync(assignment);
                _logger.LogInformation("Shift assignment {AssignmentId} created successfully", assignment.Id);

                await DetectAndCreateConflictsAsync(assignment);

                return assignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning shift");
                throw;
            }
        }

        public async Task<bool> UnassignShiftAsync(string assignmentId)
        {
            try
            {
                var assignment = await _assignmentRepository.GetByIdAsync(assignmentId);
                if (assignment == null)
                {
                    return false;
                }

                await _assignmentRepository.DeleteAsync(assignment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning shift {AssignmentId}", assignmentId);
                throw;
            }
        }

        public async Task<List<ShiftAssignment>> GetShiftAssignmentsAsync(string tenantId, string? userId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _assignmentRepository.Query()
                    .Where(a => a.TenantId == tenantId)
                    .Include(a => a.Shift)
                    .Include(a => a.User);

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(a => a.UserId == userId);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.ScheduledDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.ScheduledDate <= endDate.Value);
                }

                return await query.OrderBy(a => a.ScheduledDate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift assignments for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ShiftAssignment?> GetShiftAssignmentAsync(string assignmentId)
        {
            try
            {
                return await _assignmentRepository.Query()
                    .Include(a => a.Shift)
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == assignmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift assignment {AssignmentId}", assignmentId);
                throw;
            }
        }

        public async Task<ShiftSwapRequest> CreateSwapRequestAsync(CreateSwapRequestRequest request)
        {
            try
            {
                var swapRequest = new ShiftSwapRequest
                {
                    Id = Guid.NewGuid().ToString(),
                    TenantId = request.TenantId,
                    RequesterId = request.RequesterId,
                    OriginalAssignmentId = request.OriginalAssignmentId,
                    TargetUserId = request.TargetUserId,
                    TargetAssignmentId = request.TargetAssignmentId,
                    Reason = request.Reason,
                    Type = request.Type,
                    Status = SwapRequestStatus.Pending,
                    RequestedDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                await _swapRequestRepository.AddAsync(swapRequest);
                _logger.LogInformation("Swap request {RequestId} created successfully", swapRequest.Id);

                return swapRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating swap request");
                throw;
            }
        }

        public async Task<ShiftSwapRequest> ProcessSwapRequestAsync(string requestId, ProcessSwapRequestRequest request)
        {
            try
            {
                var swapRequest = await _swapRequestRepository.GetByIdAsync(requestId);
                if (swapRequest == null)
                {
                    throw new ArgumentException("Swap request not found");
                }

                swapRequest.Status = request.Status;
                swapRequest.ResponseNotes = request.Notes;
                swapRequest.RespondedBy = request.ProcessedBy;
                swapRequest.ResponseDate = DateTime.UtcNow;
                swapRequest.UpdatedAt = DateTime.UtcNow;

                await _swapRequestRepository.UpdateAsync(swapRequest);

                if (request.Status == SwapRequestStatus.Accepted)
                {
                    await ExecuteShiftSwapAsync(swapRequest);
                }

                return swapRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing swap request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<List<ShiftSwapRequest>> GetSwapRequestsAsync(string tenantId, string? userId = null, SwapRequestStatus? status = null)
        {
            try
            {
                var query = _swapRequestRepository.Query()
                    .Where(r => r.TenantId == tenantId)
                    .Include(r => r.Requester)
                    .Include(r => r.OriginalAssignment)
                    .Include(r => r.TargetUser);

                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(r => r.RequesterId == userId || r.TargetUserId == userId);
                }

                if (status.HasValue)
                {
                    query = query.Where(r => r.Status == status.Value);
                }

                return await query.OrderByDescending(r => r.RequestedDate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting swap requests for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ShiftSwapRequest?> GetSwapRequestAsync(string requestId)
        {
            try
            {
                return await _swapRequestRepository.Query()
                    .Include(r => r.Requester)
                    .Include(r => r.OriginalAssignment)
                    .Include(r => r.TargetUser)
                    .Include(r => r.TargetAssignment)
                    .FirstOrDefaultAsync(r => r.Id == requestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting swap request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<List<ShiftConflict>> DetectConflictsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var conflicts = new List<ShiftConflict>();
                var assignments = await GetShiftAssignmentsAsync(tenantId, null, startDate, endDate);

                foreach (var assignment in assignments)
                {
                    var detectedConflicts = await DetectConflictsForAssignmentAsync(assignment);
                    conflicts.AddRange(detectedConflicts);
                }

                return conflicts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting conflicts for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ShiftConflict> ResolveConflictAsync(string conflictId, ResolveConflictRequest request)
        {
            try
            {
                var conflict = await _conflictRepository.GetByIdAsync(conflictId);
                if (conflict == null)
                {
                    throw new ArgumentException("Conflict not found");
                }

                conflict.Status = request.Status;
                conflict.ResolvedBy = request.ResolvedBy;
                conflict.ResolutionNotes = request.ResolutionNotes;
                conflict.ResolvedAt = DateTime.UtcNow;
                conflict.UpdatedAt = DateTime.UtcNow;

                await _conflictRepository.UpdateAsync(conflict);

                return conflict;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving conflict {ConflictId}", conflictId);
                throw;
            }
        }

        public async Task<List<ShiftConflict>> GetConflictsAsync(string tenantId, ConflictStatus? status = null)
        {
            try
            {
                var query = _conflictRepository.Query()
                    .Where(c => c.TenantId == tenantId)
                    .Include(c => c.User)
                    .Include(c => c.Assignment);

                if (status.HasValue)
                {
                    query = query.Where(c => c.Status == status.Value);
                }

                return await query.OrderByDescending(c => c.DetectedAt).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conflicts for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ShiftScheduleDto> GenerateScheduleAsync(GenerateScheduleRequest request)
        {
            try
            {
                var assignments = await GetShiftAssignmentsAsync(request.TenantId, null, request.StartDate, request.EndDate);
                var conflicts = await DetectConflictsAsync(request.TenantId, request.StartDate, request.EndDate);
                var coverage = await CalculateCoverageAsync(request.TenantId, request.StartDate, request.EndDate);

                var assignmentDtos = assignments.Select(a => new ShiftAssignmentDto
                {
                    Id = a.Id,
                    ShiftId = a.ShiftId,
                    ShiftName = a.Shift?.Name ?? "",
                    UserId = a.UserId,
                    UserName = $"{a.User?.FirstName} {a.User?.LastName}",
                    ScheduledDate = a.ScheduledDate,
                    StartTime = a.Shift?.StartTime ?? TimeSpan.Zero,
                    EndTime = a.Shift?.EndTime ?? TimeSpan.Zero,
                    Status = a.Status,
                    Department = a.Shift?.Department,
                    Location = a.Shift?.Location,
                    Notes = a.Notes
                }).ToList();

                var conflictDtos = conflicts.Select(c => new ShiftConflictDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserName = $"{c.User?.FirstName} {c.User?.LastName}",
                    AssignmentId = c.AssignmentId,
                    Type = c.Type,
                    Description = c.Description,
                    Severity = c.Severity,
                    Status = c.Status,
                    DetectedAt = c.DetectedAt
                }).ToList();

                return new ShiftScheduleDto
                {
                    TenantId = request.TenantId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Assignments = assignmentDtos,
                    Conflicts = conflictDtos,
                    Coverage = coverage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating schedule for tenant {TenantId}", request.TenantId);
                throw;
            }
        }

        public async Task<List<ShiftAssignment>> AutoAssignShiftsAsync(AutoAssignRequest request)
        {
            try
            {
                var shifts = await _shiftRepository.Query()
                    .Where(s => request.ShiftIds.Contains(s.Id))
                    .ToListAsync();

                var availableUsers = await _userRepository.Query()
                    .Where(u => u.TenantId == request.TenantId && u.IsActive)
                    .ToListAsync();

                if (request.PreferredUserIds != null && request.PreferredUserIds.Any())
                {
                    availableUsers = availableUsers.Where(u => request.PreferredUserIds.Contains(u.Id)).ToList();
                }

                var assignments = new List<ShiftAssignment>();

                foreach (var shift in shifts)
                {
                    var assignedUsers = await GetOptimalUsersForShift(shift, availableUsers, request);
                    
                    foreach (var user in assignedUsers.Take(shift.MaxEmployees))
                    {
                        var assignment = new ShiftAssignment
                        {
                            Id = Guid.NewGuid().ToString(),
                            TenantId = request.TenantId,
                            ShiftId = shift.Id,
                            UserId = user.Id,
                            ScheduledDate = request.StartDate,
                            Status = ShiftAssignmentStatus.Scheduled,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _assignmentRepository.AddAsync(assignment);
                        assignments.Add(assignment);
                    }
                }

                return assignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-assigning shifts");
                throw;
            }
        }

        public async Task<ShiftCoverageReport> GetCoverageReportAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            try
            {
                return await CalculateCoverageAsync(tenantId, startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coverage report for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<ShiftMetrics>> GetShiftMetricsAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var assignments = await GetShiftAssignmentsAsync(tenantId, null, startDate, endDate);
                var swapRequests = await GetSwapRequestsAsync(tenantId);
                var conflicts = await GetConflictsAsync(tenantId);

                var metrics = new List<ShiftMetrics>();
                var periodDays = (endDate - startDate).Days + 1;

                for (int i = 0; i < periodDays; i++)
                {
                    var currentDate = startDate.AddDays(i);
                    var dayAssignments = assignments.Where(a => a.ScheduledDate.Date == currentDate.Date).ToList();
                    
                    var dayMetrics = new ShiftMetrics
                    {
                        Period = currentDate.ToString("yyyy-MM-dd"),
                        TotalShifts = dayAssignments.Count,
                        CompletedShifts = dayAssignments.Count(a => a.Status == ShiftAssignmentStatus.Completed),
                        MissedShifts = dayAssignments.Count(a => a.Status == ShiftAssignmentStatus.NoShow),
                        AttendanceRate = dayAssignments.Count > 0 ? 
                            (double)dayAssignments.Count(a => a.Status == ShiftAssignmentStatus.Completed) / dayAssignments.Count * 100 : 0,
                        AverageHoursPerShift = CalculateAverageHours(dayAssignments),
                        OvertimeHours = CalculateOvertimeHours(dayAssignments),
                        SwapRequests = swapRequests.Count(r => r.RequestedDate.Date == currentDate.Date),
                        ConflictsDetected = conflicts.Count(c => c.DetectedAt.Date == currentDate.Date),
                        ConflictsResolved = conflicts.Count(c => c.ResolvedAt?.Date == currentDate.Date)
                    };

                    metrics.Add(dayMetrics);
                }

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift metrics for tenant {TenantId}", tenantId);
                throw;
            }
        }

        private async Task DetectAndCreateConflictsAsync(ShiftAssignment assignment)
        {
            var conflicts = await DetectConflictsForAssignmentAsync(assignment);
            
            foreach (var conflict in conflicts)
            {
                await _conflictRepository.AddAsync(conflict);
            }
        }

        private async Task<List<ShiftConflict>> DetectConflictsForAssignmentAsync(ShiftAssignment assignment)
        {
            var conflicts = new List<ShiftConflict>();

            var userAssignments = await _assignmentRepository.Query()
                .Where(a => a.UserId == assignment.UserId && 
                           a.ScheduledDate.Date == assignment.ScheduledDate.Date &&
                           a.Id != assignment.Id)
                .Include(a => a.Shift)
                .ToListAsync();

            foreach (var existingAssignment in userAssignments)
            {
                if (existingAssignment.Shift != null && assignment.Shift != null)
                {
                    if (IsTimeOverlapping(assignment.Shift.StartTime, assignment.Shift.EndTime,
                                        existingAssignment.Shift.StartTime, existingAssignment.Shift.EndTime))
                    {
                        conflicts.Add(new ShiftConflict
                        {
                            Id = Guid.NewGuid().ToString(),
                            TenantId = assignment.TenantId,
                            UserId = assignment.UserId,
                            AssignmentId = assignment.Id,
                            Type = ConflictType.OverlappingShifts,
                            Description = $"Overlapping shift with assignment {existingAssignment.Id}",
                            Severity = ConflictSeverity.High,
                            Status = ConflictStatus.Open,
                            DetectedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }

            return conflicts;
        }

        private bool IsTimeOverlapping(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
        {
            return start1 < end2 && start2 < end1;
        }

        private async Task ExecuteShiftSwapAsync(ShiftSwapRequest swapRequest)
        {
            if (swapRequest.TargetAssignmentId != null)
            {
                var originalAssignment = await _assignmentRepository.GetByIdAsync(swapRequest.OriginalAssignmentId);
                var targetAssignment = await _assignmentRepository.GetByIdAsync(swapRequest.TargetAssignmentId);

                if (originalAssignment != null && targetAssignment != null)
                {
                    var tempUserId = originalAssignment.UserId;
                    originalAssignment.UserId = targetAssignment.UserId;
                    targetAssignment.UserId = tempUserId;

                    await _assignmentRepository.UpdateAsync(originalAssignment);
                    await _assignmentRepository.UpdateAsync(targetAssignment);
                }
            }
        }

        private async Task<List<User>> GetOptimalUsersForShift(Shift shift, List<User> availableUsers, AutoAssignRequest request)
        {
            var optimalUsers = availableUsers.ToList();

            if (request.ConsiderAvailability)
            {
                optimalUsers = await FilterByAvailability(optimalUsers, shift);
            }

            if (request.ConsiderSkills && !string.IsNullOrEmpty(shift.Department))
            {
                optimalUsers = optimalUsers.Where(u => u.Department == shift.Department).ToList();
            }

            if (request.BalanceWorkload)
            {
                optimalUsers = await OrderByWorkload(optimalUsers, shift);
            }

            return optimalUsers;
        }

        private async Task<List<User>> FilterByAvailability(List<User> users, Shift shift)
        {
            var availableUsers = new List<User>();

            foreach (var user in users)
            {
                var existingAssignments = await _assignmentRepository.Query()
                    .Where(a => a.UserId == user.Id)
                    .Include(a => a.Shift)
                    .ToListAsync();

                bool hasConflict = existingAssignments.Any(a => 
                    a.Shift != null && 
                    IsTimeOverlapping(shift.StartTime, shift.EndTime, a.Shift.StartTime, a.Shift.EndTime));

                if (!hasConflict)
                {
                    availableUsers.Add(user);
                }
            }

            return availableUsers;
        }

        private async Task<List<User>> OrderByWorkload(List<User> users, Shift shift)
        {
            var userWorkloads = new Dictionary<string, int>();

            foreach (var user in users)
            {
                var assignmentCount = await _assignmentRepository.Query()
                    .CountAsync(a => a.UserId == user.Id && 
                               a.ScheduledDate >= DateTime.UtcNow.Date.AddDays(-7));
                userWorkloads[user.Id] = assignmentCount;
            }

            return users.OrderBy(u => userWorkloads.GetValueOrDefault(u.Id, 0)).ToList();
        }

        private async Task<ShiftCoverageReport> CalculateCoverageAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            var shifts = await _shiftRepository.Query()
                .Where(s => s.TenantId == tenantId && s.Status == ShiftStatus.Active)
                .ToListAsync();

            var assignments = await GetShiftAssignmentsAsync(tenantId, null, startDate, endDate);

            var totalShifts = shifts.Count;
            var assignedShifts = assignments.Count;
            var unassignedShifts = totalShifts - assignedShifts;
            var coveragePercentage = totalShifts > 0 ? (double)assignedShifts / totalShifts * 100 : 0;

            var departmentCoverage = shifts
                .Where(s => !string.IsNullOrEmpty(s.Department))
                .GroupBy(s => s.Department)
                .Select(g => new DepartmentCoverage
                {
                    Department = g.Key!,
                    TotalShifts = g.Count(),
                    AssignedShifts = assignments.Count(a => a.Shift?.Department == g.Key),
                    CoveragePercentage = g.Count() > 0 ? 
                        (double)assignments.Count(a => a.Shift?.Department == g.Key) / g.Count() * 100 : 0
                }).ToList();

            var locationCoverage = shifts
                .Where(s => !string.IsNullOrEmpty(s.Location))
                .GroupBy(s => s.Location)
                .Select(g => new LocationCoverage
                {
                    Location = g.Key!,
                    TotalShifts = g.Count(),
                    AssignedShifts = assignments.Count(a => a.Shift?.Location == g.Key),
                    CoveragePercentage = g.Count() > 0 ? 
                        (double)assignments.Count(a => a.Shift?.Location == g.Key) / g.Count() * 100 : 0
                }).ToList();

            return new ShiftCoverageReport
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalShifts = totalShifts,
                AssignedShifts = assignedShifts,
                UnassignedShifts = unassignedShifts,
                CoveragePercentage = coveragePercentage,
                DepartmentCoverage = departmentCoverage,
                LocationCoverage = locationCoverage
            };
        }

        private double CalculateAverageHours(List<ShiftAssignment> assignments)
        {
            if (!assignments.Any()) return 0;

            var totalHours = assignments
                .Where(a => a.Shift != null)
                .Sum(a => (a.Shift!.EndTime - a.Shift.StartTime).TotalHours);

            return totalHours / assignments.Count;
        }

        private double CalculateOvertimeHours(List<ShiftAssignment> assignments)
        {
            return assignments
                .Where(a => a.Shift != null && a.Shift.AllowOvertime)
                .Sum(a => Math.Max(0, (a.Shift!.EndTime - a.Shift.StartTime).TotalHours - 8));
        }
    }
}
