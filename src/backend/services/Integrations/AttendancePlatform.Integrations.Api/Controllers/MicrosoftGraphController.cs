using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Integrations.Api.Services;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MicrosoftGraphController : ControllerBase
    {
        private readonly IMicrosoftGraphService _microsoftGraphService;

        public MicrosoftGraphController(IMicrosoftGraphService microsoftGraphService)
        {
            _microsoftGraphService = microsoftGraphService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _microsoftGraphService.GetUsersAsync();
            return Ok(users);
        }

        [HttpGet("users/{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var user = await _microsoftGraphService.GetUserAsync(userId);
            if (user == null)
                return NotFound();
            
            return Ok(user);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            var createdUser = await _microsoftGraphService.CreateUserAsync(user);
            if (createdUser == null)
                return BadRequest(new { message = "Failed to create user" });
            
            return CreatedAtAction(nameof(GetUser), new { userId = createdUser.Id }, createdUser);
        }

        [HttpPut("users/{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, [FromBody] User user)
        {
            var updatedUser = await _microsoftGraphService.UpdateUserAsync(userId, user);
            if (updatedUser == null)
                return NotFound();
            
            return Ok(updatedUser);
        }

        [HttpDelete("users/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var result = await _microsoftGraphService.DeleteUserAsync(userId);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _microsoftGraphService.GetGroupsAsync();
            return Ok(groups);
        }

        [HttpGet("groups/{groupId}")]
        public async Task<IActionResult> GetGroup(string groupId)
        {
            var group = await _microsoftGraphService.GetGroupAsync(groupId);
            if (group == null)
                return NotFound();
            
            return Ok(group);
        }

        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] Group group)
        {
            var createdGroup = await _microsoftGraphService.CreateGroupAsync(group);
            if (createdGroup == null)
                return BadRequest(new { message = "Failed to create group" });
            
            return CreatedAtAction(nameof(GetGroup), new { groupId = createdGroup.Id }, createdGroup);
        }

        [HttpPut("groups/{groupId}")]
        public async Task<IActionResult> UpdateGroup(string groupId, [FromBody] Group group)
        {
            var updatedGroup = await _microsoftGraphService.UpdateGroupAsync(groupId, group);
            if (updatedGroup == null)
                return NotFound();
            
            return Ok(updatedGroup);
        }

        [HttpDelete("groups/{groupId}")]
        public async Task<IActionResult> DeleteGroup(string groupId)
        {
            var result = await _microsoftGraphService.DeleteGroupAsync(groupId);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpPost("groups/{groupId}/members/{userId}")]
        public async Task<IActionResult> AddUserToGroup(string groupId, string userId)
        {
            var result = await _microsoftGraphService.AddUserToGroupAsync(groupId, userId);
            if (!result)
                return BadRequest(new { message = "Failed to add user to group" });
            
            return Ok(new { message = "User added to group successfully" });
        }

        [HttpDelete("groups/{groupId}/members/{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(string groupId, string userId)
        {
            var result = await _microsoftGraphService.RemoveUserFromGroupAsync(groupId, userId);
            if (!result)
                return BadRequest(new { message = "Failed to remove user from group" });
            
            return Ok(new { message = "User removed from group successfully" });
        }

        [HttpGet("groups/{groupId}/members")]
        public async Task<IActionResult> GetGroupMembers(string groupId)
        {
            var members = await _microsoftGraphService.GetGroupMembersAsync(groupId);
            return Ok(members);
        }

        [HttpGet("users/{userId}/calendar/events")]
        public async Task<IActionResult> GetCalendarEvents(string userId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            var events = await _microsoftGraphService.GetCalendarEventsAsync(userId, startTime, endTime);
            return Ok(events);
        }

        [HttpPost("users/{userId}/calendar/events")]
        public async Task<IActionResult> CreateCalendarEvent(string userId, [FromBody] Event calendarEvent)
        {
            var createdEvent = await _microsoftGraphService.CreateCalendarEventAsync(userId, calendarEvent);
            if (createdEvent == null)
                return BadRequest(new { message = "Failed to create calendar event" });
            
            return Ok(createdEvent);
        }

        [HttpPut("users/{userId}/calendar/events/{eventId}")]
        public async Task<IActionResult> UpdateCalendarEvent(string userId, string eventId, [FromBody] Event calendarEvent)
        {
            var updatedEvent = await _microsoftGraphService.UpdateCalendarEventAsync(userId, eventId, calendarEvent);
            if (updatedEvent == null)
                return NotFound();
            
            return Ok(updatedEvent);
        }

        [HttpDelete("users/{userId}/calendar/events/{eventId}")]
        public async Task<IActionResult> DeleteCalendarEvent(string userId, string eventId)
        {
            var result = await _microsoftGraphService.DeleteCalendarEventAsync(userId, eventId);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpGet("directory-objects")]
        public async Task<IActionResult> GetDirectoryObjects()
        {
            var directoryObjects = await _microsoftGraphService.GetDirectoryObjectsAsync();
            return Ok(directoryObjects);
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            var isConnected = await _microsoftGraphService.TestConnectionAsync();
            return Ok(new { isConnected });
        }
    }
}
