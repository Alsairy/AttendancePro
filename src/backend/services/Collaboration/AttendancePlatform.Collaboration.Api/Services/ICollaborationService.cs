using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Collaboration.Api.Services
{
    public interface ICollaborationService
    {
        Task<TeamDto> CreateTeamAsync(CreateTeamDto createTeam);
        Task<TeamDto> UpdateTeamAsync(Guid teamId, UpdateTeamDto updateTeam);
        Task<bool> DeleteTeamAsync(Guid teamId);
        Task<TeamDto> GetTeamAsync(Guid teamId);
        Task<IEnumerable<TeamDto>> GetUserTeamsAsync(Guid userId);
        Task<bool> AddTeamMemberAsync(Guid teamId, Guid userId, TeamRole role);
        Task<bool> RemoveTeamMemberAsync(Guid teamId, Guid userId);
        Task<bool> UpdateTeamMemberRoleAsync(Guid teamId, Guid userId, TeamRole newRole);
        
        Task<ChatMessageDto> SendMessageAsync(SendMessageDto sendMessage);
        Task<IEnumerable<ChatMessageDto>> GetTeamMessagesAsync(Guid teamId, int page = 1, int pageSize = 50);
        Task<IEnumerable<ChatMessageDto>> GetDirectMessagesAsync(Guid userId1, Guid userId2, int page = 1, int pageSize = 50);
        Task<bool> DeleteMessageAsync(Guid messageId);
        Task<ChatMessageDto> EditMessageAsync(Guid messageId, string newContent);
        
        Task<VideoConferenceDto> StartVideoConferenceAsync(StartVideoConferenceDto startConference);
        Task<bool> JoinVideoConferenceAsync(Guid conferenceId, Guid userId);
        Task<bool> LeaveVideoConferenceAsync(Guid conferenceId, Guid userId);
        Task<bool> EndVideoConferenceAsync(Guid conferenceId);
        Task<VideoConferenceDto> GetVideoConferenceAsync(Guid conferenceId);
        Task<IEnumerable<VideoConferenceDto>> GetActiveConferencesAsync(Guid tenantId);
        
        Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto createDocument);
        Task<DocumentDto> UpdateDocumentAsync(Guid documentId, UpdateDocumentDto updateDocument);
        Task<bool> DeleteDocumentAsync(Guid documentId);
        Task<DocumentDto> GetDocumentAsync(Guid documentId);
        Task<IEnumerable<DocumentDto>> GetTeamDocumentsAsync(Guid teamId);
        Task<DocumentVersionDto> CreateDocumentVersionAsync(Guid documentId, CreateDocumentVersionDto createVersion);
        Task<IEnumerable<DocumentVersionDto>> GetDocumentVersionsAsync(Guid documentId);
        Task<bool> ShareDocumentAsync(Guid documentId, Guid userId, DocumentPermission permission);
        Task<bool> RevokeDocumentAccessAsync(Guid documentId, Guid userId);
        
        Task<PresenceDto> UpdatePresenceAsync(Guid userId, UpdatePresenceDto updatePresence);
        Task<PresenceDto> GetUserPresenceAsync(Guid userId);
        Task<IEnumerable<PresenceDto>> GetTeamPresenceAsync(Guid teamId);
        
        Task<ScreenShareSessionDto> StartScreenShareAsync(StartScreenShareDto startScreenShare);
        Task<bool> JoinScreenShareAsync(Guid sessionId, Guid userId);
        Task<bool> EndScreenShareAsync(Guid sessionId);
        Task<ScreenShareSessionDto> GetScreenShareSessionAsync(Guid sessionId);
        
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTask);
        Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateTask);
        Task<bool> DeleteTaskAsync(Guid taskId);
        Task<TaskDto> GetTaskAsync(Guid taskId);
        Task<IEnumerable<TaskDto>> GetTeamTasksAsync(Guid teamId);
        Task<IEnumerable<TaskDto>> GetUserTasksAsync(Guid userId);
        Task<bool> AssignTaskAsync(Guid taskId, Guid userId);
        Task<bool> CompleteTaskAsync(Guid taskId);
        
        Task<CollaborationAnalyticsDto> GetCollaborationAnalyticsAsync(Guid tenantId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<ActivityDto>> GetTeamActivityAsync(Guid teamId, DateTime startDate, DateTime endDate);
    }
}
