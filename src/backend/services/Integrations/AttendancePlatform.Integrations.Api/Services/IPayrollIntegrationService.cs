using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IPayrollIntegrationService
    {
        Task<bool> SyncPayrollDataAsync(string payrollSystemType);
        Task<IEnumerable<PayrollEmployeeDto>> GetEmployeesFromPayrollSystemAsync(string payrollSystemType);
        Task<PayrollEmployeeDto> GetEmployeeFromPayrollSystemAsync(string payrollSystemType, string employeeId);
        Task<bool> UpdateEmployeeInPayrollSystemAsync(string payrollSystemType, string employeeId, PayrollEmployeeDto employee);
        Task<bool> CreateEmployeeInPayrollSystemAsync(string payrollSystemType, PayrollEmployeeDto employee);
        Task<bool> DeactivateEmployeeInPayrollSystemAsync(string payrollSystemType, string employeeId);
        
        Task<IEnumerable<PayrollRecordDto>> GetPayrollRecordsAsync(string payrollSystemType, DateTime startDate, DateTime endDate);
        Task<PayrollRecordDto> CreatePayrollRecordAsync(string payrollSystemType, PayrollRecordDto payrollRecord);
        Task<bool> ProcessPayrollAsync(string payrollSystemType, string payPeriodId);
        
        Task<IEnumerable<TimeEntryDto>> GetTimeEntriesAsync(string payrollSystemType, string employeeId, DateTime startDate, DateTime endDate);
        Task<TimeEntryDto> CreateTimeEntryAsync(string payrollSystemType, TimeEntryDto timeEntry);
        Task<bool> ApproveTimeEntriesAsync(string payrollSystemType, List<string> timeEntryIds);
        
        Task<PayrollSyncStatusDto> GetSyncStatusAsync(string payrollSystemType);
        Task<bool> TestConnectionAsync(string payrollSystemType);
    }

    public class PayrollEmployeeDto
    {
        public string Id { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string SocialSecurityNumber { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Status { get; set; }
        public string PayType { get; set; }
        public decimal PayRate { get; set; }
        public string PayFrequency { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string ManagerId { get; set; }
        public PayrollAddressDto Address { get; set; }
        public List<PayrollDeductionDto> Deductions { get; set; }
        public List<PayrollBenefitDto> Benefits { get; set; }
        public Dictionary<string, object> CustomFields { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class PayrollAddressDto
    {
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class PayrollDeductionDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsPreTax { get; set; }
        public bool IsActive { get; set; }
    }

    public class PayrollBenefitDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal EmployeeContribution { get; set; }
        public decimal EmployerContribution { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

    public class PayrollRecordDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string PayPeriodId { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public DateTime PayDate { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NetPay { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalTaxes { get; set; }
        public List<PayrollLineItemDto> LineItems { get; set; }
        public List<PayrollTaxDto> Taxes { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ProcessedDate { get; set; }
    }

    public class PayrollLineItemDto
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public decimal Hours { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }

    public class PayrollTaxDto
    {
        public string TaxType { get; set; }
        public string Description { get; set; }
        public decimal TaxableAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Rate { get; set; }
    }

    public class TimeEntryDto
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ClockIn { get; set; }
        public DateTime? ClockOut { get; set; }
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal BreakHours { get; set; }
        public string ProjectCode { get; set; }
        public string TaskCode { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class PayrollSyncStatusDto
    {
        public string PayrollSystemType { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public bool IsConnected { get; set; }
        public string Status { get; set; }
        public int EmployeesSynced { get; set; }
        public int PayrollRecordsSynced { get; set; }
        public int TimeEntriesSynced { get; set; }
        public List<string> Errors { get; set; }
        public DateTime NextScheduledSync { get; set; }
    }
}
