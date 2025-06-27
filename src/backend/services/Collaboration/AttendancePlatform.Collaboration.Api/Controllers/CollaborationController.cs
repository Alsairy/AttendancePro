using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Collaboration.Api.Services;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Collaboration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollaborationController : ControllerBase
    {
        private readonly ICollaborationService _collaborationService;

        public CollaborationController(ICollaborationService collaborationService)
        {
            _collaborationService = collaborationService;
        }

        [HttpPost("teams")]
        public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto createTeam)
        {
            var result = await _collaborationService.CreateTeamAsync(createTeam);
            return Ok(result);
        }

        [HttpPut("teams/{teamId}")]
        public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] UpdateTeamDto updateTeam)
        {
            var result = await _collaborationService.UpdateTeamAsync(teamId, updateTeam);
            return Ok(result);
        }

        [HttpDelete("teams/{teamId}")]
        public async Task<IActionResult> DeleteTeam(Guid teamId)
        {
            var result = await _collaborationService.DeleteTeamAsync(teamId);
            if (result)
                return Ok(new { message = "Team deleted successfully" });
            
            return BadRequest(new { message = "Failed to delete team" });
        }

        [HttpGet("teams/{teamId}")]
        public async Task<IActionResult> GetTeam(Guid teamId)
        {
            var result = await _collaborationService.GetTeamAsync(teamId);
            return Ok(result);
        }

        [HttpGet("users/{userId}/teams")]
        public async Task<IActionResult> GetUserTeams(Guid userId)
        {
            var result = await _collaborationService.GetUserTeamsAsync(userId);
            return Ok(result);
        }

        [HttpPost("teams/{teamId}/members")]
        public async Task<IActionResult> AddTeamMember(Guid teamId, [FromBody] AddTeamMemberRequest request)
        {
            var result = await _collaborationService.AddTeamMemberAsync(teamId, request.UserId, request.Role);
            if (result)
                return Ok(new { message = "Team member added successfully" });
            
            return BadRequest(new { message = "Failed to add team member" });
        }

        [HttpDelete("teams/{teamId}/members/{userId}")]
        public async Task<IActionResult> RemoveTeamMember(Guid teamId, Guid userId)
        {
            var result = await _collaborationService.RemoveTeamMemberAsync(teamId, userId);
            if (result)
                return Ok(new { message = "Team member removed successfully" });
            
            return BadRequest(new { message = "Failed to remove team member" });
        }

        [HttpPut("teams/{teamId}/members/{userId}/role")]
        public async Task<IActionResult> UpdateTeamMemberRole(Guid teamId, Guid userId, [FromBody] UpdateRoleRequest request)
        {
            var result = await _collaborationService.UpdateTeamMemberRoleAsync(teamId, userId, request.NewRole);
            if (result)
                return Ok(new { message = "Team member role updated successfully" });
            
            return BadRequest(new { message = "Failed to update team member role" });
        }

        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessage)
        {
            var result = await _collaborationService.SendMessageAsync(sendMessage);
            return Ok(result);
        }

        [HttpGet("teams/{teamId}/messages")]
        public async Task<IActionResult> GetTeamMessages(Guid teamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _collaborationService.GetTeamMessagesAsync(teamId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("users/{userId1}/messages/{userId2}")]
        public async Task<IActionResult> GetDirectMessages(Guid userId1, Guid userId2, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _collaborationService.GetDirectMessagesAsync(userId1, userId2, page, pageSize);
            return Ok(result);
        }

        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var result = await _collaborationService.DeleteMessageAsync(messageId);
            if (result)
                return Ok(new { message = "Message deleted successfully" });
            
            return BadRequest(new { message = "Failed to delete message" });
        }

        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] EditMessageRequest request)
        {
            var result = await _collaborationService.EditMessageAsync(messageId, request.NewContent);
            return Ok(result);
        }

        [HttpPost("video-conferences")]
        public async Task<IActionResult> StartVideoConference([FromBody] StartVideoConferenceDto startConference)
        {
            var result = await _collaborationService.StartVideoConferenceAsync(startConference);
            return Ok(result);
        }

        [HttpPost("video-conferences/{conferenceId}/join")]
        public async Task<IActionResult> JoinVideoConference(Guid conferenceId, [FromBody] JoinConferenceRequest request)
        {
            var result = await _collaborationService.JoinVideoConferenceAsync(conferenceId, request.UserId);
            if (result)
                return Ok(new { message = "Joined video conference successfully" });
            
            return BadRequest(new { message = "Failed to join video conference" });
        }

        [HttpPost("video-conferences/{conferenceId}/leave")]
        public async Task<IActionResult> LeaveVideoConference(Guid conferenceId, [FromBody] LeaveConferenceRequest request)
        {
            var result = await _collaborationService.LeaveVideoConferenceAsync(conferenceId, request.UserId);
            if (result)
                return Ok(new { message = "Left video conference successfully" });
            
            return BadRequest(new { message = "Failed to leave video conference" });
        }

        [HttpPost("video-conferences/{conferenceId}/end")]
        public async Task<IActionResult> EndVideoConference(Guid conferenceId)
        {
            var result = await _collaborationService.EndVideoConferenceAsync(conferenceId);
            if (result)
                return Ok(new { message = "Video conference ended successfully" });
            
            return BadRequest(new { message = "Failed to end video conference" });
        }

        [HttpGet("video-conferences/{conferenceId}")]
        public async Task<IActionResult> GetVideoConference(Guid conferenceId)
        {
            var result = await _collaborationService.GetVideoConferenceAsync(conferenceId);
            return Ok(result);
        }

        [HttpGet("tenants/{tenantId}/video-conferences/active")]
        public async Task<IActionResult> GetActiveConferences(Guid tenantId)
        {
            var result = await _collaborationService.GetActiveConferencesAsync(tenantId);
            return Ok(result);
        }

        [HttpPost("documents")]
        public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentDto createDocument)
        {
            var result = await _collaborationService.CreateDocumentAsync(createDocument);
            return Ok(result);
        }

        [HttpPut("documents/{documentId}")]
        public async Task<IActionResult> UpdateDocument(Guid documentId, [FromBody] UpdateDocumentDto updateDocument)
        {
            var result = await _collaborationService.UpdateDocumentAsync(documentId, updateDocument);
            return Ok(result);
        }

        [HttpDelete("documents/{documentId}")]
        public async Task<IActionResult> DeleteDocument(Guid documentId)
        {
            var result = await _collaborationService.DeleteDocumentAsync(documentId);
            if (result)
                return Ok(new { message = "Document deleted successfully" });
            
            return BadRequest(new { message = "Failed to delete document" });
        }

        [HttpGet("documents/{documentId}")]
        public async Task<IActionResult> GetDocument(Guid documentId)
        {
            var result = await _collaborationService.GetDocumentAsync(documentId);
            return Ok(result);
        }

        [HttpGet("teams/{teamId}/documents")]
        public async Task<IActionResult> GetTeamDocuments(Guid teamId)
        {
            var result = await _collaborationService.GetTeamDocumentsAsync(teamId);
            return Ok(result);
        }

        [HttpPost("documents/{documentId}/versions")]
        public async Task<IActionResult> CreateDocumentVersion(Guid documentId, [FromBody] CreateDocumentVersionDto createVersion)
        {
            var result = await _collaborationService.CreateDocumentVersionAsync(documentId, createVersion);
            return Ok(result);
        }

        [HttpGet("documents/{documentId}/versions")]
        public async Task<IActionResult> GetDocumentVersions(Guid documentId)
        {
            var result = await _collaborationService.GetDocumentVersionsAsync(documentId);
            return Ok(result);
        }

        [HttpPost("documents/{documentId}/share")]
        public async Task<IActionResult> ShareDocument(Guid documentId, [FromBody] ShareDocumentRequest request)
        {
            var result = await _collaborationService.ShareDocumentAsync(documentId, request.UserId, request.Permission);
            if (result)
                return Ok(new { message = "Document shared successfully" });
            
            return BadRequest(new { message = "Failed to share document" });
        }

        [HttpDelete("documents/{documentId}/share/{userId}")]
        public async Task<IActionResult> RevokeDocumentAccess(Guid documentId, Guid userId)
        {
            var result = await _collaborationService.RevokeDocumentAccessAsync(documentId, userId);
            if (result)
                return Ok(new { message = "Document access revoked successfully" });
            
            return BadRequest(new { message = "Failed to revoke document access" });
        }

        [HttpPut("users/{userId}/presence")]
        public async Task<IActionResult> UpdatePresence(Guid userId, [FromBody] UpdatePresenceDto updatePresence)
        {
            var result = await _collaborationService.UpdatePresenceAsync(userId, updatePresence);
            return Ok(result);
        }

        [HttpGet("users/{userId}/presence")]
        public async Task<IActionResult> GetUserPresence(Guid userId)
        {
            var result = await _collaborationService.GetUserPresenceAsync(userId);
            return Ok(result);
        }

        [HttpGet("teams/{teamId}/presence")]
        public async Task<IActionResult> GetTeamPresence(Guid teamId)
        {
            var result = await _collaborationService.GetTeamPresenceAsync(teamId);
            return Ok(result);
        }

        [HttpPost("screen-share")]
        public async Task<IActionResult> StartScreenShare([FromBody] StartScreenShareDto startScreenShare)
        {
            var result = await _collaborationService.StartScreenShareAsync(startScreenShare);
            return Ok(result);
        }

        [HttpPost("screen-share/{sessionId}/join")]
        public async Task<IActionResult> JoinScreenShare(Guid sessionId, [FromBody] JoinScreenShareRequest request)
        {
            var result = await _collaborationService.JoinScreenShareAsync(sessionId, request.UserId);
            if (result)
                return Ok(new { message = "Joined screen share successfully" });
            
            return BadRequest(new { message = "Failed to join screen share" });
        }

        [HttpPost("screen-share/{sessionId}/end")]
        public async Task<IActionResult> EndScreenShare(Guid sessionId)
        {
            var result = await _collaborationService.EndScreenShareAsync(sessionId);
            if (result)
                return Ok(new { message = "Screen share ended successfully" });
            
            return BadRequest(new { message = "Failed to end screen share" });
        }

        [HttpGet("screen-share/{sessionId}")]
        public async Task<IActionResult> GetScreenShareSession(Guid sessionId)
        {
            var result = await _collaborationService.GetScreenShareSessionAsync(sessionId);
            return Ok(result);
        }

        [HttpPost("tasks")]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTask)
        {
            var result = await _collaborationService.CreateTaskAsync(createTask);
            return Ok(result);
        }

        [HttpPut("tasks/{taskId}")]
        public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto updateTask)
        {
            var result = await _collaborationService.UpdateTaskAsync(taskId, updateTask);
            return Ok(result);
        }

        [HttpDelete("tasks/{taskId}")]
        public async Task<IActionResult> DeleteTask(Guid taskId)
        {
            var result = await _collaborationService.DeleteTaskAsync(taskId);
            if (result)
                return Ok(new { message = "Task deleted successfully" });
            
            return BadRequest(new { message = "Failed to delete task" });
        }

        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTask(Guid taskId)
        {
            var result = await _collaborationService.GetTaskAsync(taskId);
            return Ok(result);
        }

        [HttpGet("teams/{teamId}/tasks")]
        public async Task<IActionResult> GetTeamTasks(Guid teamId)
        {
            var result = await _collaborationService.GetTeamTasksAsync(teamId);
            return Ok(result);
        }

        [HttpGet("users/{userId}/tasks")]
        public async Task<IActionResult> GetUserTasks(Guid userId)
        {
            var result = await _collaborationService.GetUserTasksAsync(userId);
            return Ok(result);
        }

        [HttpPost("tasks/{taskId}/assign")]
        public async Task<IActionResult> AssignTask(Guid taskId, [FromBody] AssignTaskRequest request)
        {
            var result = await _collaborationService.AssignTaskAsync(taskId, request.UserId);
            if (result)
                return Ok(new { message = "Task assigned successfully" });
            
            return BadRequest(new { message = "Failed to assign task" });
        }

        [HttpPost("tasks/{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(Guid taskId)
        {
            var result = await _collaborationService.CompleteTaskAsync(taskId);
            if (result)
                return Ok(new { message = "Task completed successfully" });
            
            return BadRequest(new { message = "Failed to complete task" });
        }

        [HttpGet("tenants/{tenantId}/analytics")]
        public async Task<IActionResult> GetCollaborationAnalytics(Guid tenantId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _collaborationService.GetCollaborationAnalyticsAsync(tenantId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("teams/{teamId}/activity")]
        public async Task<IActionResult> GetTeamActivity(Guid teamId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var result = await _collaborationService.GetTeamActivityAsync(teamId, startDate, endDate);
            return Ok(result);
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "collaboration" });
        }
    }

    public class AddTeamMemberRequest
    {
        public Guid UserId { get; set; }
        public TeamRole Role { get; set; }
    }

    public class UpdateRoleRequest
    {
        public TeamRole NewRole { get; set; }
    }

    public class EditMessageRequest
    {
        public string NewContent { get; set; }
    }

    public class JoinConferenceRequest
    {
        public Guid UserId { get; set; }
    }

    public class LeaveConferenceRequest
    {
        public Guid UserId { get; set; }
    }

    public class ShareDocumentRequest
    {
        public Guid UserId { get; set; }
        public DocumentPermission Permission { get; set; }
    }

    public class JoinScreenShareRequest
    {
        public Guid UserId { get; set; }
    }

    public class AssignTaskRequest
    {
        public Guid UserId { get; set; }
    }
}
