using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IRefreshTokenService
{
    Task<ApiResponse<string>> GenerateRefreshTokenAsync(string userId);
    Task<ApiResponse<bool>> ValidateRefreshTokenAsync(string refreshToken);
    Task<ApiResponse<bool>> RevokeRefreshTokenAsync(string refreshToken);
    Task<ApiResponse<bool>> RevokeAllUserTokensAsync(string userId);
}
