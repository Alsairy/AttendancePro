using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveCustomerServiceController : ControllerBase
    {
        private readonly IComprehensiveCustomerServiceService _customerServiceService;
        private readonly ILogger<ComprehensiveCustomerServiceController> _logger;

        public ComprehensiveCustomerServiceController(
            IComprehensiveCustomerServiceService customerServiceService,
            ILogger<ComprehensiveCustomerServiceController> logger)
        {
            _customerServiceService = customerServiceService;
            _logger = logger;
        }

        [HttpGet("ticket-management")]
        public async Task<IActionResult> GetTicketManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var tickets = await _customerServiceService.GetTicketManagementAsync(tenantId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ticket management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("customer-satisfaction")]
        public async Task<IActionResult> GetCustomerSatisfaction()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var satisfaction = await _customerServiceService.GetCustomerSatisfactionAsync(tenantId);
                return Ok(satisfaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer satisfaction");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("agent-performance")]
        public async Task<IActionResult> GetAgentPerformance()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var performance = await _customerServiceService.GetAgentPerformanceAsync(tenantId);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting agent performance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("service-analytics")]
        public async Task<IActionResult> GetServiceAnalytics()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analytics = await _customerServiceService.GetServiceAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("knowledge-base")]
        public async Task<IActionResult> GetKnowledgeBase()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var knowledgeBase = await _customerServiceService.GetKnowledgeBaseAsync(tenantId);
                return Ok(knowledgeBase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting knowledge base");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("escalation-management")]
        public async Task<IActionResult> GetEscalationManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var escalation = await _customerServiceService.GetEscalationManagementAsync(tenantId);
                return Ok(escalation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting escalation management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("live-chat-support")]
        public async Task<IActionResult> GetLiveChatSupport()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var liveChat = await _customerServiceService.GetLiveChatSupportAsync(tenantId);
                return Ok(liveChat);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting live chat support");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("customer-feedback")]
        public async Task<IActionResult> GetCustomerFeedback()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var feedback = await _customerServiceService.GetCustomerFeedbackAsync(tenantId);
                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer feedback");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("service-reports")]
        public async Task<IActionResult> GetServiceReports()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _customerServiceService.GetServiceReportsAsync(tenantId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting service reports");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
