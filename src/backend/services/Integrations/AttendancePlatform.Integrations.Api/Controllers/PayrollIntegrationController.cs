using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Integrations.Api.Services;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PayrollIntegrationController : ControllerBase
    {
        private readonly IPayrollIntegrationService _payrollIntegrationService;

        public PayrollIntegrationController(IPayrollIntegrationService payrollIntegrationService)
        {
            _payrollIntegrationService = payrollIntegrationService;
        }

        [HttpPost("sync/{payrollSystemType}")]
        public async Task<IActionResult> SyncPayrollData(string payrollSystemType)
        {
            var result = await _payrollIntegrationService.SyncPayrollDataAsync(payrollSystemType);
            if (result)
                return Ok(new { message = "Payroll data sync completed successfully" });
            
            return BadRequest(new { message = "Payroll data sync failed" });
        }

        [HttpGet("employees/{payrollSystemType}")]
        public async Task<IActionResult> GetEmployees(string payrollSystemType)
        {
            var employees = await _payrollIntegrationService.GetEmployeesFromPayrollSystemAsync(payrollSystemType);
            return Ok(employees);
        }

        [HttpGet("employees/{payrollSystemType}/{employeeId}")]
        public async Task<IActionResult> GetEmployee(string payrollSystemType, string employeeId)
        {
            var employee = await _payrollIntegrationService.GetEmployeeFromPayrollSystemAsync(payrollSystemType, employeeId);
            if (employee == null)
                return NotFound();
            
            return Ok(employee);
        }

        [HttpPut("employees/{payrollSystemType}/{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(string payrollSystemType, string employeeId, [FromBody] PayrollEmployeeDto employee)
        {
            var result = await _payrollIntegrationService.UpdateEmployeeInPayrollSystemAsync(payrollSystemType, employeeId, employee);
            if (result)
                return Ok(new { message = "Employee updated successfully" });
            
            return BadRequest(new { message = "Failed to update employee" });
        }

        [HttpPost("employees/{payrollSystemType}")]
        public async Task<IActionResult> CreateEmployee(string payrollSystemType, [FromBody] PayrollEmployeeDto employee)
        {
            var result = await _payrollIntegrationService.CreateEmployeeInPayrollSystemAsync(payrollSystemType, employee);
            if (result)
                return Ok(new { message = "Employee created successfully" });
            
            return BadRequest(new { message = "Failed to create employee" });
        }

        [HttpDelete("employees/{payrollSystemType}/{employeeId}")]
        public async Task<IActionResult> DeactivateEmployee(string payrollSystemType, string employeeId)
        {
            var result = await _payrollIntegrationService.DeactivateEmployeeInPayrollSystemAsync(payrollSystemType, employeeId);
            if (result)
                return Ok(new { message = "Employee deactivated successfully" });
            
            return BadRequest(new { message = "Failed to deactivate employee" });
        }

        [HttpGet("payroll-records/{payrollSystemType}")]
        public async Task<IActionResult> GetPayrollRecords(string payrollSystemType, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var records = await _payrollIntegrationService.GetPayrollRecordsAsync(payrollSystemType, startDate, endDate);
            return Ok(records);
        }

        [HttpPost("payroll-records/{payrollSystemType}")]
        public async Task<IActionResult> CreatePayrollRecord(string payrollSystemType, [FromBody] PayrollRecordDto payrollRecord)
        {
            var createdRecord = await _payrollIntegrationService.CreatePayrollRecordAsync(payrollSystemType, payrollRecord);
            if (createdRecord == null)
                return BadRequest(new { message = "Failed to create payroll record" });
            
            return Ok(createdRecord);
        }

        [HttpPost("process-payroll/{payrollSystemType}/{payPeriodId}")]
        public async Task<IActionResult> ProcessPayroll(string payrollSystemType, string payPeriodId)
        {
            var result = await _payrollIntegrationService.ProcessPayrollAsync(payrollSystemType, payPeriodId);
            if (result)
                return Ok(new { message = "Payroll processed successfully" });
            
            return BadRequest(new { message = "Failed to process payroll" });
        }

        [HttpGet("time-entries/{payrollSystemType}/{employeeId}")]
        public async Task<IActionResult> GetTimeEntries(string payrollSystemType, string employeeId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var timeEntries = await _payrollIntegrationService.GetTimeEntriesAsync(payrollSystemType, employeeId, startDate, endDate);
            return Ok(timeEntries);
        }

        [HttpPost("time-entries/{payrollSystemType}")]
        public async Task<IActionResult> CreateTimeEntry(string payrollSystemType, [FromBody] TimeEntryDto timeEntry)
        {
            var createdEntry = await _payrollIntegrationService.CreateTimeEntryAsync(payrollSystemType, timeEntry);
            if (createdEntry == null)
                return BadRequest(new { message = "Failed to create time entry" });
            
            return Ok(createdEntry);
        }

        [HttpPost("approve-time-entries/{payrollSystemType}")]
        public async Task<IActionResult> ApproveTimeEntries(string payrollSystemType, [FromBody] List<string> timeEntryIds)
        {
            var result = await _payrollIntegrationService.ApproveTimeEntriesAsync(payrollSystemType, timeEntryIds);
            if (result)
                return Ok(new { message = "Time entries approved successfully" });
            
            return BadRequest(new { message = "Failed to approve time entries" });
        }

        [HttpGet("sync-status/{payrollSystemType}")]
        public async Task<IActionResult> GetSyncStatus(string payrollSystemType)
        {
            var status = await _payrollIntegrationService.GetSyncStatusAsync(payrollSystemType);
            return Ok(status);
        }

        [HttpGet("test-connection/{payrollSystemType}")]
        public async Task<IActionResult> TestConnection(string payrollSystemType)
        {
            var isConnected = await _payrollIntegrationService.TestConnectionAsync(payrollSystemType);
            return Ok(new { isConnected, payrollSystemType });
        }
    }
}
