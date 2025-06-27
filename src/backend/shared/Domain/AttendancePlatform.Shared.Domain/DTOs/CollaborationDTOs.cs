namespace AttendancePlatform.Shared.Domain.DTOs
{
    public enum TeamRole
    {
        Member,
        Moderator,
        Admin,
        Owner
    }

    public enum DocumentPermission
    {
        Read,
        Write,
        Admin
    }

    public enum PresenceStatus
    {
        Online,
        Away,
        Busy,
        DoNotDisturb,
        Offline
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Cancelled,
        OnHold
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class TeamDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<TeamMemberDto> Members { get; set; } = new List<TeamMemberDto>();
    }

    public class CreateTeamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
        public IEnumerable<Guid> InitialMembers { get; set; } = new List<Guid>();
    }

    public class UpdateTeamDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class TeamMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public TeamRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
        public PresenceStatus Status { get; set; }
    }

    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? RecipientId { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public IEnumerable<MessageAttachmentDto> Attachments { get; set; }
        public MessageType Type { get; set; }
    }

    public enum MessageType
    {
        Text,
        File,
        Image,
        System,
        Emoji
    }

    public class MessageAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FileUrl { get; set; }
    }

    public class SendMessageDto
    {
        public string Content { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? RecipientId { get; set; }
        public MessageType Type { get; set; }
        public IEnumerable<MessageAttachmentDto> Attachments { get; set; }
    }

    public class VideoConferenceDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid HostId { get; set; }
        public string HostName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string MeetingUrl { get; set; }
        public string MeetingId { get; set; }
        public bool IsRecording { get; set; }
        public IEnumerable<ConferenceParticipantDto> Participants { get; set; }
        public ConferenceStatus Status { get; set; }
    }

    public enum ConferenceStatus
    {
        Scheduled,
        Active,
        Ended,
        Cancelled
    }

    public class ConferenceParticipantDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }
        public bool IsMuted { get; set; }
        public bool IsVideoOn { get; set; }
        public bool IsScreenSharing { get; set; }
    }

    public class StartVideoConferenceDto
    {
        public string Title { get; set; }
        public Guid? TeamId { get; set; }
        public IEnumerable<Guid> InvitedUsers { get; set; }
        public bool AllowRecording { get; set; }
        public DateTime? ScheduledStartTime { get; set; }
    }

    public class DocumentDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public Guid? TeamId { get; set; }
        public DocumentVersionDto CurrentVersion { get; set; }
        public IEnumerable<DocumentPermissionDto> Permissions { get; set; }
        public bool IsLocked { get; set; }
        public Guid? LockedBy { get; set; }
    }

    public class CreateDocumentDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid? TeamId { get; set; }
        public IEnumerable<DocumentPermissionDto> InitialPermissions { get; set; }
    }

    public class UpdateDocumentDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ChangeDescription { get; set; }
    }

    public class DocumentVersionDto
    {
        public Guid Id { get; set; }
        public int VersionNumber { get; set; }
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ChangeDescription { get; set; }
    }

    public class CreateDocumentVersionDto
    {
        public string Content { get; set; }
        public string ChangeDescription { get; set; }
    }

    public class DocumentPermissionDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DocumentPermission Permission { get; set; }
        public DateTime GrantedAt { get; set; }
        public Guid GrantedBy { get; set; }
    }

    public class PresenceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public PresenceStatus Status { get; set; }
        public string StatusMessage { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsOnline { get; set; }
        public string CurrentActivity { get; set; }
    }

    public class UpdatePresenceDto
    {
        public PresenceStatus Status { get; set; }
        public string StatusMessage { get; set; }
        public string CurrentActivity { get; set; }
    }

    public class ScreenShareSessionDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public string HostName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string SessionUrl { get; set; }
        public IEnumerable<ScreenShareParticipantDto> Participants { get; set; }
        public bool IsActive { get; set; }
    }

    public class ScreenShareParticipantDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool CanControl { get; set; }
    }

    public class StartScreenShareDto
    {
        public Guid? TeamId { get; set; }
        public IEnumerable<Guid> InvitedUsers { get; set; }
        public bool AllowControl { get; set; }
        public string Title { get; set; }
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public Guid? AssignedTo { get; set; }
        public string AssignedToName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Guid? TeamId { get; set; }
        public IEnumerable<TaskCommentDto> Comments { get; set; }
        public IEnumerable<TaskAttachmentDto> Attachments { get; set; }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? TeamId { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public Guid? AssignedTo { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class TaskCommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public Guid CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TaskAttachmentDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string FileUrl { get; set; }
        public Guid UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class CollaborationAnalyticsDto
    {
        public int TotalTeams { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalMessages { get; set; }
        public int TotalDocuments { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int ActiveVideoConferences { get; set; }
        public Dictionary<string, int> ActivityByType { get; set; }
        public IEnumerable<CollaborationTrendDto> Trends { get; set; }
    }

    public class CollaborationTrendDto
    {
        public DateTime Date { get; set; }
        public int MessageCount { get; set; }
        public int DocumentEdits { get; set; }
        public int TasksCompleted { get; set; }
        public int VideoConferences { get; set; }
    }

    public class ActivityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }
}
