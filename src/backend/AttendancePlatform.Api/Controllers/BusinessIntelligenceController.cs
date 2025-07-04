using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/business-intelligence")]
    [Authorize]
    public class BusinessIntelligenceController : ControllerBase
    {
        private readonly IBusinessIntelligenceService _businessIntelligenceService;
        private readonly ILogger<BusinessIntelligenceController> _logger;

        public BusinessIntelligenceController(
            IBusinessIntelligenceService businessIntelligenceService,
            ILogger<BusinessIntelligenceController> logger)
        {
            _businessIntelligenceService = businessIntelligenceService;
            _logger = logger;
        }

        [HttpGet("executive-dashboard")]
        public async Task<ActionResult<BusinessIntelligenceReportDto>> GetExecutiveDashboard([FromQuery] Guid tenantId)
        {
            try
            {
                var dashboard = await _businessIntelligenceService.GenerateExecutiveDashboardAsync(tenantId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting executive dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("kpi-metrics")]
        public async Task<ActionResult<List<KpiMetricDto>>> GetKpiMetrics([FromQuery] Guid tenantId)
        {
            try
            {
                var metrics = await _businessIntelligenceService.GetKpiMetricsAsync(tenantId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPI metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("trend-analysis")]
        public async Task<ActionResult<List<TrendAnalysisDto>>> GetTrendAnalysis(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var trends = await _businessIntelligenceService.GetTrendAnalysisAsync(tenantId, fromDate, toDate);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trend analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("predictive-insights")]
        public async Task<ActionResult<List<PredictiveInsightDto>>> GetPredictiveInsights([FromQuery] Guid tenantId)
        {
            try
            {
                var insights = await _businessIntelligenceService.GetPredictiveInsightsAsync(tenantId);
                return Ok(insights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictive insights");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("benchmark-comparison")]
        public async Task<ActionResult<List<BenchmarkComparisonDto>>> GetBenchmarkComparison([FromQuery] Guid tenantId)
        {
            try
            {
                var comparison = await _businessIntelligenceService.GetBenchmarkComparisonAsync(tenantId);
                return Ok(comparison);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting benchmark comparison");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("risk-assessment")]
        public async Task<ActionResult<List<RiskAssessmentDto>>> GetRiskAssessment([FromQuery] Guid tenantId)
        {
            try
            {
                var risks = await _businessIntelligenceService.GetRiskAssessmentAsync(tenantId);
                return Ok(risks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting risk assessment");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("opportunity-analysis")]
        public async Task<ActionResult<List<OpportunityAnalysisDto>>> GetOpportunityAnalysis([FromQuery] Guid tenantId)
        {
            try
            {
                var opportunities = await _businessIntelligenceService.GetOpportunityAnalysisAsync(tenantId);
                return Ok(opportunities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting opportunity analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("performance-metrics")]
        public async Task<ActionResult<List<PerformanceMetricDto>>> GetPerformanceMetrics([FromQuery] Guid tenantId)
        {
            try
            {
                var metrics = await _businessIntelligenceService.GetPerformanceMetricsAsync(tenantId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting performance metrics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("resource-optimization")]
        public async Task<ActionResult<List<ResourceOptimizationDto>>> GetResourceOptimization([FromQuery] Guid tenantId)
        {
            try
            {
                var optimization = await _businessIntelligenceService.GetResourceOptimizationAsync(tenantId);
                return Ok(optimization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting resource optimization");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("competitive-analysis")]
        public async Task<ActionResult<List<CompetitiveAnalysisDto>>> GetCompetitiveAnalysis([FromQuery] Guid tenantId)
        {
            try
            {
                var analysis = await _businessIntelligenceService.GetCompetitiveAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting competitive analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("kpis")]
        public async Task<IActionResult> GetBusinessIntelligenceKPIs()
        {
            try
            {
                var kpis = new
                {
                    revenue_growth = 15.8,
                    customer_satisfaction = 4.6,
                    employee_productivity = 87.3,
                    operational_efficiency = 92.1,
                    market_share = 23.4,
                    kpi_trends = new
                    {
                        revenue = new[] { 2.1, 3.5, 4.2, 5.8, 7.1, 8.9, 10.2, 12.5, 13.8, 15.8 },
                        satisfaction = new[] { 4.1, 4.2, 4.3, 4.4, 4.5, 4.5, 4.6, 4.6, 4.6, 4.6 },
                        productivity = new[] { 82.1, 83.5, 84.8, 85.9, 86.2, 86.8, 87.0, 87.1, 87.2, 87.3 }
                    }
                };
                return Ok(kpis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business intelligence KPIs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
    }
}
