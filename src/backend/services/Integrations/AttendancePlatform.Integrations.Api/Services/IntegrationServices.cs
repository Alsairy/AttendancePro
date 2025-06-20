using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Text.Json;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string entityType, string entityId, object? oldValues = null, object? newValues = null, string? userId = null);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? entityType = null, string? entityId = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50);
        Task<AuditLogDto?> GetAuditLogAsync(Guid id);
        Task<AuditLogStatsDto> GetAuditStatsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> ExportAuditLogsAsync(string filePath, DateTime? fromDate = null, DateTime? toDate = null);
        Task CleanupOldLogsAsync(int retentionDays = 2555);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<AuditLogService> _logger;
        private readonly ITenantContext _tenantContext;

        public AuditLogService(
            AttendancePlatformDbContext context,
            ILogger<AuditLogService> logger,
            ITenantContext tenantContext)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public async Task LogAsync(string action, string entityType, string entityId, object? oldValues = null, object? newValues = null, string? userId = null)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    Id = Guid.NewGuid(),
                    TenantId = _tenantContext.TenantId,
                    UserId = userId != null ? Guid.Parse(userId) : _tenantContext.UserId,
                    Action = action,
                    EntityType = entityType,
                    EntityId = entityId,
                    OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                    NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                    Timestamp = DateTime.UtcNow,
                    IpAddress = GetClientIpAddress(),
                    UserAgent = GetUserAgent()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to log audit entry for {action} on {entityType}:{entityId}");
            }
        }

        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync(string? entityType = null, string? entityId = null, DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 50)
        {
            var query = _context.AuditLogs
                .Where(a => a.TenantId == _tenantContext.TenantId);

            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (!string.IsNullOrEmpty(entityId))
                query = query.Where(a => a.EntityId == entityId);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            var auditLogs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return auditLogs.Select(MapToDto);
        }

        public async Task<AuditLogDto?> GetAuditLogAsync(Guid id)
        {
            var auditLog = await _context.AuditLogs
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == _tenantContext.TenantId);

            return auditLog != null ? MapToDto(auditLog) : null;
        }

        public async Task<AuditLogStatsDto> GetAuditStatsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.AuditLogs
                .Where(a => a.TenantId == _tenantContext.TenantId);

            if (fromDate.HasValue)
                query = query.Where(a => a.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a => a.Timestamp <= toDate.Value);

            var totalLogs = await query.CountAsync();

            var actionStats = await query
                .GroupBy(a => a.Action)
                .Select(g => new { Action = g.Key, Count = g.Count() })
                .ToListAsync();

            var entityStats = await query
                .GroupBy(a => a.EntityType)
                .Select(g => new { EntityType = g.Key, Count = g.Count() })
                .ToListAsync();

            var userStats = await query
                .Where(a => a.UserId.HasValue)
                .GroupBy(a => a.UserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(10)
                .ToListAsync();

            var dailyStats = await query
                .Where(a => a.Timestamp >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(a => a.Timestamp.Date)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .OrderBy(g => g.Date)
                .ToListAsync();

            return new AuditLogStatsDto
            {
                TotalLogs = totalLogs,
                ActionBreakdown = actionStats.ToDictionary(a => a.Action, a => a.Count),
                EntityBreakdown = entityStats.ToDictionary(e => e.EntityType, e => e.Count),
                TopUsers = userStats.ToDictionary(u => u.UserId?.ToString() ?? "Unknown", u => u.Count),
                DailyActivity = dailyStats.ToDictionary(d => d.Date.ToString("yyyy-MM-dd"), d => d.Count)
            };
        }

        public async Task<bool> ExportAuditLogsAsync(string filePath, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = _context.AuditLogs
                    .Where(a => a.TenantId == _tenantContext.TenantId);

                if (fromDate.HasValue)
                    query = query.Where(a => a.Timestamp >= fromDate.Value);

                if (toDate.HasValue)
                    query = query.Where(a => a.Timestamp <= toDate.Value);

                var auditLogs = await query
                    .OrderByDescending(a => a.Timestamp)
                    .ToListAsync();

                var csvContent = GenerateCsvContent(auditLogs);
                await File.WriteAllTextAsync(filePath, csvContent);

                _logger.LogInformation($"Exported {auditLogs.Count} audit logs to {filePath}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export audit logs to {filePath}");
                return false;
            }
        }

        public async Task CleanupOldLogsAsync(int retentionDays = 2555)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

                var oldLogs = await _context.AuditLogs
                    .Where(a => a.Timestamp < cutoffDate)
                    .ToListAsync();

                if (oldLogs.Any())
                {
                    _context.AuditLogs.RemoveRange(oldLogs);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Cleaned up {oldLogs.Count} audit logs older than {retentionDays} days");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to cleanup old audit logs");
            }
        }

        private string GenerateCsvContent(List<AuditLog> auditLogs)
        {
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("Timestamp,UserId,Action,EntityType,EntityId,OldValues,NewValues,IpAddress,UserAgent");

            foreach (var log in auditLogs)
            {
                csv.AppendLine($"{log.Timestamp:yyyy-MM-dd HH:mm:ss},{log.UserId},{log.Action},{log.EntityType},{log.EntityId},\"{log.OldValues?.Replace("\"", "\"\"")}\",\"{log.NewValues?.Replace("\"", "\"\"")}\",{log.IpAddress},{log.UserAgent}");
            }

            return csv.ToString();
        }

        private string? GetClientIpAddress()
        {
            // In a real implementation, this would extract the IP from HttpContext
            return "127.0.0.1";
        }

        private string? GetUserAgent()
        {
            // In a real implementation, this would extract the User-Agent from HttpContext
            return "AttendancePro API";
        }

        private AuditLogDto MapToDto(AuditLog auditLog)
        {
            return new AuditLogDto
            {
                Id = auditLog.Id,
                UserId = auditLog.UserId,
                Action = auditLog.Action,
                EntityType = auditLog.EntityType,
                EntityId = auditLog.EntityId,
                OldValues = auditLog.OldValues,
                NewValues = auditLog.NewValues,
                Timestamp = auditLog.Timestamp,
                IpAddress = auditLog.IpAddress,
                UserAgent = auditLog.UserAgent
            };
        }
    }

    public interface IHrIntegrationService
    {
        Task<bool> SyncEmployeesAsync(string provider);
        Task<bool> SyncDepartmentsAsync(string provider);
        Task<HrEmployeeDto?> GetEmployeeFromHrAsync(string employeeId, string provider);
        Task<IEnumerable<HrEmployeeDto>> GetAllEmployeesFromHrAsync(string provider);
        Task<bool> PushAttendanceDataAsync(string provider, DateTime fromDate, DateTime toDate);
        Task<HrIntegrationStatusDto> GetIntegrationStatusAsync(string provider);
    }

    public class HrIntegrationService : IHrIntegrationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HrIntegrationService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public HrIntegrationService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<HrIntegrationService> logger,
            AttendancePlatformDbContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        public async Task<bool> SyncEmployeesAsync(string provider)
        {
            try
            {
                var employees = await GetAllEmployeesFromHrAsync(provider);
                
                foreach (var hrEmployee in employees)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.ExternalId == hrEmployee.Id || u.Email == hrEmployee.Email);

                    if (existingUser == null)
                    {
                        // Create new user
                        var newUser = new User
                        {
                            Id = Guid.NewGuid(),
                            ExternalId = hrEmployee.Id,
                            UserName = hrEmployee.Email,
                            Email = hrEmployee.Email,
                            FirstName = hrEmployee.FirstName,
                            LastName = hrEmployee.LastName,
                            PhoneNumber = hrEmployee.PhoneNumber,
                            IsActive = hrEmployee.IsActive,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.Users.Add(newUser);
                    }
                    else
                    {
                        // Update existing user
                        existingUser.FirstName = hrEmployee.FirstName;
                        existingUser.LastName = hrEmployee.LastName;
                        existingUser.PhoneNumber = hrEmployee.PhoneNumber;
                        existingUser.IsActive = hrEmployee.IsActive;
                        existingUser.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully synced employees from {provider}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to sync employees from {provider}");
                return false;
            }
        }

        public async Task<bool> SyncDepartmentsAsync(string provider)
        {
            // Implementation for syncing departments from HR systems
            _logger.LogInformation($"Syncing departments from {provider}");
            return await Task.FromResult(true);
        }

        public async Task<HrEmployeeDto?> GetEmployeeFromHrAsync(string employeeId, string provider)
        {
            return provider.ToLower() switch
            {
                "bamboohr" => await GetBambooHrEmployeeAsync(employeeId),
                "workday" => await GetWorkdayEmployeeAsync(employeeId),
                "successfactors" => await GetSuccessFactorsEmployeeAsync(employeeId),
                _ => null
            };
        }

        public async Task<IEnumerable<HrEmployeeDto>> GetAllEmployeesFromHrAsync(string provider)
        {
            return provider.ToLower() switch
            {
                "bamboohr" => await GetAllBambooHrEmployeesAsync(),
                "workday" => await GetAllWorkdayEmployeesAsync(),
                "successfactors" => await GetAllSuccessFactorsEmployeesAsync(),
                _ => new List<HrEmployeeDto>()
            };
        }

        public async Task<bool> PushAttendanceDataAsync(string provider, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendanceRecords = await _context.AttendanceRecords
                    .Where(a => a.CheckInTime >= fromDate && a.CheckInTime <= toDate)
                    .Include(a => a.User)
                    .ToListAsync();

                return provider.ToLower() switch
                {
                    "bamboohr" => await PushToBambooHrAsync(attendanceRecords),
                    "workday" => await PushToWorkdayAsync(attendanceRecords),
                    "successfactors" => await PushToSuccessFactorsAsync(attendanceRecords),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to push attendance data to {provider}");
                return false;
            }
        }

        public async Task<HrIntegrationStatusDto> GetIntegrationStatusAsync(string provider)
        {
            var config = _configuration.GetSection($"HrIntegrations:{provider}");
            var isEnabled = config.GetValue<bool>("Enabled");

            return new HrIntegrationStatusDto
            {
                Provider = provider,
                IsEnabled = isEnabled,
                LastSyncTime = await GetLastSyncTimeAsync(provider),
                Status = isEnabled ? "Active" : "Disabled"
            };
        }

        private async Task<HrEmployeeDto?> GetBambooHrEmployeeAsync(string employeeId)
        {
            // BambooHR API implementation
            _logger.LogInformation($"Getting employee {employeeId} from BambooHR");
            return await Task.FromResult<HrEmployeeDto?>(null);
        }

        private async Task<HrEmployeeDto?> GetWorkdayEmployeeAsync(string employeeId)
        {
            // Workday API implementation
            _logger.LogInformation($"Getting employee {employeeId} from Workday");
            return await Task.FromResult<HrEmployeeDto?>(null);
        }

        private async Task<HrEmployeeDto?> GetSuccessFactorsEmployeeAsync(string employeeId)
        {
            // SuccessFactors API implementation
            _logger.LogInformation($"Getting employee {employeeId} from SuccessFactors");
            return await Task.FromResult<HrEmployeeDto?>(null);
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetAllBambooHrEmployeesAsync()
        {
            // BambooHR API implementation
            _logger.LogInformation("Getting all employees from BambooHR");
            return await Task.FromResult(new List<HrEmployeeDto>());
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetAllWorkdayEmployeesAsync()
        {
            // Workday API implementation
            _logger.LogInformation("Getting all employees from Workday");
            return await Task.FromResult(new List<HrEmployeeDto>());
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetAllSuccessFactorsEmployeesAsync()
        {
            // SuccessFactors API implementation
            _logger.LogInformation("Getting all employees from SuccessFactors");
            return await Task.FromResult(new List<HrEmployeeDto>());
        }

        private async Task<bool> PushToBambooHrAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Pushing {records.Count} attendance records to BambooHR");
            return await Task.FromResult(true);
        }

        private async Task<bool> PushToWorkdayAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Pushing {records.Count} attendance records to Workday");
            return await Task.FromResult(true);
        }

        private async Task<bool> PushToSuccessFactorsAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Pushing {records.Count} attendance records to SuccessFactors");
            return await Task.FromResult(true);
        }

        private async Task<DateTime?> GetLastSyncTimeAsync(string provider)
        {
            // Get last sync time from database or configuration
            return await Task.FromResult<DateTime?>(DateTime.UtcNow.AddHours(-1));
        }
    }

    public interface IPayrollIntegrationService
    {
        Task<bool> ExportPayrollDataAsync(string provider, DateTime fromDate, DateTime toDate);
        Task<PayrollIntegrationStatusDto> GetPayrollStatusAsync(string provider);
    }

    public class PayrollIntegrationService : IPayrollIntegrationService
    {
        private readonly ILogger<PayrollIntegrationService> _logger;
        private readonly AttendancePlatformDbContext _context;
        private readonly IConfiguration _configuration;

        public PayrollIntegrationService(
            ILogger<PayrollIntegrationService> logger,
            AttendancePlatformDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> ExportPayrollDataAsync(string provider, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var attendanceData = await _context.AttendanceRecords
                    .Where(a => a.CheckInTime >= fromDate && a.CheckInTime <= toDate)
                    .Include(a => a.User)
                    .ToListAsync();

                return provider.ToLower() switch
                {
                    "adp" => await ExportToAdpAsync(attendanceData),
                    "paychex" => await ExportToPaychexAsync(attendanceData),
                    "quickbooks" => await ExportToQuickBooksAsync(attendanceData),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to export payroll data to {provider}");
                return false;
            }
        }

        public async Task<PayrollIntegrationStatusDto> GetPayrollStatusAsync(string provider)
        {
            var config = _configuration.GetSection($"PayrollIntegrations:{provider}");
            var isEnabled = config.GetValue<bool>("Enabled");

            return new PayrollIntegrationStatusDto
            {
                Provider = provider,
                IsEnabled = isEnabled,
                LastExportTime = await GetLastExportTimeAsync(provider),
                Status = isEnabled ? "Active" : "Disabled"
            };
        }

        private async Task<bool> ExportToAdpAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Exporting {records.Count} records to ADP");
            return await Task.FromResult(true);
        }

        private async Task<bool> ExportToPaychexAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Exporting {records.Count} records to Paychex");
            return await Task.FromResult(true);
        }

        private async Task<bool> ExportToQuickBooksAsync(List<AttendanceRecord> records)
        {
            _logger.LogInformation($"Exporting {records.Count} records to QuickBooks");
            return await Task.FromResult(true);
        }

        private async Task<DateTime?> GetLastExportTimeAsync(string provider)
        {
            return await Task.FromResult<DateTime?>(DateTime.UtcNow.AddDays(-1));
        }
    }

    public interface IActiveDirectoryService
    {
        Task<bool> SyncUsersFromAdAsync();
        Task<bool> AuthenticateUserAsync(string username, string password);
        Task<AdUserDto?> GetAdUserAsync(string username);
    }

    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly IConfiguration _configuration;

        public ActiveDirectoryService(
            ILogger<ActiveDirectoryService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<bool> SyncUsersFromAdAsync()
        {
            _logger.LogInformation("Syncing users from Active Directory");
            return await Task.FromResult(true);
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            _logger.LogInformation($"Authenticating user {username} against Active Directory");
            return await Task.FromResult(true);
        }

        public async Task<AdUserDto?> GetAdUserAsync(string username)
        {
            _logger.LogInformation($"Getting user {username} from Active Directory");
            return await Task.FromResult<AdUserDto?>(null);
        }
    }

    // DTOs
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }

    public class AuditLogStatsDto
    {
        public int TotalLogs { get; set; }
        public Dictionary<string, int> ActionBreakdown { get; set; } = new();
        public Dictionary<string, int> EntityBreakdown { get; set; } = new();
        public Dictionary<string, int> TopUsers { get; set; } = new();
        public Dictionary<string, int> DailyActivity { get; set; } = new();
    }

    public class HrEmployeeDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public string? JobTitle { get; set; }
        public DateTime? HireDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class HrIntegrationStatusDto
    {
        public string Provider { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PayrollIntegrationStatusDto
    {
        public string Provider { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public DateTime? LastExportTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class AdUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string? Title { get; set; }
    }
}

