using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveDigitalTransformationController : ControllerBase
    {
        private readonly IComprehensiveDigitalTransformationService _digitalService;
        private readonly ILogger<ComprehensiveDigitalTransformationController> _logger;

        public ComprehensiveDigitalTransformationController(
            IComprehensiveDigitalTransformationService digitalService,
            ILogger<ComprehensiveDigitalTransformationController> logger)
        {
            _digitalService = digitalService;
            _logger = logger;
        }

        [HttpGet("digital-maturity")]
        public async Task<IActionResult> GetDigitalMaturity()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var maturity = await _digitalService.GetDigitalMaturityAsync(tenantId);
                return Ok(maturity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting digital maturity");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("automation-analysis")]
        public async Task<IActionResult> GetAutomationAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var automation = await _digitalService.GetAutomationAnalysisAsync(tenantId);
                return Ok(automation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting automation analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cloud-adoption")]
        public async Task<IActionResult> GetCloudAdoption()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var cloud = await _digitalService.GetCloudAdoptionAsync(tenantId);
                return Ok(cloud);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cloud adoption");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("data-digitization")]
        public async Task<IActionResult> GetDataDigitization()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var digitization = await _digitalService.GetDataDigitizationAsync(tenantId);
                return Ok(digitization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting data digitization");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("process-optimization")]
        public async Task<IActionResult> GetProcessOptimization()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var optimization = await _digitalService.GetProcessOptimizationAsync(tenantId);
                return Ok(optimization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting process optimization");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("technology-investment")]
        public async Task<IActionResult> GetTechnologyInvestment()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var investment = await _digitalService.GetTechnologyInvestmentAsync(tenantId);
                return Ok(investment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting technology investment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("digital-skills")]
        public async Task<IActionResult> GetDigitalSkills()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var skills = await _digitalService.GetDigitalSkillsAsync(tenantId);
                return Ok(skills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting digital skills");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("digital-strategy")]
        public async Task<IActionResult> GetDigitalStrategy()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var strategy = await _digitalService.GetDigitalStrategyAsync(tenantId);
                return Ok(strategy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting digital strategy");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("change-management")]
        public async Task<IActionResult> GetChangeManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var change = await _digitalService.GetChangeManagementAsync(tenantId);
                return Ok(change);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting change management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("digital-metrics")]
        public async Task<IActionResult> GetDigitalMetrics()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var metrics = await _digitalService.GetDigitalMetricsAsync(tenantId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting digital metrics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
