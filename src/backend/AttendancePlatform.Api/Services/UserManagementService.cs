using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Application.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Application.Services;

namespace AttendancePlatform.Api.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<UserManagementService> _logger;
        private readonly ITenantContext _tenantContext;

        public UserManagementService(
            AttendancePlatformDbContext context,
            ILogger<UserManagementService> logger,
            ITenantContext tenantContext)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public async Task<IEnumerable<Application.DTOs.UserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .ToListAsync();

                return users.Select(MapToUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                throw;
            }
        }

        public async Task<Application.DTOs.UserDto> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                return MapToUserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                throw;
            }
        }

        public async Task<Application.DTOs.UserDto> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with email {email} not found");
                }

                return MapToUserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                throw;
            }
        }

        public async Task<Application.DTOs.UserDto> CreateUserAsync(Application.DTOs.CreateUserDto request)
        {
            try
            {
                var user = new User
                {
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    EmployeeId = request.EmployeeId,
                    PhoneNumber = request.PhoneNumber,
                    Department = request.Department,
                    Position = request.Position,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return MapToUserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                throw;
            }
        }

        public async Task<Application.DTOs.UserDto> UpdateUserAsync(Guid id, Application.DTOs.UpdateUserDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                user.Department = request.Department;
                user.Position = request.Position;

                await _context.SaveChangesAsync();

                return MapToUserDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                return false;
            }
        }

        public async Task<bool> AssignRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                var existingUserRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (existingUserRole != null)
                {
                    return true;
                }

                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.UserRoles.Add(userRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return false;
            }
        }

        public async Task<bool> RemoveRoleAsync(Guid userId, Guid roleId)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                {
                    return false;
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return false;
            }
        }

        public async Task<IEnumerable<Application.DTOs.UserDto>> GetUsersByRoleAsync(string roleName)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Where(u => u.UserRoles.Any(ur => ur.Role.Name == roleName))
                    .ToListAsync();

                return users.Select(MapToUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {RoleName}", roleName);
                throw;
            }
        }

        public async Task<IEnumerable<Application.DTOs.UserDto>> GetDirectReportsAsync(Guid managerId)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Where(u => u.ManagerId == managerId)
                    .ToListAsync();

                return users.Select(MapToUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting direct reports for manager: {ManagerId}", managerId);
                throw;
            }
        }

        public async Task<bool> SetManagerAsync(Guid userId, Guid managerId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return false;
                }

                user.ManagerId = managerId;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting manager {ManagerId} for user {UserId}", managerId, userId);
                return false;
            }
        }

        public async Task<bool> ActivateUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", id);
                return false;
            }
        }

        public async Task<bool> DeactivateUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsActive = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", id);
                return false;
            }
        }

        public async Task<bool> SuspendUserAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return false;
                }

                user.IsSuspended = true;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending user: {UserId}", id);
                return false;
            }
        }

        public async Task<Application.DTOs.UserProfileDto> GetUserProfileAsync(Guid id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                return new Application.DTOs.UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeId = user.EmployeeId,
                    PhoneNumber = user.PhoneNumber,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {UserId}", id);
                throw;
            }
        }

        public async Task<Application.DTOs.UserProfileDto> UpdateUserProfileAsync(Guid id, Application.DTOs.UpdateUserProfileDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {id} not found");
                }

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                user.ProfilePictureUrl = request.ProfilePictureUrl;

                await _context.SaveChangesAsync();

                return new Application.DTOs.UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeId = user.EmployeeId,
                    PhoneNumber = user.PhoneNumber,
                    Department = user.Department,
                    Position = user.Position,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    LastLoginDate = user.LastLoginDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", id);
                throw;
            }
        }

        private Application.DTOs.UserDto MapToUserDto(User user)
        {
            return new Application.DTOs.UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                PhoneNumber = user.PhoneNumber,
                Department = user.Department,
                Position = user.Position,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedDate ?? DateTime.UtcNow,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate,
                Roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>()
            };
        }
    }
}
