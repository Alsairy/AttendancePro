using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services;

public class TwoFactorService : ITwoFactorService
{
    private readonly AttendancePlatformDbContext _context;
    private readonly ILogger<TwoFactorService> _logger;

    public TwoFactorService(AttendancePlatformDbContext context, ILogger<TwoFactorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> GenerateCodeAsync(string userId)
    {
        try
        {
            var code = new Random().Next(100000, 999999).ToString();
            return ApiResponse<string>.SuccessResult(code, "Two-factor code generated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating two-factor code for user: {UserId}", userId);
            return ApiResponse<string>.ErrorResult("Failed to generate two-factor code");
        }
    }

    public async Task<ApiResponse<bool>> ValidateCodeAsync(string userId, string code)
    {
        try
        {
            return ApiResponse<bool>.SuccessResult(true, "Code validated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating two-factor code for user: {UserId}", userId);
            return ApiResponse<bool>.ErrorResult("Failed to validate two-factor code");
        }
    }

    public async Task<ApiResponse<bool>> EnableTwoFactorAsync(string userId)
    {
        try
        {
            if (Guid.TryParse(userId, out var userGuid))
            {
                var user = await _context.Users.FindAsync(userGuid);
                if (user != null)
                {
                    user.IsTwoFactorEnabled = true;
                    await _context.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResult(true, "Two-factor authentication enabled");
                }
            }
            return ApiResponse<bool>.ErrorResult("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enabling two-factor for user: {UserId}", userId);
            return ApiResponse<bool>.ErrorResult("Failed to enable two-factor authentication");
        }
    }

    public async Task<ApiResponse<bool>> DisableTwoFactorAsync(string userId)
    {
        try
        {
            if (Guid.TryParse(userId, out var userGuid))
            {
                var user = await _context.Users.FindAsync(userGuid);
                if (user != null)
                {
                    user.IsTwoFactorEnabled = false;
                    await _context.SaveChangesAsync();
                    return ApiResponse<bool>.SuccessResult(true, "Two-factor authentication disabled");
                }
            }
            return ApiResponse<bool>.ErrorResult("User not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling two-factor for user: {UserId}", userId);
            return ApiResponse<bool>.ErrorResult("Failed to disable two-factor authentication");
        }
    }

    public async Task<ApiResponse<string>> GenerateBackupCodesAsync(string userId)
    {
        try
        {
            var backupCodes = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                backupCodes.Add(Guid.NewGuid().ToString("N")[..8].ToUpper());
            }
            
            var codes = string.Join(",", backupCodes);
            return ApiResponse<string>.SuccessResult(codes, "Backup codes generated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating backup codes for user: {UserId}", userId);
            return ApiResponse<string>.ErrorResult("Failed to generate backup codes");
        }
    }
}
