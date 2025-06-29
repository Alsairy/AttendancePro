using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;

        public SecurityController(ISecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpPost("validate-headers")]
        public async Task<IActionResult> ValidateSecurityHeaders()
        {
            var isValid = await _securityService.ValidateSecurityHeaders(HttpContext);
            return Ok(new { isValid, message = isValid ? "Security headers valid" : "Security headers missing" });
        }

        [HttpPost("validate-input")]
        public async Task<IActionResult> ValidateInputSanitization([FromBody] string input)
        {
            var isValid = await _securityService.ValidateInputSanitization(input);
            return Ok(new { isValid, message = isValid ? "Input is safe" : "Malicious input detected" });
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "security", timestamp = DateTime.UtcNow });
        }
    }
}
