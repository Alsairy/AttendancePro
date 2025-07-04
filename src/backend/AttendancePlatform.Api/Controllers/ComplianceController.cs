using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ComplianceController : ControllerBase
    {
        private readonly IComplianceService _complianceService;

        public ComplianceController(IComplianceService complianceService)
        {
            _complianceService = complianceService;
        }

        [HttpGet("owasp-validation")]
        public async Task<IActionResult> ValidateOWASPCompliance()
        {
            var isCompliant = await _complianceService.ValidateOWASPCompliance();
            return Ok(new { isCompliant, message = isCompliant ? "OWASP compliant" : "OWASP compliance issues detected" });
        }

        [HttpGet("policy-validation")]
        public async Task<IActionResult> ValidateCybersecurityPolicy()
        {
            var isCompliant = await _complianceService.ValidateCybersecurityPolicy();
            return Ok(new { isCompliant, message = isCompliant ? "Policy compliant" : "Policy compliance issues detected" });
        }

        [HttpGet("report")]
        public async Task<IActionResult> GenerateComplianceReport()
        {
            var report = await _complianceService.GenerateComplianceReport();
            return Ok(new { report, generatedAt = DateTime.UtcNow });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "compliance", timestamp = DateTime.UtcNow });
        }
    }
}
