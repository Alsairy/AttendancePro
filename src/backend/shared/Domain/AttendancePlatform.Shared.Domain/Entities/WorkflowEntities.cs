using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendancePlatform.Shared.Domain.Entities;

[Table("WorkflowDefinitions")]
public class WorkflowDefinition : TenantEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public string Steps { get; set; } = string.Empty;

    public string Triggers { get; set; } = string.Empty;

    public string Variables { get; set; } = "{}";

    public bool IsActive { get; set; } = true;

    public int Version { get; set; } = 1;

    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [MaxLength(50)]
    public string? UpdatedBy { get; set; }

    public DateTime? LastExecutedAt { get; set; }

    public int ExecutionCount { get; set; } = 0;

    public virtual ICollection<WorkflowInstance> Instances { get; set; } = new List<WorkflowInstance>();
}

[Table("WorkflowInstances")]
public class WorkflowInstance : TenantEntity
{
    [Required]
    [MaxLength(50)]
    public string WorkflowDefinitionId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "running";

    public string InputData { get; set; } = "{}";

    public string OutputData { get; set; } = "{}";

    public string Variables { get; set; } = "{}";

    public string CurrentStepId { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    [MaxLength(50)]
    public string? StartedBy { get; set; }

    [MaxLength(50)]
    public string? CompletedBy { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public int RetryCount { get; set; } = 0;

    public int MaxRetries { get; set; } = 3;

    [MaxLength(100)]
    public string? CorrelationId { get; set; }

    public virtual WorkflowDefinition WorkflowDefinition { get; set; } = null!;
    public virtual ICollection<WorkflowTask> Tasks { get; set; } = new List<WorkflowTask>();
    public virtual ICollection<WorkflowExecutionLog> ExecutionLogs { get; set; } = new List<WorkflowExecutionLog>();
}

[Table("WorkflowTasks")]
public class WorkflowTask : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string StepId { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = "pending";

    [MaxLength(50)]
    public string? AssignedTo { get; set; }

    [MaxLength(50)]
    public string? AssignedBy { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CompletedAt { get; set; }

    [MaxLength(50)]
    public string? CompletedBy { get; set; }

    public string InputData { get; set; } = "{}";

    public string OutputData { get; set; } = "{}";

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public int RetryCount { get; set; } = 0;

    [MaxLength(20)]
    public string Priority { get; set; } = "normal";

    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}

[Table("WorkflowExecutionLogs")]
public class WorkflowExecutionLog : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string WorkflowInstanceId { get; set; } = string.Empty;

    [MaxLength(100)]
    public string StepId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Action { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Message { get; set; }

    public string? Data { get; set; }

    [MaxLength(50)]
    public string? UserId { get; set; }

    public DateTime ExecutedAt { get; set; } = DateTime.UtcNow;

    public TimeSpan? Duration { get; set; }

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;
}

[Table("WorkflowTemplates")]
public class WorkflowTemplate : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    public string Definition { get; set; } = string.Empty;

    public string Parameters { get; set; } = "{}";

    public bool IsPublic { get; set; } = false;

    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    public int UsageCount { get; set; } = 0;

    public double Rating { get; set; } = 0.0;

    public string Tags { get; set; } = string.Empty;
}

public class WorkflowStep
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<string> NextSteps { get; set; } = new();
    public List<WorkflowCondition> Conditions { get; set; } = new();
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
    public TimeSpan? Timeout { get; set; }
    public int MaxRetries { get; set; } = 0;
}

public class WorkflowTrigger
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Dictionary<string, object> Properties { get; set; } = new();
    public List<WorkflowCondition> Conditions { get; set; } = new();
    public bool IsActive { get; set; } = true;
}

public class WorkflowCondition
{
    public string Field { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public object Value { get; set; } = new();
    public string LogicalOperator { get; set; } = "AND";
}
