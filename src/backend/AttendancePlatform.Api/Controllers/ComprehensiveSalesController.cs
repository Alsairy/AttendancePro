using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveSalesController : ControllerBase
    {
        private readonly IComprehensiveSalesService _salesService;
        private readonly ILogger<ComprehensiveSalesController> _logger;

        public ComprehensiveSalesController(
            IComprehensiveSalesService salesService,
            ILogger<ComprehensiveSalesController> logger)
        {
            _salesService = salesService;
            _logger = logger;
        }

        [HttpGet("pipeline-management")]
        public async Task<IActionResult> GetPipelineManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var pipeline = await _salesService.GetPipelineManagementAsync(tenantId);
                return Ok(pipeline);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pipeline management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lead-qualification")]
        public async Task<IActionResult> GetLeadQualification()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var qualification = await _salesService.GetLeadQualificationAsync(tenantId);
                return Ok(qualification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lead qualification");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("opportunity-management")]
        public async Task<IActionResult> GetOpportunityManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var opportunities = await _salesService.GetOpportunityManagementAsync(tenantId);
                return Ok(opportunities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting opportunity management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sales-forecasting")]
        public async Task<IActionResult> GetSalesForecasting()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var forecasting = await _salesService.GetSalesForecastingAsync(tenantId);
                return Ok(forecasting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales forecasting");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("territory-management")]
        public async Task<IActionResult> GetTerritoryManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var territory = await _salesService.GetTerritoryManagementAsync(tenantId);
                return Ok(territory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting territory management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sales-analytics")]
        public async Task<IActionResult> GetSalesAnalytics()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analytics = await _salesService.GetSalesAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("commission-management")]
        public async Task<IActionResult> GetCommissionManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var commission = await _salesService.GetCommissionManagementAsync(tenantId);
                return Ok(commission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting commission management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sales-reports")]
        public async Task<IActionResult> GetSalesReports()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _salesService.GetSalesReportsAsync(tenantId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales reports");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sales-performance")]
        public async Task<IActionResult> GetSalesPerformance()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var performance = await _salesService.GetSalesPerformanceAsync(tenantId);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sales performance");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
