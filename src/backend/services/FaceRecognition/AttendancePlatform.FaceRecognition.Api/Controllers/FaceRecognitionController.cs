using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.FaceRecognition.Api.Services;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.FaceRecognition.Api.Controllers
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

        /// <summary>
        /// Enroll a new face template for the authenticated user
        /// </summary>
        [HttpPost("enroll")]
        public async Task<ActionResult<ApiResponse<FaceEnrollmentDto>>> EnrollFace([FromBody] FaceEnrollmentRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<FaceEnrollmentDto>.ErrorResult("User not authenticated"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<FaceEnrollmentDto>.ErrorResult("Invalid request data"));
            }

            var result = await _faceRecognitionService.EnrollFaceAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Verify a face against enrolled templates for the authenticated user
        /// </summary>
        [HttpPost("verify")]
        public async Task<ActionResult<ApiResponse<FaceVerificationDto>>> VerifyFace([FromBody] FaceVerificationRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<FaceVerificationDto>.ErrorResult("User not authenticated"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<FaceVerificationDto>.ErrorResult("Invalid request data"));
            }

            var result = await _faceRecognitionService.VerifyFaceAsync(request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Perform liveness detection on a face image
        /// </summary>
        [HttpPost("liveness")]
        public async Task<ActionResult<ApiResponse<LivenessDetectionDto>>> DetectLiveness([FromBody] LivenessDetectionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<LivenessDetectionDto>.ErrorResult("Invalid request data"));
            }

            var result = await _faceRecognitionService.DetectLivenessAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get all face templates for the authenticated user
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BiometricTemplateDto>>>> GetMyFaceTemplates()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<IEnumerable<BiometricTemplateDto>>.ErrorResult("User not authenticated"));
            }

            var result = await _faceRecognitionService.GetUserFaceTemplatesAsync(userId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Delete a specific face template
        /// </summary>
        [HttpDelete("templates/{templateId}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteFaceTemplate(Guid templateId)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _faceRecognitionService.DeleteFaceTemplateAsync(templateId, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Update a face template (activate/deactivate)
        /// </summary>
        [HttpPut("templates/{templateId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateFaceTemplate(Guid templateId, [FromBody] UpdateFaceTemplateRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Invalid request data"));
            }

            var result = await _faceRecognitionService.UpdateFaceTemplateAsync(templateId, request, userId.Value);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Search for face matches across all enrolled users (Admin/Manager only)
        /// </summary>
        [HttpPost("search")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<FaceMatchDto>>> SearchFace([FromBody] FaceMatchRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<FaceMatchDto>.ErrorResult("Invalid request data"));
            }

            var result = await _faceRecognitionService.FindFaceMatchAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Get face templates for a specific user (Admin/Manager only)
        /// </summary>
        [HttpGet("users/{userId}/templates")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ApiResponse<IEnumerable<BiometricTemplateDto>>>> GetUserFaceTemplates(Guid userId)
        {
            var result = await _faceRecognitionService.GetUserFaceTemplatesAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
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
}

// Request/Response DTOs
namespace AttendancePlatform.Shared.Domain.DTOs
{
    public class FaceEnrollmentRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public bool RequireLivenessCheck { get; set; } = true;
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
    }

    public class FaceVerificationRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public bool RequireLivenessCheck { get; set; } = true;
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
    }

    public class LivenessDetectionRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
    }

    public class FaceMatchRequest
    {
        public string ImageBase64 { get; set; } = string.Empty;
        public double? MinConfidence { get; set; }
        public int? MaxResults { get; set; }
    }

    public class UpdateFaceTemplateRequest
    {
        public bool IsActive { get; set; }
    }

    public class FaceEnrollmentDto
    {
        public Guid TemplateId { get; set; }
        public double Quality { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class FaceVerificationDto
    {
        public bool IsVerified { get; set; }
        public double ConfidenceScore { get; set; }
        public Guid? MatchedTemplateId { get; set; }
        public DateTime VerificationTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LivenessDetectionDto
    {
        public bool IsLive { get; set; }
        public double ConfidenceScore { get; set; }
        public DateTime DetectionTime { get; set; }
        public Dictionary<string, bool> Checks { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }

    public class FaceMatchDto
    {
        public bool HasMatches { get; set; }
        public int MatchCount { get; set; }
        public List<FaceMatchResult> Matches { get; set; } = new();
        public DateTime SearchTime { get; set; }
    }

    public class BiometricTemplateDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Quality { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
    }
}

