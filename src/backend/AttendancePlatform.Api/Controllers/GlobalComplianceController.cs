using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GlobalComplianceController : ControllerBase
    {
        private readonly IGlobalComplianceService _complianceService;
        private readonly ILogger<GlobalComplianceController> _logger;

        public GlobalComplianceController(
            IGlobalComplianceService complianceService,
            ILogger<GlobalComplianceController> logger)
        {
            _complianceService = complianceService;
            _logger = logger;
        }

        [HttpGet("gdpr")]
        public async Task<ActionResult<GdprComplianceDto>> GetGdprCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetGdprComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting GDPR compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("saudi-pdpl")]
        public async Task<ActionResult<SaudiPdplComplianceDto>> GetSaudiPdplCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetSaudiPdplComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Saudi PDPL compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("iso27001")]
        public async Task<ActionResult<Iso27001ComplianceDto>> GetIso27001Compliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetIso27001ComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ISO 27001 compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("soc2")]
        public async Task<ActionResult<Soc2ComplianceDto>> GetSoc2Compliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetSoc2ComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting SOC 2 compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("owasp")]
        public async Task<ActionResult<OwaspComplianceDto>> GetOwaspCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetOwaspComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting OWASP compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("hipaa")]
        public async Task<ActionResult<HipaaComplianceDto>> GetHipaaCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetHipaaComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting HIPAA compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("pci-dss")]
        public async Task<ActionResult<PciDssComplianceDto>> GetPciDssCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetPciDssComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting PCI DSS compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("fedramp")]
        public async Task<ActionResult<FedRampComplianceDto>> GetFedRampCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetFedRampComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting FedRAMP compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("nist")]
        public async Task<ActionResult<NistComplianceDto>> GetNistCompliance([FromQuery] Guid tenantId)
        {
            try
            {
                var compliance = await _complianceService.GetNistComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting NIST compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cis-controls")]
        public async Task<ActionResult<CisControlsDto>> GetCisControls([FromQuery] Guid tenantId)
        {
            try
            {
                var controls = await _complianceService.GetCisControlsAsync(tenantId);
                return Ok(controls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting CIS Controls");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("audit")]
        public async Task<ActionResult<ComplianceAuditDto>> GenerateComplianceAudit([FromQuery] Guid tenantId)
        {
            try
            {
                var audit = await _complianceService.GenerateComplianceAuditAsync(tenantId);
                return Ok(audit);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating compliance audit");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("schedule")]
        public async Task<ActionResult> ScheduleComplianceReport(
            [FromQuery] Guid tenantId,
            [FromBody] ComplianceScheduleDto schedule)
        {
            try
            {
                var result = await _complianceService.ScheduleComplianceReportAsync(tenantId, schedule);
                if (!result)
                    return BadRequest("Failed to schedule compliance report");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling compliance report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("violations")]
        public async Task<ActionResult<List<ComplianceViolationDto>>> GetComplianceViolations([FromQuery] Guid tenantId)
        {
            try
            {
                var violations = await _complianceService.GetComplianceViolationsAsync(tenantId);
                return Ok(violations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting compliance violations");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("violations/{violationId}/remediate")]
        public async Task<ActionResult> RemediateViolation([FromQuery] Guid tenantId, Guid violationId)
        {
            try
            {
                var result = await _complianceService.RemediateViolationAsync(tenantId, violationId);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error remediating violation");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("training")]
        public async Task<ActionResult<ComplianceTrainingDto>> GetComplianceTraining([FromQuery] Guid tenantId)
        {
            try
            {
                var training = await _complianceService.GetComplianceTrainingAsync(tenantId);
                return Ok(training);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting compliance training");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
