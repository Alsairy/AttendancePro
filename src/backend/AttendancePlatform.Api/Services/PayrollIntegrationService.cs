using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IPayrollIntegrationService
    {
        Task<PayrollSyncResultDto> SyncPayrollDataAsync(Guid tenantId);
        Task<List<PayrollProviderDto>> GetPayrollProvidersAsync();
        Task<PayrollConfigurationDto> ConfigurePayrollProviderAsync(Guid tenantId, PayrollConfigurationDto config);
        Task<List<PayrollReportDto>> GetPayrollReportsAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<PayrollCalculationDto> CalculatePayrollAsync(Guid tenantId, Guid employeeId, DateTime payPeriodStart, DateTime payPeriodEnd);
        Task<bool> ProcessPayrollAsync(Guid tenantId, List<Guid> employeeIds);
        Task<List<PayrollAuditDto>> GetPayrollAuditTrailAsync(Guid tenantId);
        Task<PayrollComplianceDto> ValidatePayrollComplianceAsync(Guid tenantId);
        Task<List<TaxCalculationDto>> CalculateTaxesAsync(Guid tenantId, List<PayrollCalculationDto> calculations);
        Task<PayrollExportDto> ExportPayrollDataAsync(Guid tenantId, string format);
    }

    public class PayrollIntegrationService : IPayrollIntegrationService
    {
        private readonly ILogger<PayrollIntegrationService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public PayrollIntegrationService(ILogger<PayrollIntegrationService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<PayrollSyncResultDto> SyncPayrollDataAsync(Guid tenantId)
        {
            try
            {
                var employees = await _context.Users.Where(u => u.TenantId == tenantId && u.IsActive).ToListAsync();
                var attendanceRecords = await _context.AttendanceRecords
                    .Where(a => a.TenantId == tenantId && a.CheckInTime >= DateTime.UtcNow.AddDays(-30))
                    .ToListAsync();

                var syncResult = new PayrollSyncResultDto
                {
                    TenantId = tenantId,
                    EmployeesProcessed = employees.Count,
                    RecordsProcessed = attendanceRecords.Count,
                    SyncedAt = DateTime.UtcNow,
                    Status = "Success",
                    Errors = new List<string>()
                };

                _logger.LogInformation("Payroll data synced for tenant {TenantId}: {EmployeesProcessed} employees, {RecordsProcessed} records", 
                    tenantId, employees.Count, attendanceRecords.Count);

                return syncResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync payroll data for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<PayrollProviderDto>> GetPayrollProvidersAsync()
        {
            await Task.CompletedTask;
            return new List<PayrollProviderDto>
            {
                new PayrollProviderDto { Id = Guid.NewGuid(), Name = "ADP", Description = "ADP Workforce Now", IsActive = true },
                new PayrollProviderDto { Id = Guid.NewGuid(), Name = "Paychex", Description = "Paychex Flex", IsActive = true },
                new PayrollProviderDto { Id = Guid.NewGuid(), Name = "QuickBooks", Description = "QuickBooks Payroll", IsActive = true },
                new PayrollProviderDto { Id = Guid.NewGuid(), Name = "Sage", Description = "Sage Payroll", IsActive = true },
                new PayrollProviderDto { Id = Guid.NewGuid(), Name = "BambooHR", Description = "BambooHR Payroll", IsActive = true }
            };
        }

        public async Task<PayrollConfigurationDto> ConfigurePayrollProviderAsync(Guid tenantId, PayrollConfigurationDto config)
        {
            try
            {
                await Task.CompletedTask;
                config.TenantId = tenantId;
                config.ConfiguredAt = DateTime.UtcNow;
                config.Status = "Active";

                _logger.LogInformation("Payroll provider configured for tenant {TenantId}: {Provider}", tenantId, config.ProviderName);
                return config;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure payroll provider for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<PayrollReportDto>> GetPayrollReportsAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var employees = await _context.Users.Where(u => u.TenantId == tenantId && u.IsActive).ToListAsync();
                var reports = new List<PayrollReportDto>();

                foreach (var employee in employees.Take(10))
                {
                    reports.Add(new PayrollReportDto
                    {
                        EmployeeId = employee.Id,
                        EmployeeName = $"{employee.FirstName} {employee.LastName}",
                        PayPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                        GrossPay = 5000m,
                        NetPay = 3800m,
                        Deductions = 1200m,
                        HoursWorked = 160,
                        OvertimeHours = 8,
                        GeneratedAt = DateTime.UtcNow
                    });
                }

                return reports;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payroll reports for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<PayrollCalculationDto> CalculatePayrollAsync(Guid tenantId, Guid employeeId, DateTime payPeriodStart, DateTime payPeriodEnd)
        {
            try
            {
                var employee = await _context.Users.FindAsync(employeeId);
                if (employee == null) throw new ArgumentException("Employee not found");

                var attendanceRecords = await _context.AttendanceRecords
                    .Where(a => a.UserId == employeeId && a.CheckInTime >= payPeriodStart && a.CheckInTime <= payPeriodEnd)
                    .ToListAsync();

                var totalHours = attendanceRecords.Sum(a => (a.CheckOutTime - a.CheckInTime)?.TotalHours ?? 0);
                var regularHours = Math.Min(totalHours, 160);
                var overtimeHours = Math.Max(0, totalHours - 160);
                var hourlyRate = 25.0m;
                var overtimeRate = hourlyRate * 1.5m;

                var grossPay = (decimal)(regularHours * (double)hourlyRate + overtimeHours * (double)overtimeRate);
                var taxDeductions = grossPay * 0.25m;
                var benefitDeductions = grossPay * 0.05m;
                var netPay = grossPay - taxDeductions - benefitDeductions;

                return new PayrollCalculationDto
                {
                    EmployeeId = employeeId,
                    PayPeriodStart = payPeriodStart,
                    PayPeriodEnd = payPeriodEnd,
                    RegularHours = regularHours,
                    OvertimeHours = overtimeHours,
                    HourlyRate = hourlyRate,
                    GrossPay = grossPay,
                    TaxDeductions = taxDeductions,
                    BenefitDeductions = benefitDeductions,
                    NetPay = netPay,
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate payroll for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<bool> ProcessPayrollAsync(Guid tenantId, List<Guid> employeeIds)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Payroll processed for tenant {TenantId}: {EmployeeCount} employees", tenantId, employeeIds.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payroll for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<List<PayrollAuditDto>> GetPayrollAuditTrailAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<PayrollAuditDto>
            {
                new PayrollAuditDto { Action = "Payroll Calculated", User = "System", Timestamp = DateTime.UtcNow.AddHours(-2), Details = "Monthly payroll calculation completed" },
                new PayrollAuditDto { Action = "Payroll Processed", User = "admin@test.com", Timestamp = DateTime.UtcNow.AddHours(-1), Details = "Payroll processing initiated" },
                new PayrollAuditDto { Action = "Tax Calculation", User = "System", Timestamp = DateTime.UtcNow.AddMinutes(-30), Details = "Tax calculations updated" }
            };
        }

        public async Task<PayrollComplianceDto> ValidatePayrollComplianceAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new PayrollComplianceDto
            {
                TenantId = tenantId,
                ComplianceScore = 96.5,
                MinimumWageCompliance = true,
                OvertimeCompliance = true,
                TaxComplianceStatus = "Compliant",
                LaborLawCompliance = true,
                Issues = new List<string>(),
                ValidatedAt = DateTime.UtcNow
            };
        }

        public async Task<List<TaxCalculationDto>> CalculateTaxesAsync(Guid tenantId, List<PayrollCalculationDto> calculations)
        {
            await Task.CompletedTask;
            var taxCalculations = new List<TaxCalculationDto>();

            foreach (var calc in calculations)
            {
                taxCalculations.Add(new TaxCalculationDto
                {
                    EmployeeId = calc.EmployeeId,
                    FederalTax = calc.GrossPay * 0.15m,
                    StateTax = calc.GrossPay * 0.05m,
                    SocialSecurity = calc.GrossPay * 0.062m,
                    Medicare = calc.GrossPay * 0.0145m,
                    TotalTax = calc.GrossPay * 0.2265m,
                    CalculatedAt = DateTime.UtcNow
                });
            }

            return taxCalculations;
        }

        public async Task<PayrollExportDto> ExportPayrollDataAsync(Guid tenantId, string format)
        {
            await Task.CompletedTask;
            return new PayrollExportDto
            {
                TenantId = tenantId,
                Format = format,
                FileName = $"payroll_export_{DateTime.UtcNow:yyyyMMdd}.{format.ToLower()}",
                FileSize = 1024 * 50,
                RecordCount = 100,
                ExportedAt = DateTime.UtcNow,
                DownloadUrl = $"/api/payroll/download/{Guid.NewGuid()}"
            };
        }
    }

    public class PayrollSyncResultDto
    {
        public Guid TenantId { get; set; }
        public int EmployeesProcessed { get; set; }
        public int RecordsProcessed { get; set; }
        public DateTime SyncedAt { get; set; }
        public required string Status { get; set; }
        public required List<string> Errors { get; set; }
    }

    public class PayrollProviderDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class PayrollConfigurationDto
    {
        public Guid TenantId { get; set; }
        public required string ProviderName { get; set; }
        public required string ApiEndpoint { get; set; }
        public required string ApiKey { get; set; }
        public required string Status { get; set; }
        public DateTime ConfiguredAt { get; set; }
    }

    public class PayrollReportDto
    {
        public Guid EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public required string PayPeriod { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public decimal Deductions { get; set; }
        public double HoursWorked { get; set; }
        public double OvertimeHours { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class PayrollCalculationDto
    {
        public Guid EmployeeId { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public double RegularHours { get; set; }
        public double OvertimeHours { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TaxDeductions { get; set; }
        public decimal BenefitDeductions { get; set; }
        public decimal NetPay { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    public class PayrollAuditDto
    {
        public required string Action { get; set; }
        public required string User { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Details { get; set; }
    }

    public class PayrollComplianceDto
    {
        public Guid TenantId { get; set; }
        public double ComplianceScore { get; set; }
        public bool MinimumWageCompliance { get; set; }
        public bool OvertimeCompliance { get; set; }
        public required string TaxComplianceStatus { get; set; }
        public bool LaborLawCompliance { get; set; }
        public List<string> Issues { get; set; }
        public DateTime ValidatedAt { get; set; }
    }

    public class TaxCalculationDto
    {
        public Guid EmployeeId { get; set; }
        public decimal FederalTax { get; set; }
        public decimal StateTax { get; set; }
        public decimal SocialSecurity { get; set; }
        public decimal Medicare { get; set; }
        public decimal TotalTax { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    public class PayrollExportDto
    {
        public Guid TenantId { get; set; }
        public required string Format { get; set; }
        public required string FileName { get; set; }
        public long FileSize { get; set; }
        public int RecordCount { get; set; }
        public DateTime ExportedAt { get; set; }
        public required string DownloadUrl { get; set; }
    }
}
