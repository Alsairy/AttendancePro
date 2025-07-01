using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CollaborationController : ControllerBase
    {
        private readonly ICollaborationService _collaborationService;
        private readonly ILogger<CollaborationController> _logger;

        public CollaborationController(
            ICollaborationService collaborationService,
            ILogger<CollaborationController> logger)
        {
            _collaborationService = collaborationService;
            _logger = logger;
        }

        [HttpPost("teams")]
        public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] TeamDto team)
        {
            try
            {
                var createdTeam = await _collaborationService.CreateTeamAsync(team);
                return Ok(createdTeam);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating team");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("teams")]
        public async Task<ActionResult<List<TeamDto>>> GetTeams([FromQuery] Guid tenantId)
        {
            try
            {
                var teams = await _collaborationService.GetTeamsAsync(tenantId);
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting teams");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("teams/{teamId}")]
        public async Task<ActionResult<TeamDto>> GetTeam(Guid teamId)
        {
            try
            {
                var team = await _collaborationService.GetTeamAsync(teamId);
                if (team == null)
                    return NotFound();

                return Ok(team);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting team");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("teams/{teamId}/members")]
        public async Task<ActionResult> AddTeamMember(Guid teamId, [FromQuery] Guid userId)
        {
            try
            {
                var result = await _collaborationService.AddTeamMemberAsync(teamId, userId);
                if (!result)
                    return BadRequest("Failed to add team member");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding team member");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("teams/{teamId}/members/{userId}")]
        public async Task<ActionResult> RemoveTeamMember(Guid teamId, Guid userId)
        {
            try
            {
                var result = await _collaborationService.RemoveTeamMemberAsync(teamId, userId);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing team member");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("projects")]
        public async Task<ActionResult<ProjectDto>> CreateProject([FromBody] ProjectDto project)
        {
            try
            {
                var createdProject = await _collaborationService.CreateProjectAsync(project);
                return Ok(createdProject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("projects")]
        public async Task<ActionResult<List<ProjectDto>>> GetProjects([FromQuery] Guid tenantId)
        {
            try
            {
                var projects = await _collaborationService.GetProjectsAsync(tenantId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("tasks")]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] TaskDto task)
        {
            try
            {
                var createdTask = await _collaborationService.CreateTaskAsync(task);
                return Ok(createdTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("projects/{projectId}/tasks")]
        public async Task<ActionResult<List<TaskDto>>> GetTasks(Guid projectId)
        {
            try
            {
                var tasks = await _collaborationService.GetTasksAsync(projectId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tasks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("tasks/{taskId}/status")]
        public async Task<ActionResult> UpdateTaskStatus(Guid taskId, [FromBody] string status)
        {
            try
            {
                var result = await _collaborationService.UpdateTaskStatusAsync(taskId, status);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("documents")]
        public async Task<ActionResult<DocumentDto>> CreateDocument([FromBody] DocumentDto document)
        {
            try
            {
                var createdDocument = await _collaborationService.CreateDocumentAsync(document);
                return Ok(createdDocument);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("projects/{projectId}/documents")]
        public async Task<ActionResult<List<DocumentDto>>> GetDocuments(Guid projectId)
        {
            try
            {
                var documents = await _collaborationService.GetDocumentsAsync(projectId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting documents");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("documents/{documentId}/share")]
        public async Task<ActionResult> ShareDocument(Guid documentId, [FromBody] List<Guid> userIds)
        {
            try
            {
                var result = await _collaborationService.ShareDocumentAsync(documentId, userIds);
                if (!result)
                    return BadRequest("Failed to share document");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing document");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("messages")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage([FromBody] ChatMessageDto message)
        {
            try
            {
                var sentMessage = await _collaborationService.SendMessageAsync(message);
                return Ok(sentMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("teams/{teamId}/messages")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetMessages(Guid teamId)
        {
            try
            {
                var messages = await _collaborationService.GetMessagesAsync(teamId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
