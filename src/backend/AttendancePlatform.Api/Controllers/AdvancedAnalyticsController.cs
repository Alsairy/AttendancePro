using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdvancedAnalyticsController : ControllerBase
    {
        private readonly IAdvancedAnalyticsService _analyticsService;
        private readonly ILogger<AdvancedAnalyticsController> _logger;

        public AdvancedAnalyticsController(
            IAdvancedAnalyticsService analyticsService,
            ILogger<AdvancedAnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        [HttpGet("predictive")]
        public async Task<ActionResult<PredictiveAnalyticsDto>> GetPredictiveAnalytics([FromQuery] Guid tenantId)
        {
            try
            {
                var analytics = await _analyticsService.GetPredictiveAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting predictive analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("behavioral")]
        public async Task<ActionResult<BehavioralAnalyticsDto>> GetBehavioralAnalytics([FromQuery] Guid tenantId)
        {
            try
            {
                var analytics = await _analyticsService.GetBehavioralAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting behavioral analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sentiment")]
        public async Task<ActionResult<SentimentAnalysisDto>> GetSentimentAnalysis([FromQuery] Guid tenantId)
        {
            try
            {
                var analysis = await _analyticsService.GetSentimentAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sentiment analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("anomaly")]
        public async Task<ActionResult<AnomalyDetectionDto>> GetAnomalyDetection([FromQuery] Guid tenantId)
        {
            try
            {
                var detection = await _analyticsService.GetAnomalyDetectionAsync(tenantId);
                return Ok(detection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting anomaly detection");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("forecasting")]
        public async Task<ActionResult<ForecastingDto>> GetForecasting([FromQuery] Guid tenantId, [FromQuery] int daysAhead = 30)
        {
            try
            {
                var forecasting = await _analyticsService.GetForecastingAsync(tenantId, daysAhead);
                return Ok(forecasting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting forecasting");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("correlation")]
        public async Task<ActionResult<CorrelationAnalysisDto>> GetCorrelationAnalysis([FromQuery] Guid tenantId)
        {
            try
            {
                var analysis = await _analyticsService.GetCorrelationAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting correlation analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("clustering")]
        public async Task<ActionResult<ClusterAnalysisDto>> GetClusterAnalysis([FromQuery] Guid tenantId)
        {
            try
            {
                var analysis = await _analyticsService.GetClusterAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cluster analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("timeseries")]
        public async Task<ActionResult<TimeSeriesAnalysisDto>> GetTimeSeriesAnalysis(
            [FromQuery] Guid tenantId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            try
            {
                var analysis = await _analyticsService.GetTimeSeriesAnalysisAsync(tenantId, fromDate, toDate);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting time series analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("ml-insights")]
        public async Task<ActionResult<MachineLearningInsightsDto>> GetMachineLearningInsights([FromQuery] Guid tenantId)
        {
            try
            {
                var insights = await _analyticsService.GetMachineLearningInsightsAsync(tenantId);
                return Ok(insights);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ML insights");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("realtime")]
        public async Task<ActionResult<RealTimeAnalyticsDto>> GetRealTimeAnalytics([FromQuery] Guid tenantId)
        {
            try
            {
                var analytics = await _analyticsService.GetRealTimeAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting real-time analytics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
