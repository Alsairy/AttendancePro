using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VoiceRecognitionController : ControllerBase
    {
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly ILogger<VoiceRecognitionController> _logger;

        public VoiceRecognitionController(
            IVoiceRecognitionService voiceRecognitionService,
            ILogger<VoiceRecognitionController> logger)
        {
            _voiceRecognitionService = voiceRecognitionService;
            _logger = logger;
        }

        [HttpPost("enroll")]
        public async Task<ActionResult<VoiceEnrollmentResultDto>> EnrollVoice([FromQuery] Guid userId, [FromBody] byte[] audioData)
        {
            try
            {
                var result = await _voiceRecognitionService.EnrollVoiceAsync(userId, audioData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling voice");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult<VoiceVerificationResultDto>> VerifyVoice([FromQuery] Guid userId, [FromBody] byte[] audioData)
        {
            try
            {
                var result = await _voiceRecognitionService.VerifyVoiceAsync(userId, audioData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying voice");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("templates")]
        public async Task<ActionResult<List<VoiceTemplateDto>>> GetVoiceTemplates([FromQuery] Guid userId)
        {
            try
            {
                var templates = await _voiceRecognitionService.GetVoiceTemplatesAsync(userId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voice templates");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("templates/{templateId}")]
        public async Task<ActionResult> DeleteVoiceTemplate(Guid templateId)
        {
            try
            {
                var result = await _voiceRecognitionService.DeleteVoiceTemplateAsync(templateId);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voice template");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("command")]
        public async Task<ActionResult<VoiceCommandResultDto>> ProcessVoiceCommand([FromQuery] Guid userId, [FromBody] byte[] audioData)
        {
            try
            {
                var result = await _voiceRecognitionService.ProcessVoiceCommandAsync(userId, audioData);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing voice command");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("commands")]
        public async Task<ActionResult<List<VoiceCommandDto>>> GetAvailableCommands()
        {
            try
            {
                var commands = await _voiceRecognitionService.GetAvailableCommandsAsync();
                return Ok(commands);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available commands");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("configuration")]
        public async Task<ActionResult<VoiceConfigurationDto>> GetVoiceConfiguration([FromQuery] Guid tenantId)
        {
            try
            {
                var configuration = await _voiceRecognitionService.GetVoiceConfigurationAsync(tenantId);
                return Ok(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voice configuration");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("configuration")]
        public async Task<ActionResult> UpdateVoiceConfiguration([FromQuery] Guid tenantId, [FromBody] VoiceConfigurationDto configuration)
        {
            try
            {
                var result = await _voiceRecognitionService.UpdateVoiceConfigurationAsync(tenantId, configuration);
                if (!result)
                    return BadRequest("Failed to update configuration");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating voice configuration");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("analytics")]
        public async Task<ActionResult<VoiceAnalyticsDto>> GetVoiceAnalytics([FromQuery] Guid tenantId)
        {
            try
            {
                var analytics = await _voiceRecognitionService.GetVoiceAnalyticsAsync(tenantId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting voice analytics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("train-model")]
        public async Task<ActionResult> TrainVoiceModel([FromQuery] Guid tenantId)
        {
            try
            {
                var result = await _voiceRecognitionService.TrainVoiceModelAsync(tenantId);
                if (!result)
                    return BadRequest("Failed to train voice model");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error training voice model");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
