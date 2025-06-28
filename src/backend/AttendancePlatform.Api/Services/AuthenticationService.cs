using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Application.DTOs;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AttendancePlatform.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly ITenantContext _tenantContext;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            AttendancePlatformDbContext context,
            ILogger<AuthenticationService> logger,
            ITenantContext tenantContext,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
            _configuration = configuration;
        }

        public async Task<ApiResponse<Application.DTOs.LoginResponse>> LoginAsync(Application.DTOs.LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    return ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("Account is deactivated");
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.LastLoginDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var response = new Application.DTOs.LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = MapToUserDto(user),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("JWT")["ExpiryInMinutes"] ?? "60"))
                };

                return ApiResponse<Application.DTOs.LoginResponse>.SuccessResult(response, "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
                return ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("An error occurred during login");
            }
        }

        public async Task<ApiResponse<Application.DTOs.UserDto>> RegisterAsync(Application.DTOs.RegisterRequest request)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                {
                    return ApiResponse<Application.DTOs.UserDto>.ErrorResult("User with this email already exists");
                }

                var user = new User
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PasswordHash = HashPassword(request.Password),
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = _tenantContext.TenantId ?? Guid.NewGuid()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return ApiResponse<Application.DTOs.UserDto>.SuccessResult(MapToUserDto(user), "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
                return ApiResponse<Application.DTOs.UserDto>.ErrorResult("An error occurred during registration");
            }
        }

        public async Task<ApiResponse<Application.DTOs.LoginResponse>> RefreshTokenAsync(Application.DTOs.RefreshTokenRequest request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

                if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("Invalid or expired refresh token");
                }

                var token = GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _context.SaveChangesAsync();

                var response = new Application.DTOs.LoginResponse
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    User = MapToUserDto(user),
                    ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetSection("JWT")["ExpiryInMinutes"] ?? "60"))
                };

                return ApiResponse<Application.DTOs.LoginResponse>.SuccessResult(response, "Token refreshed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return ApiResponse<Application.DTOs.LoginResponse>.ErrorResult("An error occurred during token refresh");
            }
        }

        public async Task<ApiResponse<bool>> LogoutAsync(string userId)
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

                return ApiResponse<bool>.SuccessResult(true, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
                return ApiResponse<bool>.ErrorResult("An error occurred during logout");
            }
        }

        public async Task<ApiResponse<bool>> ChangePasswordAsync(Application.DTOs.ChangePasswordRequest request)
        {
            try
            {
                if (Guid.TryParse(request.UserId, out var userGuid))
                {
                    var user = await _context.Users.FindAsync(userGuid);
                    if (user == null)
                    {
                        return ApiResponse<bool>.ErrorResult("User not found");
                    }

                    if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    {
                        return ApiResponse<bool>.ErrorResult("Current password is incorrect");
                    }

                    user.PasswordHash = HashPassword(request.NewPassword);
                    await _context.SaveChangesAsync();

                    return ApiResponse<bool>.SuccessResult(true, "Password changed successfully");
                }

                return ApiResponse<bool>.ErrorResult("Invalid user ID");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", request.UserId);
                return ApiResponse<bool>.ErrorResult("An error occurred during password change");
            }
        }

        public async Task<ApiResponse<bool>> ForgotPasswordAsync(Application.DTOs.ForgotPasswordRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                {
                    return ApiResponse<bool>.SuccessResult(true, "If the email exists, a reset link has been sent");
                }

                var resetToken = Guid.NewGuid().ToString();
                user.PasswordResetToken = resetToken;
                user.PasswordResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Password reset email sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email: {Email}", request.Email);
                return ApiResponse<bool>.ErrorResult("An error occurred during password reset request");
            }
        }

        public async Task<ApiResponse<bool>> ResetPasswordAsync(Application.DTOs.ResetPasswordRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

                if (user == null || user.PasswordResetTokenExpiry <= DateTime.UtcNow)
                {
                    return ApiResponse<bool>.ErrorResult("Invalid or expired reset token");
                }

                user.PasswordHash = HashPassword(request.NewPassword);
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset");
                return ApiResponse<bool>.ErrorResult("An error occurred during password reset");
            }
        }

        public async Task<ApiResponse<Application.DTOs.UserDto>> GetCurrentUserAsync(string userId)
        {
            try
            {
                if (Guid.TryParse(userId, out var userGuid))
                {
                    var user = await _context.Users
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.Id == userGuid);

                    if (user == null)
                    {
                        return ApiResponse<Application.DTOs.UserDto>.ErrorResult("User not found");
                    }

                    return ApiResponse<Application.DTOs.UserDto>.SuccessResult(MapToUserDto(user));
                }

                return ApiResponse<Application.DTOs.UserDto>.ErrorResult("Invalid user ID");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user: {UserId}", userId);
                return ApiResponse<Application.DTOs.UserDto>.ErrorResult("An error occurred while retrieving user information");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryInMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"] ?? "AttendancePlatform",
                Audience = jwtSettings["Audience"] ?? "AttendancePlatform",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == hash;
        }

        private Application.DTOs.UserDto MapToUserDto(User user)
        {
            return new Application.DTOs.UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                Position = user.Position,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Status = user.IsActive ? "Active" : "Inactive",
                Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            };
        }
    }
}
