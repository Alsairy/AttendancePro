using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Integrations.Api.Services;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ActiveDirectoryController : ControllerBase
    {
        private readonly IActiveDirectoryService _activeDirectoryService;

        public ActiveDirectoryController(IActiveDirectoryService activeDirectoryService)
        {
            _activeDirectoryService = activeDirectoryService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string searchFilter = null)
        {
            var users = await _activeDirectoryService.GetUsersAsync(searchFilter);
            return Ok(users);
        }

        [HttpGet("users/{userPrincipalName}")]
        public async Task<IActionResult> GetUser(string userPrincipalName)
        {
            var user = await _activeDirectoryService.GetUserAsync(userPrincipalName);
            if (user == null)
                return NotFound();
            
            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] AdUserDto user)
        {
            var createdUser = await _activeDirectoryService.CreateUserAsync(user);
            if (createdUser == null)
                return BadRequest(new { message = "Failed to create user" });
            
            return CreatedAtAction(nameof(GetUser), new { userPrincipalName = createdUser.UserPrincipalName }, createdUser);
        }

        [HttpPut("users/{userPrincipalName}")]
        public async Task<IActionResult> UpdateUser(string userPrincipalName, [FromBody] AdUserDto user)
        {
            var updatedUser = await _activeDirectoryService.UpdateUserAsync(userPrincipalName, user);
            if (updatedUser == null)
                return NotFound();
            
            return Ok(updatedUser);
        }

        [HttpDelete("users/{userPrincipalName}")]
        public async Task<IActionResult> DeleteUser(string userPrincipalName)
        {
            var result = await _activeDirectoryService.DeleteUserAsync(userPrincipalName);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpPost("users/{userPrincipalName}/enable")]
        public async Task<IActionResult> EnableUser(string userPrincipalName)
        {
            var result = await _activeDirectoryService.EnableUserAsync(userPrincipalName);
            if (!result)
                return NotFound();
            
            return Ok(new { message = "User enabled successfully" });
        }

        [HttpPost("users/{userPrincipalName}/disable")]
        public async Task<IActionResult> DisableUser(string userPrincipalName)
        {
            var result = await _activeDirectoryService.DisableUserAsync(userPrincipalName);
            if (!result)
                return NotFound();
            
            return Ok(new { message = "User disabled successfully" });
        }

        [HttpPost("users/{userPrincipalName}/reset-password")]
        public async Task<IActionResult> ResetPassword(string userPrincipalName, [FromBody] ResetPasswordRequest request)
        {
            var result = await _activeDirectoryService.ResetPasswordAsync(userPrincipalName, request.NewPassword);
            if (!result)
                return BadRequest(new { message = "Failed to reset password" });
            
            return Ok(new { message = "Password reset successfully" });
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups([FromQuery] string searchFilter = null)
        {
            var groups = await _activeDirectoryService.GetGroupsAsync(searchFilter);
            return Ok(groups);
        }

        [HttpGet("groups/{groupName}")]
        public async Task<IActionResult> GetGroup(string groupName)
        {
            var group = await _activeDirectoryService.GetGroupAsync(groupName);
            if (group == null)
                return NotFound();
            
            return Ok(group);
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] AdGroupDto group)
        {
            var createdGroup = await _activeDirectoryService.CreateGroupAsync(group);
            if (createdGroup == null)
                return BadRequest(new { message = "Failed to create group" });
            
            return CreatedAtAction(nameof(GetGroup), new { groupName = createdGroup.Name }, createdGroup);
        }

        [HttpPut("groups/{groupName}")]
        public async Task<IActionResult> UpdateGroup(string groupName, [FromBody] AdGroupDto group)
        {
            var updatedGroup = await _activeDirectoryService.UpdateGroupAsync(groupName, group);
            if (updatedGroup == null)
                return NotFound();
            
            return Ok(updatedGroup);
        }

        [HttpDelete("groups/{groupName}")]
        public async Task<IActionResult> DeleteGroup(string groupName)
        {
            var result = await _activeDirectoryService.DeleteGroupAsync(groupName);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpPost("groups/{groupName}/members/{userPrincipalName}")]
        public async Task<IActionResult> AddUserToGroup(string groupName, string userPrincipalName)
        {
            var result = await _activeDirectoryService.AddUserToGroupAsync(groupName, userPrincipalName);
            if (!result)
                return BadRequest(new { message = "Failed to add user to group" });
            
            return Ok(new { message = "User added to group successfully" });
        }

        [HttpDelete("groups/{groupName}/members/{userPrincipalName}")]
        public async Task<IActionResult> RemoveUserFromGroup(string groupName, string userPrincipalName)
        {
            var result = await _activeDirectoryService.RemoveUserFromGroupAsync(groupName, userPrincipalName);
            if (!result)
                return BadRequest(new { message = "Failed to remove user from group" });
            
            return Ok(new { message = "User removed from group successfully" });
        }

        [HttpGet("groups/{groupName}/members")]
        public async Task<IActionResult> GetGroupMembers(string groupName)
        {
            var members = await _activeDirectoryService.GetGroupMembersAsync(groupName);
            return Ok(members);
        }

        [HttpGet("users/{userPrincipalName}/groups")]
        public async Task<IActionResult> GetUserGroups(string userPrincipalName)
        {
            var groups = await _activeDirectoryService.GetUserGroupsAsync(userPrincipalName);
            return Ok(groups);
        }

        [HttpGet("organizational-units")]
        public async Task<IActionResult> GetOrganizationalUnits()
        {
            var orgUnits = await _activeDirectoryService.GetOrganizationalUnitsAsync();
            return Ok(orgUnits);
        }

        [HttpPost("users/{userPrincipalName}/move-to-ou")]
        public async Task<IActionResult> MoveUserToOU(string userPrincipalName, [FromBody] MoveToOURequest request)
        {
            var result = await _activeDirectoryService.MoveUserToOUAsync(userPrincipalName, request.OUPath);
            if (!result)
                return BadRequest(new { message = "Failed to move user to OU" });
            
            return Ok(new { message = "User moved to OU successfully" });
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateUser([FromBody] AuthenticateUserRequest request)
        {
            var result = await _activeDirectoryService.AuthenticateUserAsync(request.UserPrincipalName, request.Password);
            return Ok(new { isAuthenticated = result });
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var isConnected = await _activeDirectoryService.TestConnectionAsync();
            return Ok(new { isConnected });
        }

        [HttpGet("domain-info")]
        public async Task<IActionResult> GetDomainInfo()
        {
            var domainInfo = await _activeDirectoryService.GetDomainInfoAsync();
            return Ok(domainInfo);
        }
    }

    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }
    }

    public class MoveToOURequest
    {
        public string OUPath { get; set; }
    }

    public class AuthenticateUserRequest
    {
        public string UserPrincipalName { get; set; }
        public string Password { get; set; }
    }
}
