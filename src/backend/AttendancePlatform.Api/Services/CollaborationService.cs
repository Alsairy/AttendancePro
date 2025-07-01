using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface ICollaborationService
    {
        Task<TeamDto> CreateTeamAsync(TeamDto team);
        Task<List<TeamDto>> GetTeamsAsync(Guid tenantId);
        Task<TeamDto> GetTeamAsync(Guid teamId);
        Task<bool> AddTeamMemberAsync(Guid teamId, Guid userId);
        Task<bool> RemoveTeamMemberAsync(Guid teamId, Guid userId);
        Task<ProjectDto> CreateProjectAsync(ProjectDto project);
        Task<List<ProjectDto>> GetProjectsAsync(Guid tenantId);
        Task<TaskDto> CreateTaskAsync(TaskDto task);
        Task<List<TaskDto>> GetTasksAsync(Guid projectId);
        Task<bool> UpdateTaskStatusAsync(Guid taskId, string status);
        Task<DocumentDto> CreateDocumentAsync(DocumentDto document);
        Task<List<DocumentDto>> GetDocumentsAsync(Guid projectId);
        Task<bool> ShareDocumentAsync(Guid documentId, List<Guid> userIds);
        Task<ChatMessageDto> SendMessageAsync(ChatMessageDto message);
        Task<List<ChatMessageDto>> GetMessagesAsync(Guid teamId);
    }

    public class CollaborationService : ICollaborationService
    {
        private readonly ILogger<CollaborationService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public CollaborationService(ILogger<CollaborationService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<TeamDto> CreateTeamAsync(TeamDto team)
        {
            try
            {
                var teamEntity = new Team
                {
                    Id = Guid.NewGuid(),
                    Name = team.Name,
                    Description = team.Description,
                    TenantId = team.TenantId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Teams.Add(teamEntity);
                await _context.SaveChangesAsync();

                team.Id = teamEntity.Id;
                team.CreatedAt = teamEntity.CreatedAt;

                _logger.LogInformation("Team created: {TeamId}", teamEntity.Id);
                return team;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create team");
                throw;
            }
        }

        public async Task<List<TeamDto>> GetTeamsAsync(Guid tenantId)
        {
            try
            {
                var teams = await _context.Teams
                    .Where(t => t.TenantId == tenantId && t.IsActive)
                    .Select(t => new TeamDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        TenantId = t.TenantId,
                        CreatedAt = t.CreatedAt,
                        MemberCount = t.TeamMembers.Count
                    })
                    .ToListAsync();

                return teams;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get teams for tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<TeamDto> GetTeamAsync(Guid teamId)
        {
            try
            {
                var team = await _context.Teams
                    .Include(t => t.TeamMembers)
                    .ThenInclude(tm => tm.User)
                    .FirstOrDefaultAsync(t => t.Id == teamId);

                if (team == null) return null;

                return new TeamDto
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    TenantId = team.TenantId,
                    CreatedAt = team.CreatedAt,
                    MemberCount = team.TeamMembers.Count,
                    Members = team.TeamMembers.Select(tm => new TeamMemberDto
                    {
                        UserId = tm.UserId,
                        UserName = tm.User.FirstName + " " + tm.User.LastName,
                        Role = tm.Role,
                        JoinedAt = tm.JoinedAt
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get team: {TeamId}", teamId);
                throw;
            }
        }

        public async Task<bool> AddTeamMemberAsync(Guid teamId, Guid userId)
        {
            try
            {
                var existingMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

                if (existingMember != null) return false;

                var teamMember = new TeamMember
                {
                    Id = Guid.NewGuid(),
                    TeamId = teamId,
                    UserId = userId,
                    Role = "Member",
                    JoinedAt = DateTime.UtcNow
                };

                _context.TeamMembers.Add(teamMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} added to team {TeamId}", userId, teamId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add team member: {UserId} to {TeamId}", userId, teamId);
                throw;
            }
        }

        public async Task<bool> RemoveTeamMemberAsync(Guid teamId, Guid userId)
        {
            try
            {
                var teamMember = await _context.TeamMembers
                    .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);

                if (teamMember == null) return false;

                _context.TeamMembers.Remove(teamMember);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from team {TeamId}", userId, teamId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove team member: {UserId} from {TeamId}", userId, teamId);
                throw;
            }
        }

        public async Task<ProjectDto> CreateProjectAsync(ProjectDto project)
        {
            try
            {
                var projectEntity = new AttendancePlatform.Shared.Domain.Entities.TeamProject
                {
                    Id = Guid.NewGuid(),
                    Name = project.Name,
                    Description = project.Description,
                    TeamId = project.TeamId ?? Guid.Empty,
                    CreatedById = Guid.NewGuid(),
                    Status = "Active",
                    Priority = "Medium",
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TeamProjects.Add(projectEntity);
                await _context.SaveChangesAsync();

                project.Id = projectEntity.Id;
                project.CreatedAt = projectEntity.CreatedAt;

                _logger.LogInformation("Project created: {ProjectId}", projectEntity.Id);
                return project;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create project");
                throw;
            }
        }

        public async Task<List<ProjectDto>> GetProjectsAsync(Guid tenantId)
        {
            try
            {
                var projects = await _context.TeamProjects
                    .Where(p => p.TeamId != null)
                    .Select(p => new ProjectDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        TenantId = tenantId,
                        TeamId = p.TeamId,
                        Status = p.Status,
                        StartDate = p.StartDate,
                        EndDate = p.EndDate,
                        CreatedAt = p.CreatedAt,
                        TaskCount = 0
                    })
                    .ToListAsync();

                return projects;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get projects for tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<TaskDto> CreateTaskAsync(TaskDto task)
        {
            try
            {
                var taskEntity = new AttendancePlatform.Shared.Domain.Entities.ProjectTask
                {
                    Id = Guid.NewGuid(),
                    Title = task.Title,
                    Description = task.Description,
                    ProjectId = task.ProjectId,
                    AssignedUserId = task.AssignedUserId,
                    Status = "Todo",
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.ProjectTasks.Add(taskEntity);
                await _context.SaveChangesAsync();

                task.Id = taskEntity.Id;
                task.CreatedAt = taskEntity.CreatedAt;

                _logger.LogInformation("Task created: {TaskId}", taskEntity.Id);
                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task");
                throw;
            }
        }

        public async Task<List<TaskDto>> GetTasksAsync(Guid projectId)
        {
            try
            {
                var tasks = await _context.ProjectTasks
                    .Where(t => t.ProjectId == projectId)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Description = t.Description,
                        ProjectId = t.ProjectId,
                        AssignedUserId = t.AssignedUserId,
                        Status = t.Status,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get tasks for project: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<bool> UpdateTaskStatusAsync(Guid taskId, string status)
        {
            try
            {
                var task = await _context.ProjectTasks.FindAsync(taskId);
                if (task == null) return false;

                task.Status = status;
                task.UpdatedAt = DateTime.UtcNow;

                if (status == "Completed")
                    task.CompletedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Task status updated: {TaskId} to {Status}", taskId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update task status: {TaskId}", taskId);
                throw;
            }
        }

        public async Task<DocumentDto> CreateDocumentAsync(DocumentDto document)
        {
            try
            {
                var documentEntity = new AttendancePlatform.Shared.Domain.Entities.Document
                {
                    Id = Guid.NewGuid(),
                    Name = document.Name,
                    FileName = document.Name,
                    FilePath = "",
                    FileType = "text",
                    FileSize = 0,
                    ProjectId = document.ProjectId,
                    UploadedById = document.CreatedBy,
                    UploadedAt = DateTime.UtcNow,
                    Version = 1
                };

                _context.Documents.Add(documentEntity);
                await _context.SaveChangesAsync();

                document.Id = documentEntity.Id;
                document.CreatedAt = documentEntity.UploadedAt;

                _logger.LogInformation("Document created: {DocumentId}", documentEntity.Id);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create document");
                throw;
            }
        }

        public async Task<List<DocumentDto>> GetDocumentsAsync(Guid projectId)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => d.ProjectId == projectId)
                    .Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        ProjectId = d.ProjectId ?? Guid.Empty,
                        CreatedBy = d.UploadedById,
                        CreatedAt = d.UploadedAt,
                        UpdatedAt = d.UploadedAt,
                        Version = d.Version
                    })
                    .ToListAsync();

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get documents for project: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<bool> ShareDocumentAsync(Guid documentId, List<Guid> userIds)
        {
            try
            {
                var existingShares = await _context.DocumentShares
                    .Where(ds => ds.DocumentId == documentId)
                    .ToListAsync();

                _context.DocumentShares.RemoveRange(existingShares);

                foreach (var userId in userIds)
                {
                    var share = new AttendancePlatform.Shared.Domain.Entities.DocumentShare
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = documentId,
                        SharedWithId = userId,
                        SharedById = Guid.NewGuid(),
                        Permission = "Read",
                        SharedAt = DateTime.UtcNow
                    };

                    _context.DocumentShares.Add(share);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Document shared: {DocumentId} with {UserCount} users", documentId, userIds.Count);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to share document: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<ChatMessageDto> SendMessageAsync(ChatMessageDto message)
        {
            try
            {
                var messageEntity = new AttendancePlatform.Shared.Domain.Entities.ChatMessage
                {
                    Id = Guid.NewGuid(),
                    TeamId = message.TeamId,
                    SenderId = message.UserId,
                    Content = message.Content,
                    MessageType = message.MessageType,
                    SentAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(messageEntity);
                await _context.SaveChangesAsync();

                message.Id = messageEntity.Id;
                message.CreatedAt = messageEntity.CreatedAt;

                _logger.LogInformation("Chat message sent: {MessageId}", messageEntity.Id);
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send chat message");
                throw;
            }
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(Guid teamId)
        {
            try
            {
                var messages = await _context.ChatMessages
                    .Where(m => m.TeamId == teamId)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        TeamId = m.TeamId ?? Guid.Empty,
                        UserId = m.SenderId,
                        Content = m.Content,
                        MessageType = m.MessageType,
                        CreatedAt = m.CreatedAt
                    })
                    .ToListAsync();

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get messages for team: {TeamId}", teamId);
                throw;
            }
        }
    }


    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public List<TeamMemberDto> Members { get; set; } = new();
    }

    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }

    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TenantId { get; set; }
        public Guid? TeamId { get; set; }
        public string Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public Guid ProjectId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int Version { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
