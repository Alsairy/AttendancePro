using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Application.DTOs;
using ResetPasswordRequest = AttendancePlatform.Application.DTOs.ResetPasswordRequest;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthenticationService authenticationService,
            ILogger<AuthController> logger)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<Application.DTOs.LoginResponse>>> Login([FromBody] Application.DTOs.LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("Invalid request data"));
            }

            var result = await _authenticationService.LoginAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<Application.DTOs.UserDto>>> Register([FromBody] Application.DTOs.RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Application.DTOs.UserDto>.ErrorResult("Invalid request data"));
            }

            var result = await _authenticationService.RegisterAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<ApiResponse<Application.DTOs.LoginResponse>>> RefreshToken([FromBody] Application.DTOs.RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("Invalid request data"));
            }

            var result = await _authenticationService.RefreshTokenAsync(request);
            
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            var result = await _authenticationService.LogoutAsync(userId);
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] Application.DTOs.ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Invalid request data"));
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResult("User not authenticated"));
            }

            request.UserId = userId;
            var result = await _authenticationService.ChangePasswordAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] Application.DTOs.ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Invalid request data"));
            }

            var result = await _authenticationService.ForgotPasswordAsync(request);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] Application.DTOs.ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Invalid request data"));
            }

            var result = await _authenticationService.ResetPasswordAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<Application.DTOs.UserDto>>> GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<Application.DTOs.UserDto>.ErrorResult("User not authenticated"));
            }

            var result = await _authenticationService.GetCurrentUserAsync(userId);
            
            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] Application.DTOs.ValidateTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(ApiResponse<bool>.ErrorResult("Token is required"));
            }

            return Ok(ApiResponse<bool>.SuccessResult(true, "Token validation endpoint"));
        }

        [HttpGet("health")]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
