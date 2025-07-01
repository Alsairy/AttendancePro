using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnterpriseIntegrationController : ControllerBase
    {
        private readonly IEnterpriseIntegrationService _integrationService;
        private readonly ILogger<EnterpriseIntegrationController> _logger;

        public EnterpriseIntegrationController(
            IEnterpriseIntegrationService integrationService,
            ILogger<EnterpriseIntegrationController> logger)
        {
            _integrationService = integrationService;
            _logger = logger;
        }

        [HttpPost("sap")]
        public async Task<ActionResult<SapIntegrationDto>> ConnectToSap(
            [FromQuery] Guid tenantId,
            [FromBody] SapConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToSapAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to SAP");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("oracle")]
        public async Task<ActionResult<OracleIntegrationDto>> ConnectToOracle(
            [FromQuery] Guid tenantId,
            [FromBody] OracleConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToOracleAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Oracle");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("salesforce")]
        public async Task<ActionResult<SalesforceIntegrationDto>> ConnectToSalesforce(
            [FromQuery] Guid tenantId,
            [FromBody] SalesforceConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToSalesforceAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Salesforce");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("workday")]
        public async Task<ActionResult<WorkdayIntegrationDto>> ConnectToWorkday(
            [FromQuery] Guid tenantId,
            [FromBody] WorkdayConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToWorkdayAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Workday");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("successfactors")]
        public async Task<ActionResult<SuccessFactorsIntegrationDto>> ConnectToSuccessFactors(
            [FromQuery] Guid tenantId,
            [FromBody] SuccessFactorsConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToSuccessFactorsAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to SuccessFactors");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("bamboohr")]
        public async Task<ActionResult<BambooHrIntegrationDto>> ConnectToBambooHr(
            [FromQuery] Guid tenantId,
            [FromBody] BambooHrConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToBambooHrAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to BambooHR");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("adp")]
        public async Task<ActionResult<AdpIntegrationDto>> ConnectToAdp(
            [FromQuery] Guid tenantId,
            [FromBody] AdpConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToAdpAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to ADP");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("paychex")]
        public async Task<ActionResult<PaychexIntegrationDto>> ConnectToPaychex(
            [FromQuery] Guid tenantId,
            [FromBody] PaychexConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToPaychexAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Paychex");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ultimate")]
        public async Task<ActionResult<UltimateIntegrationDto>> ConnectToUltimate(
            [FromQuery] Guid tenantId,
            [FromBody] UltimateConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToUltimateAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Ultimate Software");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("cornerstone")]
        public async Task<ActionResult<CornerstoneIntegrationDto>> ConnectToCornerstone(
            [FromQuery] Guid tenantId,
            [FromBody] CornerstoneConnectionDto connection)
        {
            try
            {
                var result = await _integrationService.ConnectToCornerstoneAsync(tenantId, connection);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Cornerstone");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<List<IntegrationStatusDto>>> GetIntegrationStatus([FromQuery] Guid tenantId)
        {
            try
            {
                var status = await _integrationService.GetIntegrationStatusAsync(tenantId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting integration status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("sync/{integrationName}")]
        public async Task<ActionResult> SyncData([FromQuery] Guid tenantId, string integrationName)
        {
            try
            {
                var result = await _integrationService.SyncDataAsync(tenantId, integrationName);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public async Task<ActionResult<IntegrationHealthDto>> GetIntegrationHealth([FromQuery] Guid tenantId)
        {
            try
            {
                var health = await _integrationService.GetIntegrationHealthAsync(tenantId);
                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting integration health");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{integrationName}")]
        public async Task<ActionResult> DisconnectIntegration([FromQuery] Guid tenantId, string integrationName)
        {
            try
            {
                var result = await _integrationService.DisconnectIntegrationAsync(tenantId, integrationName);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disconnecting integration");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("logs/{integrationName}")]
        public async Task<ActionResult<List<IntegrationLogDto>>> GetIntegrationLogs(
            [FromQuery] Guid tenantId,
            string integrationName)
        {
            try
            {
                var logs = await _integrationService.GetIntegrationLogsAsync(tenantId, integrationName);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting integration logs");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
