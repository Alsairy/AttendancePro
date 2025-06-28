using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Application.Services;

public interface ITwoFactorService
{
    Task<ApiResponse<string>> GenerateCodeAsync(string userId);
    Task<ApiResponse<bool>> ValidateCodeAsync(string userId, string code);
    Task<ApiResponse<bool>> EnableTwoFactorAsync(string userId);
    Task<ApiResponse<bool>> DisableTwoFactorAsync(string userId);
    Task<ApiResponse<string>> GenerateBackupCodesAsync(string userId);
}
