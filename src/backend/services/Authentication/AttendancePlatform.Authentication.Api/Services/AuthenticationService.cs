using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Authentication.Api.Services;

namespace AttendancePlatform.Authentication.Api.Services
{
    public interface IAuthenticationService
    {
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ApiResponse<bool>> LogoutAsync(string userId);
        Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(string userId);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ITenantContext _tenantContext;
        private readonly ITwoFactorService _twoFactorService;
        private readonly IEmailService _emailService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthenticationService(
            AttendancePlatformDbContext context,
            IJwtTokenService jwtTokenService,
            ILogger<AuthenticationService> logger,
            ITenantContext tenantContext,
            ITwoFactorService twoFactorService,
            IEmailService emailService,
            IRefreshTokenService refreshTokenService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _tenantContext = tenantContext;
            _twoFactorService = twoFactorService;
            _emailService = emailService;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Find user by email
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid email or password");
                }

                // Check if user is active
                if (user.Status != UserStatus.Active)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("User account is not active");
                }

                // Check if account is locked
                if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Account is temporarily locked");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    await _context.SaveChangesAsync();
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid email or password");
                }

                // Check if 2FA is required
                if (user.IsTwoFactorEnabled && string.IsNullOrEmpty(request.TwoFactorCode))
                {
                    return ApiResponse<LoginResponse>.SuccessResult(new LoginResponse
                    {
                        RequiresTwoFactor = true
                    });
                }

                // Validate 2FA code if provided
                if (user.IsTwoFactorEnabled && !string.IsNullOrEmpty(request.TwoFactorCode))
                {
                    if (string.IsNullOrEmpty(user.TwoFactorSecret))
                    {
                        return ApiResponse<LoginResponse>.ErrorResult("Two-factor authentication is not properly configured");
                    }

                    var isValidCode = await _twoFactorService.ValidateCodeAsync(user.TwoFactorSecret, request.TwoFactorCode);
                    if (!isValidCode)
                    {
                        return ApiResponse<LoginResponse>.ErrorResult("Invalid two-factor authentication code");
                    }
                }

                user.LockedUntil = null;
                user.LastLoginAt = DateTime.UtcNow;

                // Get user roles and permissions
                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();

                // Generate tokens
                var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, roles, permissions);
                var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Status = user.Status.ToString(),
                    Roles = roles
                };

                var response = new LoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = userDto,
                    RequiresTwoFactor = false
                };

                return ApiResponse<LoginResponse>.SuccessResult(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return ApiResponse<LoginResponse>.ErrorResult("An error occurred during login");
            }
        }

        public async Task<ApiResponse<UserDto>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate password confirmation
                if (request.Password != request.ConfirmPassword)
                {
                    return ApiResponse<UserDto>.ErrorResult("Password and confirm password do not match");
                }

                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.ErrorResult("User with this email already exists");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user with default tenant (for now, until multi-tenancy is fully configured)
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    EmployeeId = request.EmployeeId,
                    Department = request.Department,
                    Position = request.Position,
                    PasswordHash = passwordHash,
                    Status = UserStatus.Active,
                    TenantId = _tenantContext.TenantId ?? Guid.NewGuid() // Use default tenant for now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    Position = user.Position,
                    Status = user.Status.ToString(),
                    Roles = new List<string>()
                };

                return ApiResponse<UserDto>.SuccessResult(userDto, "User registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return ApiResponse<UserDto>.ErrorResult("An error occurred during registration");
            }
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var refreshToken = await _refreshTokenService.GetRefreshTokenAsync(request.RefreshToken);
                if (refreshToken == null)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid refresh token");
                }

                // Validate the refresh token
                var isValid = await _refreshTokenService.ValidateRefreshTokenAsync(request.RefreshToken, refreshToken.UserId);
                if (!isValid)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("Invalid or expired refresh token");
                }

                // Get user with roles and permissions
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                            .ThenInclude(r => r.RolePermissions)
                                .ThenInclude(rp => rp.Permission)
                    .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);

                if (user == null || user.Status != UserStatus.Active)
                {
                    return ApiResponse<LoginResponse>.ErrorResult("User not found or inactive");
                }

                // Get user roles and permissions
                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
                var permissions = user.UserRoles
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();

                // Generate new access token
                var newAccessToken = await _jwtTokenService.GenerateAccessTokenAsync(user, roles, permissions);
                
                await _refreshTokenService.RevokeRefreshTokenAsync(request.RefreshToken);
                var newRefreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Status = user.Status.ToString(),
                    Roles = roles
                };

                var response = new LoginResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                    User = userDto,
                    RequiresTwoFactor = false
                };

                return ApiResponse<LoginResponse>.SuccessResult(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                return ApiResponse<LoginResponse>.ErrorResult("An error occurred while refreshing token");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string userId)
        {
            try
            {
                // Revoke all refresh tokens for the user
                var success = await _refreshTokenService.RevokeAllUserRefreshTokensAsync(Guid.Parse(userId));
                
                if (success)
                {
                    _logger.LogInformation("User {UserId} logged out successfully", userId);
                    return ApiResponse<bool>.SuccessResult(true, "Logout successful");
                }
                else
                {
                    _logger.LogWarning("Failed to revoke refresh tokens for user {UserId}", userId);
                    return ApiResponse<bool>.SuccessResult(true, "Logout completed with warnings");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
                return ApiResponse<bool>.ErrorResult("An error occurred during logout");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(request.UserId));
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResult("User not found");
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                {
                    return ApiResponse<bool>.ErrorResult("Current password is incorrect");
                }

                // Hash new password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.PasswordChangedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user: {UserId}", request.UserId);
                return ApiResponse<bool>.ErrorResult("An error occurred while changing password");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            try
            {
                var resetToken = await _context.PasswordResetTokens
                    .Include(prt => prt.User)
                    .FirstOrDefaultAsync(prt => prt.Token == request.Token);

                if (resetToken == null)
                {
                    return ApiResponse<bool>.ErrorResult("Invalid password reset token");
                }

                if (!resetToken.IsValid)
                {
                    return ApiResponse<bool>.ErrorResult("Password reset token has expired or been used");
                }

                var user = resetToken.User;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                user.PasswordChangedAt = DateTime.UtcNow;
                user.RequirePasswordChange = false;

                resetToken.IsUsed = true;
                resetToken.UsedAt = DateTime.UtcNow;

                // Revoke all refresh tokens for security
                await _refreshTokenService.RevokeAllUserRefreshTokensAsync(user.Id);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Password reset successfully for user {UserId}", user.Id);
                return ApiResponse<bool>.SuccessResult(true, "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return ApiResponse<bool>.ErrorResult("An error occurred while resetting password");
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                // Find user by email
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
                    return ApiResponse<bool>.SuccessResult(true, "If the email exists, a password reset link has been sent");
                }

                // Check if user is active
                if (user.Status != UserStatus.Active)
                {
                    _logger.LogWarning("Password reset requested for inactive user: {UserId}", user.Id);
                    return ApiResponse<bool>.SuccessResult(true, "If the email exists, a password reset link has been sent");
                }

                // Generate password reset token
                var resetToken = new PasswordResetToken
                {
                    Token = GenerateSecureToken(),
                    UserId = user.Id,
                    ExpiresAt = DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                    IsUsed = false
                };

                // Revoke any existing unused password reset tokens for this user
                var existingTokens = await _context.PasswordResetTokens
                    .Where(prt => prt.UserId == user.Id && !prt.IsUsed && prt.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                foreach (var existingToken in existingTokens)
                {
                    existingToken.IsUsed = true;
                    existingToken.UsedAt = DateTime.UtcNow;
                }

                _context.PasswordResetTokens.Add(resetToken);
                await _context.SaveChangesAsync();

                var resetUrl = "https://app.hudur.sa/reset-password"; // This should come from configuration
                var emailSent = await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken.Token, resetUrl);

                if (!emailSent)
                {
                    _logger.LogError("Failed to send password reset email to {Email}", user.Email);
                }

                _logger.LogInformation("Password reset token generated for user {UserId}", user.Id);
                return ApiResponse<bool>.SuccessResult(true, "If the email exists, a password reset link has been sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request for email: {Email}", request.Email);
                return ApiResponse<bool>.ErrorResult("An error occurred while processing your request");
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

        public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

                if (user == null)
                {
                    return ApiResponse<UserDto>.ErrorResult("User not found");
                }

                var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmployeeId = user.EmployeeId,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Status = user.Status.ToString(),
                    Roles = roles
                };

                return ApiResponse<UserDto>.SuccessResult(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user: {UserId}", userId);
                return ApiResponse<UserDto>.ErrorResult("An error occurred while getting user information");
            }
        }
    }

    // Additional DTOs for authentication
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}

