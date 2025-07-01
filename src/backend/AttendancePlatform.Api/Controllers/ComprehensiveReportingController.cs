using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ComprehensiveReportingController : ControllerBase
    {
        private readonly IComprehensiveReportingService _reportingService;
        private readonly ILogger<ComprehensiveReportingController> _logger;

        public ComprehensiveReportingController(
            IComprehensiveReportingService reportingService,
            ILogger<ComprehensiveReportingController> logger)
        {
            _reportingService = reportingService;
            _logger = logger;
        }

        [HttpGet("executive")]
        public async Task<ActionResult<ExecutiveReportDto>> GetExecutiveReport(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportingService.GenerateExecutiveReportAsync(tenantId, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating executive report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("compliance")]
        public async Task<ActionResult<ComplianceReportDto>> GetComplianceReport([FromQuery] Guid tenantId)
        {
            try
            {
                var report = await _reportingService.GenerateComplianceReportAsync(tenantId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating compliance report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("performance")]
        public async Task<ActionResult<PerformanceReportDto>> GetPerformanceReport(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportingService.GeneratePerformanceReportAsync(tenantId, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating performance report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("financial")]
        public async Task<ActionResult<FinancialReportDto>> GetFinancialReport(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportingService.GenerateFinancialReportAsync(tenantId, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating financial report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("security")]
        public async Task<ActionResult<SecurityReportDto>> GetSecurityReport([FromQuery] Guid tenantId)
        {
            try
            {
                var report = await _reportingService.GenerateSecurityReportAsync(tenantId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating security report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("audit")]
        public async Task<ActionResult<AuditReportDto>> GetAuditReport(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var report = await _reportingService.GenerateAuditReportAsync(tenantId, fromDate, toDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating audit report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("custom")]
        public async Task<ActionResult<CustomReportDto>> GenerateCustomReport(
            [FromQuery] Guid tenantId,
            [FromBody] ComprehensiveCustomReportRequestDto request)
        {
            try
            {
                var report = await _reportingService.GenerateCustomReportAsync(tenantId, request);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating custom report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("templates")]
        public async Task<ActionResult<List<ReportTemplateDto>>> GetReportTemplates([FromQuery] Guid tenantId)
        {
            try
            {
                var templates = await _reportingService.GetReportTemplatesAsync(tenantId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report templates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("export/{reportId}")]
        public async Task<ActionResult> ExportReport(Guid reportId, [FromQuery] string format = "pdf")
        {
            try
            {
                var data = await _reportingService.ExportReportAsync(reportId, format);
                return File(data, "application/octet-stream", $"report.{format}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("schedule")]
        public async Task<ActionResult> ScheduleReport(
            [FromQuery] Guid tenantId,
            [FromBody] ScheduledReportDto scheduledReport)
        {
            try
            {
                var result = await _reportingService.ScheduleReportAsync(tenantId, scheduledReport);
                if (!result)
                    return BadRequest("Failed to schedule report");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling report");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
