using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveMarketingController : ControllerBase
    {
        private readonly IComprehensiveMarketingService _marketingService;
        private readonly ILogger<ComprehensiveMarketingController> _logger;

        public ComprehensiveMarketingController(
            IComprehensiveMarketingService marketingService,
            ILogger<ComprehensiveMarketingController> logger)
        {
            _marketingService = marketingService;
            _logger = logger;
        }

        [HttpGet("campaign-management")]
        public async Task<IActionResult> GetCampaignManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var campaigns = await _marketingService.GetCampaignManagementAsync(tenantId);
                return Ok(campaigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting campaign management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("lead-management")]
        public async Task<IActionResult> GetLeadManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var leads = await _marketingService.GetLeadManagementAsync(tenantId);
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lead management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("content-marketing")]
        public async Task<IActionResult> GetContentMarketing()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var content = await _marketingService.GetContentMarketingAsync(tenantId);
                return Ok(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting content marketing");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("social-media")]
        public async Task<IActionResult> GetSocialMedia()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var socialMedia = await _marketingService.GetSocialMediaAsync(tenantId);
                return Ok(socialMedia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting social media");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("email-marketing")]
        public async Task<IActionResult> GetEmailMarketing()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var email = await _marketingService.GetEmailMarketingAsync(tenantId);
                return Ok(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting email marketing");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("marketing-analytics")]
        public async Task<IActionResult> GetMarketingAnalytics()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analytics = await _marketingService.GetMarketingAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting marketing analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("brand-management")]
        public async Task<IActionResult> GetBrandManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var brand = await _marketingService.GetBrandManagementAsync(tenantId);
                return Ok(brand);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting brand management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("market-research")]
        public async Task<IActionResult> GetMarketResearch()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var research = await _marketingService.GetMarketResearchAsync(tenantId);
                return Ok(research);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting market research");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("marketing-roi")]
        public async Task<IActionResult> GetMarketingROI()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var roi = await _marketingService.GetMarketingROIAsync(tenantId);
                return Ok(roi);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting marketing ROI");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
