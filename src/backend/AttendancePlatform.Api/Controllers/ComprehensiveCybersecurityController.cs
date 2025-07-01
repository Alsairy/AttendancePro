using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveCybersecurityController : ControllerBase
    {
        private readonly IComprehensiveCybersecurityService _cybersecurityService;
        private readonly ILogger<ComprehensiveCybersecurityController> _logger;

        public ComprehensiveCybersecurityController(
            IComprehensiveCybersecurityService cybersecurityService,
            ILogger<ComprehensiveCybersecurityController> logger)
        {
            _cybersecurityService = cybersecurityService;
            _logger = logger;
        }

        [HttpGet("threat-detection")]
        public async Task<IActionResult> GetThreatDetection()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var threats = await _cybersecurityService.GetThreatDetectionAsync(tenantId);
                return Ok(threats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting threat detection");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("vulnerability-assessment")]
        public async Task<IActionResult> GetVulnerabilityAssessment()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var vulnerabilities = await _cybersecurityService.GetVulnerabilityAssessmentAsync(tenantId);
                return Ok(vulnerabilities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vulnerability assessment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("incident-response")]
        public async Task<IActionResult> GetIncidentResponse()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var incidents = await _cybersecurityService.GetIncidentResponseAsync(tenantId);
                return Ok(incidents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incident response");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("security-monitoring")]
        public async Task<IActionResult> GetSecurityMonitoring()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var monitoring = await _cybersecurityService.GetSecurityMonitoringAsync(tenantId);
                return Ok(monitoring);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security monitoring");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("access-control")]
        public async Task<IActionResult> GetAccessControl()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var access = await _cybersecurityService.GetAccessControlAsync(tenantId);
                return Ok(access);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting access control");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("security-training")]
        public async Task<IActionResult> GetSecurityTraining()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var training = await _cybersecurityService.GetSecurityTrainingAsync(tenantId);
                return Ok(training);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security training");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("compliance-management")]
        public async Task<IActionResult> GetComplianceManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var compliance = await _cybersecurityService.GetComplianceManagementAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting compliance management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("security-reports")]
        public async Task<IActionResult> GetSecurityReports()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _cybersecurityService.GetSecurityReportsAsync(tenantId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security reports");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("security-metrics")]
        public async Task<IActionResult> GetSecurityMetrics()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var metrics = await _cybersecurityService.GetSecurityMetricsAsync(tenantId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting security metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cyber-threat-intelligence")]
        public async Task<IActionResult> GetCyberThreatIntelligence()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var intelligence = await _cybersecurityService.GetCyberThreatIntelligenceAsync(tenantId);
                return Ok(intelligence);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cyber threat intelligence");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
