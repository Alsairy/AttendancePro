using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Shared.Domain.Entities
{
    public class VoiceTemplate : TenantEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string TemplateText { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Language { get; set; } = "en-US";

        [MaxLength(50)]
        public string Category { get; set; } = "General";

        public bool IsActive { get; set; } = true;
        public int UsageCount { get; set; } = 0;

        public virtual ICollection<VoiceCommand> VoiceCommands { get; set; } = new List<VoiceCommand>();
    }

    public class VoiceConfiguration : TenantEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string Language { get; set; } = "en-US";

        public float ConfidenceThreshold { get; set; } = 0.8f;
        public bool IsEnabled { get; set; } = true;
        public bool RequireBiometric { get; set; } = false;

        [MaxLength(1000)]
        public string Settings { get; set; } = "{}"; // JSON configuration

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<VoiceCommand> VoiceCommands { get; set; } = new List<VoiceCommand>();
    }

    public class VoiceCommand : TenantEntity
    {
        public Guid? TemplateId { get; set; }
        public Guid? ConfigurationId { get; set; }
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Command { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Parameters { get; set; } = "{}"; // JSON parameters

        public float Confidence { get; set; }
        public bool Success { get; set; } = true;
        public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string ErrorMessage { get; set; } = string.Empty;

        public virtual VoiceTemplate? Template { get; set; }
        public virtual VoiceConfiguration? Configuration { get; set; }
        public virtual User User { get; set; } = null!;
    }

    public class ProjectTask : BaseEntity
    {
        public Guid ProjectId { get; set; }
        public Guid? AssignedUserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Todo";

        [MaxLength(20)]
        public string Priority { get; set; } = "Medium";

        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int EstimatedHours { get; set; } = 0;
        public int ActualHours { get; set; } = 0;

        public virtual TeamProject Project { get; set; } = null!;
        public virtual User? AssignedUser { get; set; }
    }

    public class DocumentShare : BaseEntity
    {
        public Guid DocumentId { get; set; }
        public Guid SharedWithId { get; set; }
        public Guid SharedById { get; set; }

        [MaxLength(20)]
        public string Permission { get; set; } = "Read"; // Read, Write, Admin

        public DateTime SharedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string Message { get; set; } = string.Empty;

        public virtual Document Document { get; set; } = null!;
        public virtual User SharedWith { get; set; } = null!;
        public virtual User SharedBy { get; set; } = null!;
    }

    public class EnterpriseIntegration : TenantEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string IntegrationType { get; set; } = string.Empty; // SAP, Oracle, Salesforce, etc.

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string ConnectionString { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Configuration { get; set; } = "{}"; // JSON configuration

        [MaxLength(20)]
        public string Status { get; set; } = "Active";

        public DateTime LastSync { get; set; } = DateTime.UtcNow;
        public DateTime? NextSync { get; set; }

        public bool IsEnabled { get; set; } = true;
        public int SyncIntervalMinutes { get; set; } = 60;

        [MaxLength(500)]
        public string LastError { get; set; } = string.Empty;

        public int RecordCount { get; set; } = 0;
        public float HealthScore { get; set; } = 100.0f;
    }
}
