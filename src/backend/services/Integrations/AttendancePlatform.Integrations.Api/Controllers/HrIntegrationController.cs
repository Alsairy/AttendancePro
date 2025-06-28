using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Integrations.Api.Services;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HrIntegrationController : ControllerBase
    {
        private readonly IHrIntegrationService _hrIntegrationService;

        public HrIntegrationController(IHrIntegrationService hrIntegrationService)
        {
            _hrIntegrationService = hrIntegrationService;
        }

        [HttpPost("sync/{hrSystemType}")]
        public async Task<IActionResult> SyncEmployeeData(string hrSystemType)
        {
            var result = await _hrIntegrationService.SyncEmployeeDataAsync(hrSystemType);
            if (result)
                return Ok(new { message = "HR data sync completed successfully" });
            
            return BadRequest(new { message = "HR data sync failed" });
        }

        [HttpGet("employees/{hrSystemType}")]
        public async Task<IActionResult> GetEmployees(string hrSystemType)
        {
            var employees = await _hrIntegrationService.GetEmployeesFromHrSystemAsync(hrSystemType);
            return Ok(employees);
        }

        [HttpGet("employees/{hrSystemType}/{employeeId}")]
        public async Task<IActionResult> GetEmployee(string hrSystemType, string employeeId)
        {
            var employee = await _hrIntegrationService.GetEmployeeFromHrSystemAsync(hrSystemType, employeeId);
            if (employee == null)
                return NotFound();
            
            return Ok(employee);
        }

        [HttpPut("employees/{hrSystemType}/{employeeId}")]
        public async Task<IActionResult> UpdateEmployee(string hrSystemType, string employeeId, [FromBody] HrEmployeeDto employee)
        {
            var result = await _hrIntegrationService.UpdateEmployeeInHrSystemAsync(hrSystemType, employeeId, employee);
            if (result)
                return Ok(new { message = "Employee updated successfully" });
            
            return BadRequest(new { message = "Failed to update employee" });
        }

        [HttpPost("employees/{hrSystemType}")]
        public async Task<IActionResult> CreateEmployee(string hrSystemType, [FromBody] HrEmployeeDto employee)
        {
            var result = await _hrIntegrationService.CreateEmployeeInHrSystemAsync(hrSystemType, employee);
            if (result)
                return Ok(new { message = "Employee created successfully" });
            
            return BadRequest(new { message = "Failed to create employee" });
        }

        [HttpDelete("employees/{hrSystemType}/{employeeId}")]
        public async Task<IActionResult> DeactivateEmployee(string hrSystemType, string employeeId)
        {
            var result = await _hrIntegrationService.DeactivateEmployeeInHrSystemAsync(hrSystemType, employeeId);
            if (result)
                return Ok(new { message = "Employee deactivated successfully" });
            
            return BadRequest(new { message = "Failed to deactivate employee" });
        }

        [HttpGet("departments/{hrSystemType}")]
        public async Task<IActionResult> GetDepartments(string hrSystemType)
        {
            var departments = await _hrIntegrationService.GetDepartmentsFromHrSystemAsync(hrSystemType);
            return Ok(departments);
        }

        [HttpGet("positions/{hrSystemType}")]
        public async Task<IActionResult> GetPositions(string hrSystemType)
        {
            var positions = await _hrIntegrationService.GetPositionsFromHrSystemAsync(hrSystemType);
            return Ok(positions);
        }

        [HttpGet("sync-status/{hrSystemType}")]
        public async Task<IActionResult> GetSyncStatus(string hrSystemType)
        {
            var status = await _hrIntegrationService.GetSyncStatusAsync(hrSystemType);
            return Ok(status);
        }

        [HttpGet("test-connection/{hrSystemType}")]
        public async Task<IActionResult> TestConnection(string hrSystemType)
        {
            var isConnected = await _hrIntegrationService.TestConnectionAsync(hrSystemType);
            return Ok(new { isConnected, hrSystemType });
        }
    }
}
