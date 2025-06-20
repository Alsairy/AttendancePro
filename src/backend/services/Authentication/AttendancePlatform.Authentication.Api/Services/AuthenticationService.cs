using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;

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

        public AuthenticationService(
            AttendancePlatformDbContext context,
            IJwtTokenService jwtTokenService,
            ILogger<AuthenticationService> logger,
            ITenantContext tenantContext)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
            _tenantContext = tenantContext;
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
                    // Increment failed login attempts
                    user.FailedLoginAttempts++;
                    
                    // Lock account if too many failed attempts
                    if (user.FailedLoginAttempts >= 5)
                    {
                        user.LockedUntil = DateTime.UtcNow.AddMinutes(30);
                    }
                    
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
                    // TODO: Implement 2FA validation
                    // For now, we'll skip this validation
                }

                // Reset failed login attempts
                user.FailedLoginAttempts = 0;
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
                var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();

                // Save refresh token (in a real implementation, you'd store this securely)
                // For now, we'll skip storing refresh tokens

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
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

                if (existingUser != null)
                {
                    return ApiResponse<UserDto>.ErrorResult("User with this email already exists");
                }

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create new user
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
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
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
            // TODO: Implement refresh token validation and new token generation
            return ApiResponse<LoginResponse>.ErrorResult("Refresh token functionality not implemented yet");
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string userId)
        {
            try
            {
                // TODO: Invalidate refresh tokens for the user
                // For now, we'll just return success
                return ApiResponse<bool>.SuccessResult(true, "Logout successful");
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
            // TODO: Implement password reset with token validation
            return ApiResponse<bool>.ErrorResult("Password reset functionality not implemented yet");
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            // TODO: Implement forgot password functionality
            return ApiResponse<bool>.ErrorResult("Forgot password functionality not implemented yet");
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

