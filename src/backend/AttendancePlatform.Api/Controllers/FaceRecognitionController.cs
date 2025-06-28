using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly IFaceRecognitionService _faceRecognitionService;
        private readonly ILogger<FaceRecognitionController> _logger;

        public FaceRecognitionController(
            IFaceRecognitionService faceRecognitionService,
            ILogger<FaceRecognitionController> logger)
        {
            _faceRecognitionService = faceRecognitionService;
            _logger = logger;
        }

        [HttpPost("enroll")]
        public async Task<ActionResult<ApiResponse<FaceEnrollmentDto>>> EnrollFace([FromBody] FaceEnrollmentRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<FaceEnrollmentDto>.ErrorResult("User not authenticated"));
            }

            try
            {
                var imageData = Convert.FromBase64String(request.ImageBase64);
                var result = await _faceRecognitionService.EnrollFaceAsync(userId.Value, imageData, request.DeviceId, request.DeviceType);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (FormatException)
            {
                return BadRequest(ApiResponse<FaceEnrollmentDto>.ErrorResult("Invalid base64 image data"));
            }
        }

        [HttpPost("verify")]
        public async Task<ActionResult<ApiResponse<FaceVerificationDto>>> VerifyFace([FromBody] FaceVerificationRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<FaceVerificationDto>.ErrorResult("User not authenticated"));
            }

            try
            {
                var imageData = Convert.FromBase64String(request.ImageBase64);
                var result = await _faceRecognitionService.VerifyFaceAsync(userId.Value, imageData, request.DeviceId, request.DeviceType);
                
                return Ok(result);
            }
            catch (FormatException)
            {
                return BadRequest(ApiResponse<FaceVerificationDto>.ErrorResult("Invalid base64 image data"));
            }
        }

        [HttpPost("identify")]
        public async Task<ActionResult<ApiResponse<FaceIdentificationDto>>> IdentifyFace([FromBody] FaceIdentificationRequest request)
        {
            try
            {
                var imageData = Convert.FromBase64String(request.ImageBase64);
                var result = await _faceRecognitionService.IdentifyFaceAsync(imageData, request.DeviceId, request.DeviceType);
                
                return Ok(result);
            }
            catch (FormatException)
            {
                return BadRequest(ApiResponse<FaceIdentificationDto>.ErrorResult("Invalid base64 image data"));
            }
        }

        [HttpGet("my-templates")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BiometricTemplateDto>>>> GetMyTemplates()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<IEnumerable<BiometricTemplateDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _faceRecognitionService.GetUserTemplatesAsync(userId.Value);
            return Ok(result);
        }

        [HttpDelete("templates/{templateId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTemplate(Guid templateId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _faceRecognitionService.DeleteTemplateAsync(userId.Value, templateId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("templates/{templateId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateTemplate(Guid templateId, [FromBody] UpdateTemplateRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            try
            {
                var imageData = Convert.FromBase64String(request.ImageBase64);
                var result = await _faceRecognitionService.UpdateTemplateAsync(userId.Value, templateId, imageData);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (FormatException)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Invalid base64 image data"));
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("sub")?.Value ??
                             User.FindFirst("userId")?.Value;
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            
            return null;
        }
    }

    public class FaceEnrollmentRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
    }

    public class FaceVerificationRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
    }

    public class FaceIdentificationRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty;
    }

    public class UpdateTemplateRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
    }
}
