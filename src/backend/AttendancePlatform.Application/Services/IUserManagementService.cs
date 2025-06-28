using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IUserManagementService
{
    Task<IEnumerable<Application.DTOs.UserDto>> GetAllUsersAsync();
    Task<Application.DTOs.UserDto> GetUserByIdAsync(Guid id);
    Task<Application.DTOs.UserDto> GetUserByEmailAsync(string email);
    Task<Application.DTOs.UserDto> CreateUserAsync(Application.DTOs.CreateUserDto request);
    Task<Application.DTOs.UserDto> UpdateUserAsync(Guid id, Application.DTOs.UpdateUserDto request);
    Task<bool> DeleteUserAsync(Guid id);
    Task<bool> AssignRoleAsync(Guid userId, Guid roleId);
    Task<bool> RemoveRoleAsync(Guid userId, Guid roleId);
    Task<IEnumerable<Application.DTOs.UserDto>> GetUsersByRoleAsync(string roleName);
    Task<IEnumerable<Application.DTOs.UserDto>> GetDirectReportsAsync(Guid managerId);
    Task<bool> SetManagerAsync(Guid userId, Guid managerId);
    Task<bool> ActivateUserAsync(Guid id);
    Task<bool> DeactivateUserAsync(Guid id);
    Task<bool> SuspendUserAsync(Guid id);
    Task<Application.DTOs.UserProfileDto> GetUserProfileAsync(Guid id);
    Task<Application.DTOs.UserProfileDto> UpdateUserProfileAsync(Guid id, Application.DTOs.UpdateUserProfileDto request);
}
