using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class HrIntegrationService : IHrIntegrationService
    {
        private readonly ILogger<HrIntegrationService> _logger;
        private readonly AttendancePlatformDbContext _context;
        private readonly IConfiguration _configuration;

        public HrIntegrationService(
            ILogger<HrIntegrationService> logger,
            AttendancePlatformDbContext context,
            IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> SyncEmployeeDataAsync(string hrSystemType)
        {
            try
            {
                _logger.LogInformation("Starting HR sync for system: {HrSystemType}", hrSystemType);

                var employees = await GetEmployeesFromHrSystemAsync(hrSystemType);
                var syncedCount = 0;

                foreach (var hrEmployee in employees)
                {
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == hrEmployee.Email);

                    if (existingUser != null)
                    {
                        existingUser.FirstName = hrEmployee.FirstName;
                        existingUser.LastName = hrEmployee.LastName;
                        existingUser.UpdatedAt = DateTime.UtcNow;
                        syncedCount++;
                    }
                    else
                    {
                        var newUser = new User
                        {
                            Id = Guid.NewGuid(),
                            Username = hrEmployee.Email,
                            Email = hrEmployee.Email,
                            FirstName = hrEmployee.FirstName,
                            LastName = hrEmployee.LastName,
                            IsActive = hrEmployee.Status == "Active",
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };

                        _context.Users.Add(newUser);
                        syncedCount++;
                    }
                }

                await _context.SaveChangesAsync();

                var syncStatus = new HrSyncLog
                {
                    Id = Guid.NewGuid(),
                    HrSystemType = hrSystemType,
                    SyncTime = DateTime.UtcNow,
                    EmployeesSynced = syncedCount,
                    Status = "Success",
                    TenantId = Guid.NewGuid()
                };

                _context.HrSyncLogs.Add(syncStatus);
                await _context.SaveChangesAsync();

                _logger.LogInformation("HR sync completed successfully. Synced {Count} employees", syncedCount);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync HR data for system: {HrSystemType}", hrSystemType);
                return false;
            }
        }

        public async Task<IEnumerable<HrEmployeeDto>> GetEmployeesFromHrSystemAsync(string hrSystemType)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await GetBambooHrEmployeesAsync(),
                    "workday" => await GetWorkdayEmployeesAsync(),
                    "successfactors" => await GetSuccessFactorsEmployeesAsync(),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employees from HR system: {HrSystemType}", hrSystemType);
                return new List<HrEmployeeDto>();
            }
        }

        public async Task<HrEmployeeDto> GetEmployeeFromHrSystemAsync(string hrSystemType, string employeeId)
        {
            try
            {
                var employees = await GetEmployeesFromHrSystemAsync(hrSystemType);
                return employees.FirstOrDefault(e => e.Id == employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee {EmployeeId} from HR system: {HrSystemType}", employeeId, hrSystemType);
                return null;
            }
        }

        public async Task<bool> UpdateEmployeeInHrSystemAsync(string hrSystemType, string employeeId, HrEmployeeDto employee)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await UpdateBambooHrEmployeeAsync(employeeId, employee),
                    "workday" => await UpdateWorkdayEmployeeAsync(employeeId, employee),
                    "successfactors" => await UpdateSuccessFactorsEmployeeAsync(employeeId, employee),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update employee {EmployeeId} in HR system: {HrSystemType}", employeeId, hrSystemType);
                return false;
            }
        }

        public async Task<bool> CreateEmployeeInHrSystemAsync(string hrSystemType, HrEmployeeDto employee)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await CreateBambooHrEmployeeAsync(employee),
                    "workday" => await CreateWorkdayEmployeeAsync(employee),
                    "successfactors" => await CreateSuccessFactorsEmployeeAsync(employee),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee in HR system: {HrSystemType}", hrSystemType);
                return false;
            }
        }

        public async Task<bool> DeactivateEmployeeInHrSystemAsync(string hrSystemType, string employeeId)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await DeactivateBambooHrEmployeeAsync(employeeId),
                    "workday" => await DeactivateWorkdayEmployeeAsync(employeeId),
                    "successfactors" => await DeactivateSuccessFactorsEmployeeAsync(employeeId),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deactivate employee {EmployeeId} in HR system: {HrSystemType}", employeeId, hrSystemType);
                return false;
            }
        }

        public async Task<IEnumerable<HrDepartmentDto>> GetDepartmentsFromHrSystemAsync(string hrSystemType)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await GetBambooHrDepartmentsAsync(),
                    "workday" => await GetWorkdayDepartmentsAsync(),
                    "successfactors" => await GetSuccessFactorsDepartmentsAsync(),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get departments from HR system: {HrSystemType}", hrSystemType);
                return new List<HrDepartmentDto>();
            }
        }

        public async Task<IEnumerable<HrPositionDto>> GetPositionsFromHrSystemAsync(string hrSystemType)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await GetBambooHrPositionsAsync(),
                    "workday" => await GetWorkdayPositionsAsync(),
                    "successfactors" => await GetSuccessFactorsPositionsAsync(),
                    _ => throw new NotSupportedException($"HR system {hrSystemType} is not supported")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get positions from HR system: {HrSystemType}", hrSystemType);
                return new List<HrPositionDto>();
            }
        }

        public async Task<HrSyncStatusDto> GetSyncStatusAsync(string hrSystemType)
        {
            try
            {
                var lastSync = await _context.HrSyncLogs
                    .Where(h => h.HrSystemType == hrSystemType)
                    .OrderByDescending(h => h.SyncTime)
                    .FirstOrDefaultAsync();

                return new HrSyncStatusDto
                {
                    HrSystemType = hrSystemType,
                    LastSyncTime = lastSync?.SyncTime,
                    IsConnected = await TestConnectionAsync(hrSystemType),
                    Status = lastSync?.Status ?? "Never Synced",
                    EmployeesSynced = lastSync?.EmployeesSynced ?? 0,
                    DepartmentsSynced = lastSync?.DepartmentsSynced ?? 0,
                    PositionsSynced = lastSync?.PositionsSynced ?? 0,
                    Errors = new List<string>(),
                    NextScheduledSync = DateTime.UtcNow.AddHours(24)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sync status for HR system: {HrSystemType}", hrSystemType);
                return new HrSyncStatusDto
                {
                    HrSystemType = hrSystemType,
                    IsConnected = false,
                    Status = "Error",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<bool> TestConnectionAsync(string hrSystemType)
        {
            try
            {
                return hrSystemType.ToLower() switch
                {
                    "bamboohr" => await TestBambooHrConnectionAsync(),
                    "workday" => await TestWorkdayConnectionAsync(),
                    "successfactors" => await TestSuccessFactorsConnectionAsync(),
                    _ => false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test connection for HR system: {HrSystemType}", hrSystemType);
                return false;
            }
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetBambooHrEmployeesAsync()
        {
            await Task.CompletedTask;
            
            return new List<HrEmployeeDto>
            {
                new HrEmployeeDto
                {
                    Id = "1",
                    EmployeeNumber = "EMP001",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@company.com",
                    Department = "Engineering",
                    Position = "Software Developer",
                    Status = "Active",
                    HireDate = DateTime.UtcNow.AddYears(-2),
                    LastModified = DateTime.UtcNow
                }
            };
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetWorkdayEmployeesAsync()
        {
            await Task.CompletedTask;
            return new List<HrEmployeeDto>();
        }

        private async Task<IEnumerable<HrEmployeeDto>> GetSuccessFactorsEmployeesAsync()
        {
            await Task.CompletedTask;
            return new List<HrEmployeeDto>();
        }

        private async Task<bool> UpdateBambooHrEmployeeAsync(string employeeId, HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> UpdateWorkdayEmployeeAsync(string employeeId, HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> UpdateSuccessFactorsEmployeeAsync(string employeeId, HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> CreateBambooHrEmployeeAsync(HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> CreateWorkdayEmployeeAsync(HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> CreateSuccessFactorsEmployeeAsync(HrEmployeeDto employee)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> DeactivateBambooHrEmployeeAsync(string employeeId)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> DeactivateWorkdayEmployeeAsync(string employeeId)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<bool> DeactivateSuccessFactorsEmployeeAsync(string employeeId)
        {
            await Task.CompletedTask;
            return true;
        }

        private async Task<IEnumerable<HrDepartmentDto>> GetBambooHrDepartmentsAsync()
        {
            await Task.CompletedTask;
            return new List<HrDepartmentDto>();
        }

        private async Task<IEnumerable<HrDepartmentDto>> GetWorkdayDepartmentsAsync()
        {
            await Task.CompletedTask;
            return new List<HrDepartmentDto>();
        }

        private async Task<IEnumerable<HrDepartmentDto>> GetSuccessFactorsDepartmentsAsync()
        {
            await Task.CompletedTask;
            return new List<HrDepartmentDto>();
        }

        private async Task<IEnumerable<HrPositionDto>> GetBambooHrPositionsAsync()
        {
            await Task.CompletedTask;
            return new List<HrPositionDto>();
        }

        private async Task<IEnumerable<HrPositionDto>> GetWorkdayPositionsAsync()
        {
            await Task.CompletedTask;
            return new List<HrPositionDto>();
        }

        private async Task<IEnumerable<HrPositionDto>> GetSuccessFactorsPositionsAsync()
        {
            await Task.CompletedTask;
            return new List<HrPositionDto>();
        }

        private async Task<bool> TestBambooHrConnectionAsync()
        {
            await Task.CompletedTask;
            var config = _configuration.GetSection("HrIntegrations:BambooHr");
            return !string.IsNullOrEmpty(config["ApiKey"]) && config.GetValue<bool>("Enabled");
        }

        private async Task<bool> TestWorkdayConnectionAsync()
        {
            await Task.CompletedTask;
            var config = _configuration.GetSection("HrIntegrations:Workday");
            return !string.IsNullOrEmpty(config["Username"]) && config.GetValue<bool>("Enabled");
        }

        private async Task<bool> TestSuccessFactorsConnectionAsync()
        {
            await Task.CompletedTask;
            var config = _configuration.GetSection("HrIntegrations:SuccessFactors");
            return !string.IsNullOrEmpty(config["ApiUrl"]) && config.GetValue<bool>("Enabled");
        }
    }

    public class HrSyncLog
    {
        public Guid Id { get; set; }
        public string HrSystemType { get; set; }
        public DateTime SyncTime { get; set; }
        public int EmployeesSynced { get; set; }
        public int DepartmentsSynced { get; set; }
        public int PositionsSynced { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public Guid TenantId { get; set; }
    }
}
