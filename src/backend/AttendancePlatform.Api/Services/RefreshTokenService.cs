using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AttendancePlatformDbContext _context;
    private readonly ILogger<RefreshTokenService> _logger;

    public RefreshTokenService(AttendancePlatformDbContext context, ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<string>> GenerateRefreshTokenAsync(string userId)
    {
        try
        {
            var refreshToken = Guid.NewGuid().ToString();
            
            if (Guid.TryParse(userId, out var userGuid))
            {
                var user = await _context.Users.FindAsync(userGuid);
                if (user != null)
                {
                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _context.SaveChangesAsync();
                }
            }

            return ApiResponse<string>.SuccessResult(refreshToken, "Refresh token generated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating refresh token for user: {UserId}", userId);
            return ApiResponse<string>.ErrorResult("Failed to generate refresh token");
        }
    }

    public async Task<ApiResponse<bool>> ValidateRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ApiResponse<bool>.ErrorResult("Invalid or expired refresh token");
            }

            return ApiResponse<bool>.SuccessResult(true, "Refresh token is valid");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token");
            return ApiResponse<bool>.ErrorResult("Failed to validate refresh token");
        }
    }

    public async Task<ApiResponse<bool>> RevokeRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _context.SaveChangesAsync();
            }

            return ApiResponse<bool>.SuccessResult(true, "Refresh token revoked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token");
            return ApiResponse<bool>.ErrorResult("Failed to revoke refresh token");
        }
    }

    public async Task<ApiResponse<bool>> RevokeAllUserTokensAsync(string userId)
    {
        try
        {
            if (Guid.TryParse(userId, out var userGuid))
            {
                var user = await _context.Users.FindAsync(userGuid);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiryTime = null;
                    await _context.SaveChangesAsync();
                }
            }

            return ApiResponse<bool>.SuccessResult(true, "All user tokens revoked");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user: {UserId}", userId);
            return ApiResponse<bool>.ErrorResult("Failed to revoke user tokens");
        }
    }
}
