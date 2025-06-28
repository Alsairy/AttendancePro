using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Application.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ActiveDirectoryController : ControllerBase
    {
        private readonly IActiveDirectoryService _activeDirectoryService;
        private readonly ILogger<ActiveDirectoryController> _logger;

        public ActiveDirectoryController(
            IActiveDirectoryService activeDirectoryService,
            ILogger<ActiveDirectoryController> logger)
        {
            _activeDirectoryService = activeDirectoryService;
            _logger = logger;
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<AdUserDto>>> GetUsers([FromQuery] string? searchFilter = null)
        {
            try
            {
                var users = await _activeDirectoryService.GetUsersAsync(searchFilter);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AD users");
                return StatusCode(500, "Error retrieving users from Active Directory");
            }
        }

        [HttpGet("users/{userPrincipalName}")]
        public async Task<ActionResult<AdUserDto>> GetUser(string userPrincipalName)
        {
            try
            {
                var user = await _activeDirectoryService.GetUserAsync(userPrincipalName);
                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error retrieving user from Active Directory");
            }
        }

        [HttpPost("users")]
        public async Task<ActionResult<AdUserDto>> CreateUser([FromBody] AdUserDto userDto)
        {
            try
            {
                var createdUser = await _activeDirectoryService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { userPrincipalName = createdUser.UserPrincipalName }, createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating AD user: {UserPrincipalName}", userDto.UserPrincipalName);
                return StatusCode(500, "Error creating user in Active Directory");
            }
        }

        [HttpPut("users/{userPrincipalName}")]
        public async Task<ActionResult<AdUserDto>> UpdateUser(string userPrincipalName, [FromBody] AdUserDto userDto)
        {
            try
            {
                var updatedUser = await _activeDirectoryService.UpdateUserAsync(userPrincipalName, userDto);
                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error updating user in Active Directory");
            }
        }

        [HttpDelete("users/{userPrincipalName}")]
        public async Task<ActionResult> DeleteUser(string userPrincipalName)
        {
            try
            {
                var result = await _activeDirectoryService.DeleteUserAsync(userPrincipalName);
                if (!result)
                {
                    return NotFound($"User {userPrincipalName} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error deleting user from Active Directory");
            }
        }

        [HttpPost("users/{userPrincipalName}/enable")]
        public async Task<ActionResult> EnableUser(string userPrincipalName)
        {
            try
            {
                var result = await _activeDirectoryService.EnableUserAsync(userPrincipalName);
                if (!result)
                {
                    return NotFound($"User {userPrincipalName} not found");
                }
                return Ok(new { message = "User enabled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error enabling user in Active Directory");
            }
        }

        [HttpPost("users/{userPrincipalName}/disable")]
        public async Task<ActionResult> DisableUser(string userPrincipalName)
        {
            try
            {
                var result = await _activeDirectoryService.DisableUserAsync(userPrincipalName);
                if (!result)
                {
                    return NotFound($"User {userPrincipalName} not found");
                }
                return Ok(new { message = "User disabled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error disabling user in Active Directory");
            }
        }

        [HttpPost("users/{userPrincipalName}/reset-password")]
        public async Task<ActionResult> ResetPassword(string userPrincipalName, [FromBody] ResetPasswordRequest request)
        {
            try
            {
                var result = await _activeDirectoryService.ResetPasswordAsync(userPrincipalName, request.NewPassword);
                if (!result)
                {
                    return NotFound($"User {userPrincipalName} not found");
                }
                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error resetting password in Active Directory");
            }
        }

        [HttpGet("groups")]
        public async Task<ActionResult<IEnumerable<AdGroupDto>>> GetGroups([FromQuery] string? searchFilter = null)
        {
            try
            {
                var groups = await _activeDirectoryService.GetGroupsAsync(searchFilter);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AD groups");
                return StatusCode(500, "Error retrieving groups from Active Directory");
            }
        }

        [HttpGet("groups/{groupName}")]
        public async Task<ActionResult<AdGroupDto>> GetGroup(string groupName)
        {
            try
            {
                var group = await _activeDirectoryService.GetGroupAsync(groupName);
                return Ok(group);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting AD group: {GroupName}", groupName);
                return StatusCode(500, "Error retrieving group from Active Directory");
            }
        }

        [HttpPost("groups")]
        public async Task<ActionResult<AdGroupDto>> CreateGroup([FromBody] AdGroupDto groupDto)
        {
            try
            {
                var createdGroup = await _activeDirectoryService.CreateGroupAsync(groupDto);
                return CreatedAtAction(nameof(GetGroup), new { groupName = createdGroup.Name }, createdGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating AD group: {GroupName}", groupDto.Name);
                return StatusCode(500, "Error creating group in Active Directory");
            }
        }

        [HttpPut("groups/{groupName}")]
        public async Task<ActionResult<AdGroupDto>> UpdateGroup(string groupName, [FromBody] AdGroupDto groupDto)
        {
            try
            {
                var updatedGroup = await _activeDirectoryService.UpdateGroupAsync(groupName, groupDto);
                return Ok(updatedGroup);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AD group: {GroupName}", groupName);
                return StatusCode(500, "Error updating group in Active Directory");
            }
        }

        [HttpDelete("groups/{groupName}")]
        public async Task<ActionResult> DeleteGroup(string groupName)
        {
            try
            {
                var result = await _activeDirectoryService.DeleteGroupAsync(groupName);
                if (!result)
                {
                    return NotFound($"Group {groupName} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting AD group: {GroupName}", groupName);
                return StatusCode(500, "Error deleting group from Active Directory");
            }
        }

        [HttpPost("groups/{groupName}/members/{userPrincipalName}")]
        public async Task<ActionResult> AddUserToGroup(string groupName, string userPrincipalName)
        {
            try
            {
                var result = await _activeDirectoryService.AddUserToGroupAsync(groupName, userPrincipalName);
                if (!result)
                {
                    return NotFound("Group or user not found");
                }
                return Ok(new { message = "User added to group successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {UserPrincipalName} to group {GroupName}", userPrincipalName, groupName);
                return StatusCode(500, "Error adding user to group in Active Directory");
            }
        }

        [HttpDelete("groups/{groupName}/members/{userPrincipalName}")]
        public async Task<ActionResult> RemoveUserFromGroup(string groupName, string userPrincipalName)
        {
            try
            {
                var result = await _activeDirectoryService.RemoveUserFromGroupAsync(groupName, userPrincipalName);
                if (!result)
                {
                    return NotFound("Group or user not found");
                }
                return Ok(new { message = "User removed from group successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserPrincipalName} from group {GroupName}", userPrincipalName, groupName);
                return StatusCode(500, "Error removing user from group in Active Directory");
            }
        }

        [HttpGet("groups/{groupName}/members")]
        public async Task<ActionResult<IEnumerable<AdUserDto>>> GetGroupMembers(string groupName)
        {
            try
            {
                var members = await _activeDirectoryService.GetGroupMembersAsync(groupName);
                return Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting members of AD group: {GroupName}", groupName);
                return StatusCode(500, "Error retrieving group members from Active Directory");
            }
        }

        [HttpGet("users/{userPrincipalName}/groups")]
        public async Task<ActionResult<IEnumerable<AdGroupDto>>> GetUserGroups(string userPrincipalName)
        {
            try
            {
                var groups = await _activeDirectoryService.GetUserGroupsAsync(userPrincipalName);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups for AD user: {UserPrincipalName}", userPrincipalName);
                return StatusCode(500, "Error retrieving user groups from Active Directory");
            }
        }

        [HttpGet("organizational-units")]
        public async Task<ActionResult<IEnumerable<AdOrganizationalUnitDto>>> GetOrganizationalUnits()
        {
            try
            {
                var ous = await _activeDirectoryService.GetOrganizationalUnitsAsync();
                return Ok(ous);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organizational units from AD");
                return StatusCode(500, "Error retrieving organizational units from Active Directory");
            }
        }

        [HttpPost("users/{userPrincipalName}/move-to-ou")]
        public async Task<ActionResult> MoveUserToOU(string userPrincipalName, [FromBody] MoveToOURequest request)
        {
            try
            {
                var result = await _activeDirectoryService.MoveUserToOUAsync(userPrincipalName, request.OUPath);
                if (!result)
                {
                    return NotFound($"User {userPrincipalName} not found");
                }
                return Ok(new { message = "User moved to OU successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving user {UserPrincipalName} to OU {OUPath}", userPrincipalName, request.OUPath);
                return StatusCode(500, "Error moving user to OU in Active Directory");
            }
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            try
            {
                var result = await _activeDirectoryService.AuthenticateUserAsync(request.UserPrincipalName, request.Password);
                return Ok(new { isAuthenticated = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user: {UserPrincipalName}", request.UserPrincipalName);
                return StatusCode(500, "Error authenticating user with Active Directory");
            }
        }

        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                var result = await _activeDirectoryService.TestConnectionAsync();
                return Ok(new { isConnected = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing AD connection");
                return StatusCode(500, "Error testing Active Directory connection");
            }
        }

        [HttpGet("domain-info")]
        public async Task<ActionResult<AdDomainInfoDto>> GetDomainInfo()
        {
            try
            {
                var domainInfo = await _activeDirectoryService.GetDomainInfoAsync();
                return Ok(domainInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting domain information");
                return StatusCode(500, "Error retrieving domain information from Active Directory");
            }
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public ActionResult<object> Health()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class MoveToOURequest
    {
        public string OUPath { get; set; } = string.Empty;
    }

    public class AuthenticateUserRequest
    {
        public string UserPrincipalName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
