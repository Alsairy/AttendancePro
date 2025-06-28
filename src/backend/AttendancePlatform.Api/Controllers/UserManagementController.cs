using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Application.DTOs;
using System.Security.Claims;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(
            IUserManagementService userManagementService,
            ILogger<UserManagementController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<Application.DTOs.UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Application.DTOs.UserDto>> GetUserById(Guid id)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving user");
            }
        }

        [HttpGet("by-email/{email}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<Application.DTOs.UserDto>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManagementService.GetUserByEmailAsync(email);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return StatusCode(500, "An error occurred while retrieving user");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<Application.DTOs.UserDto>> CreateUser([FromBody] Application.DTOs.CreateUserDto request)
        {
            try
            {
                var user = await _userManagementService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred while creating user");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult<Application.DTOs.UserDto>> UpdateUser(Guid id, [FromBody] Application.DTOs.UpdateUserDto request)
        {
            try
            {
                var user = await _userManagementService.UpdateUserAsync(id, request);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {UserId}", id);
                return StatusCode(500, "An error occurred while updating user");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var result = await _userManagementService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {UserId}", id);
                return StatusCode(500, "An error occurred while deleting user");
            }
        }

        [HttpPost("{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRole(Guid userId, Guid roleId)
        {
            try
            {
                var result = await _userManagementService.AssignRoleAsync(userId, roleId);
                if (!result)
                {
                    return BadRequest("Failed to assign role to user");
                }
                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", roleId, userId);
                return StatusCode(500, "An error occurred while assigning role");
            }
        }

        [HttpDelete("{userId}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(Guid userId, Guid roleId)
        {
            try
            {
                var result = await _userManagementService.RemoveRoleAsync(userId, roleId);
                if (!result)
                {
                    return NotFound("User role not found");
                }
                return Ok(new { message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
                return StatusCode(500, "An error occurred while removing role");
            }
        }

        [HttpGet("by-role/{roleName}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<Application.DTOs.UserDto>>> GetUsersByRole(string roleName)
        {
            try
            {
                var users = await _userManagementService.GetUsersByRoleAsync(roleName);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by role: {RoleName}", roleName);
                return StatusCode(500, "An error occurred while retrieving users by role");
            }
        }

        [HttpGet("{managerId}/direct-reports")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<Application.DTOs.UserDto>>> GetDirectReports(Guid managerId)
        {
            try
            {
                var users = await _userManagementService.GetDirectReportsAsync(managerId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting direct reports for manager: {ManagerId}", managerId);
                return StatusCode(500, "An error occurred while retrieving direct reports");
            }
        }

        [HttpPost("{userId}/manager/{managerId}")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> SetManager(Guid userId, Guid managerId)
        {
            try
            {
                var result = await _userManagementService.SetManagerAsync(userId, managerId);
                if (!result)
                {
                    return NotFound($"User with ID {userId} not found");
                }
                return Ok(new { message = "Manager set successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting manager {ManagerId} for user {UserId}", managerId, userId);
                return StatusCode(500, "An error occurred while setting manager");
            }
        }

        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> ActivateUser(Guid id)
        {
            try
            {
                var result = await _userManagementService.ActivateUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user: {UserId}", id);
                return StatusCode(500, "An error occurred while activating user");
            }
        }

        [HttpPost("{id}/deactivate")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<ActionResult> DeactivateUser(Guid id)
        {
            try
            {
                var result = await _userManagementService.DeactivateUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user: {UserId}", id);
                return StatusCode(500, "An error occurred while deactivating user");
            }
        }

        [HttpPost("{id}/suspend")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SuspendUser(Guid id)
        {
            try
            {
                var result = await _userManagementService.SuspendUserAsync(id);
                if (!result)
                {
                    return NotFound($"User with ID {id} not found");
                }
                return Ok(new { message = "User suspended successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending user: {UserId}", id);
                return StatusCode(500, "An error occurred while suspending user");
            }
        }

        [HttpGet("my-profile")]
        public async Task<ActionResult<Application.DTOs.UserProfileDto>> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var profile = await _userManagementService.GetUserProfileAsync(userId.Value);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user profile");
            }
        }

        [HttpPut("my-profile")]
        public async Task<ActionResult<Application.DTOs.UserProfileDto>> UpdateMyProfile([FromBody] Application.DTOs.UpdateUserProfileDto request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized("User not authenticated");
            }

            try
            {
                var profile = await _userManagementService.UpdateUserProfileAsync(userId.Value, request);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", userId);
                return StatusCode(500, "An error occurred while updating user profile");
            }
        }

        [HttpGet("{id}/profile")]
        [Authorize(Roles = "Admin,Manager,HR")]
        public async Task<ActionResult<Application.DTOs.UserProfileDto>> GetUserProfile(Guid id)
        {
            try
            {
                var profile = await _userManagementService.GetUserProfileAsync(id);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile: {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving user profile");
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("sub")?.Value ??
                             User.FindFirst("userId")?.Value;
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            
            return null;
        }
    }
}
