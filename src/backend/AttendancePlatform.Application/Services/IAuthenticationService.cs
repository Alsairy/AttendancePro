using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IAuthenticationService
{
    Task<ApiResponse<Application.DTOs.LoginResponse>> LoginAsync(Application.DTOs.LoginRequest request);
    Task<ApiResponse<Application.DTOs.UserDto>> RegisterAsync(Application.DTOs.RegisterRequest request);
    Task<ApiResponse<Application.DTOs.LoginResponse>> RefreshTokenAsync(Application.DTOs.RefreshTokenRequest request);
    Task<ApiResponse<bool>> LogoutAsync(string userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(Application.DTOs.ChangePasswordRequest request);
    Task<ApiResponse<bool>> ForgotPasswordAsync(Application.DTOs.ForgotPasswordRequest request);
    Task<ApiResponse<bool>> ResetPasswordAsync(Application.DTOs.ResetPasswordRequest request);
    Task<ApiResponse<Application.DTOs.UserDto>> GetCurrentUserAsync(string userId);
}
