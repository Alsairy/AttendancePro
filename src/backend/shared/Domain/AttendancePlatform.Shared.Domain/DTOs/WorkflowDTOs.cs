using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Shared.Domain.DTOs
{
    public class CreateWorkflowDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public Guid TenantId { get; set; }
        
        public List<WorkflowStepDto> Steps { get; set; } = new();
        
        public Dictionary<string, object> Variables { get; set; } = new();
        
        public bool IsActive { get; set; } = true;
    }

    public class UpdateWorkflowDto
    {
        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public List<WorkflowStepDto>? Steps { get; set; }
        
        public Dictionary<string, object>? Variables { get; set; }
        
        public bool? IsActive { get; set; }
    }

    public class WorkflowDefinitionDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public Guid TenantId { get; set; }
        
        public List<WorkflowStepDto> Steps { get; set; } = new();
        
        public Dictionary<string, object> Variables { get; set; } = new();
        
        public bool IsActive { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public string? UpdatedBy { get; set; }
    }

    public class StartWorkflowDto
    {
        [Required]
        public Guid WorkflowId { get; set; }
        
        [Required]
        public Guid TenantId { get; set; }
        
        public Dictionary<string, object> InitialVariables { get; set; } = new();
        
        public string? StartedBy { get; set; }
        
        public string? Context { get; set; }
    }





    public class WorkflowStepInstanceDto
    {
        public Guid Id { get; set; }
        
        public Guid StepId { get; set; }
        
        public string StepName { get; set; } = string.Empty;
        
        public WorkflowStepStatus Status { get; set; }
        
        public Dictionary<string, object> Inputs { get; set; } = new();
        
        public Dictionary<string, object> Outputs { get; set; } = new();
        
        public DateTime? StartedAt { get; set; }
        
        public DateTime? CompletedAt { get; set; }
        
        public string? ErrorMessage { get; set; }
        
        public string? ExecutedBy { get; set; }
    }

    public class WorkflowStepConditionDto
    {
        public string Field { get; set; } = string.Empty;
        
        public WorkflowConditionOperator Operator { get; set; }
        
        public object Value { get; set; } = new();
        
        public WorkflowConditionLogic Logic { get; set; } = WorkflowConditionLogic.And;
    }

    public class CreateFromTemplateDto
    {
        [Required]
        public Guid TemplateId { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public Guid TenantId { get; set; }
        
        public Dictionary<string, object> CustomVariables { get; set; } = new();
    }



    public class WorkflowValidationResultDto
    {
        public bool IsValid { get; set; }
        
        public List<WorkflowValidationErrorDto> Errors { get; set; } = new();
        
        public List<WorkflowValidationWarningDto> Warnings { get; set; } = new();
    }

    public class WorkflowValidationErrorDto
    {
        public string Code { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public string? StepId { get; set; }
        
        public string? Field { get; set; }
    }

    public class WorkflowValidationWarningDto
    {
        public string Code { get; set; } = string.Empty;
        
        public string Message { get; set; } = string.Empty;
        
        public string? StepId { get; set; }
        
        public string? Field { get; set; }
    }

    public class CreateCustomStepDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public WorkflowStepType Type { get; set; }
        
        public Dictionary<string, object> Configuration { get; set; } = new();
        
        public List<string> InputParameters { get; set; } = new();
        
        public List<string> OutputParameters { get; set; } = new();
        
        public bool IsReusable { get; set; } = true;
        
        public Guid TenantId { get; set; }
    }

    public class WorkflowAuditLogDto
    {
        public Guid Id { get; set; }
        
        public Guid WorkflowInstanceId { get; set; }
        
        public Guid? StepInstanceId { get; set; }
        
        public WorkflowAuditAction Action { get; set; }
        
        public string? Details { get; set; }
        
        public DateTime Timestamp { get; set; }
        
        public string? UserId { get; set; }
        
        public string? UserName { get; set; }
        
        public Dictionary<string, object> Metadata { get; set; } = new();
    }

    public class WorkflowStatisticsDto
    {
        public int TotalWorkflows { get; set; }
        
        public int ActiveWorkflows { get; set; }
        
        public int CompletedWorkflows { get; set; }
        
        public int FailedWorkflows { get; set; }
        
        public double AverageExecutionTime { get; set; }
        
        public double SuccessRate { get; set; }
        
        public List<WorkflowPerformanceMetricDto> PerformanceMetrics { get; set; } = new();
        
        public DateTime PeriodStart { get; set; }
        
        public DateTime PeriodEnd { get; set; }
    }

    public class WorkflowPerformanceMetricDto
    {
        public string MetricName { get; set; } = string.Empty;
        
        public double Value { get; set; }
        
        public string Unit { get; set; } = string.Empty;
        
        public DateTime Timestamp { get; set; }
    }

    public class WorkflowExecutionResultDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowId { get; set; }
        public Guid InstanceId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public long ExecutionTimeMs { get; set; }
        public Dictionary<string, object> Result { get; set; } = new();
    }

    public class WorkflowPerformanceDto
    {
        public Guid WorkflowId { get; set; }
        public string WorkflowName { get; set; } = string.Empty;
        public int TotalExecutions { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public double SuccessRate { get; set; }
        public long AverageExecutionTimeMs { get; set; }
        public long MinExecutionTimeMs { get; set; }
        public long MaxExecutionTimeMs { get; set; }
        public DateTime LastExecuted { get; set; }
    }

    public class StartWorkflowRequestDto
    {
        [Required]
        public Guid TenantId { get; set; }
        
        [Required]
        public Guid WorkflowTemplateId { get; set; }
        
        [Required]
        public Guid InitiatedBy { get; set; }
        
        public Dictionary<string, object> Context { get; set; } = new();
        
        public string Priority { get; set; } = "Medium";
    }

    public class ExecuteStepRequestDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;
        
        public Dictionary<string, object> Data { get; set; } = new();
        
        public string? Comments { get; set; }
        
        public Guid? ExecutedBy { get; set; }
    }

    public class CreateWorkflowInstanceRequest
    {
        [Required]
        public string WorkflowType { get; set; } = string.Empty;
        
        [Required]
        public Guid EntityId { get; set; }
        
        [Required]
        public string EntityType { get; set; } = string.Empty;
        
        [Required]
        public Guid InitiatedBy { get; set; }
        
        public Dictionary<string, object> InputData { get; set; } = new();
        
        public string Priority { get; set; } = "Medium";
    }

    public class ExecuteStepRequest
    {
        [Required]
        public string Action { get; set; } = string.Empty;
        
        public Dictionary<string, object> Data { get; set; } = new();
        
        public string? Comments { get; set; }
        
        public Guid? CompletedBy { get; set; }
        
        public Dictionary<string, object>? OutputData { get; set; }
    }

    public class CreateWorkflowTemplateRequest
    {
        [Required]
        public string WorkflowType { get; set; } = string.Empty;
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string StepDefinitions { get; set; } = string.Empty;
        
        public string Steps { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
    }

    public enum WorkflowStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Cancelled,
        Paused
    }

    public enum WorkflowStepType
    {
        Manual,
        Automated,
        Approval,
        Notification,
        DataTransformation,
        Integration,
        Condition,
        Loop,
        Parallel,
        Custom
    }

    public enum WorkflowStepStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Skipped,
        Cancelled
    }

    public enum WorkflowConditionOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        NotContains,
        StartsWith,
        EndsWith,
        IsNull,
        IsNotNull
    }

    public enum WorkflowConditionLogic
    {
        And,
        Or
    }

    public enum WorkflowAuditAction
    {
        Created,
        Started,
        StepStarted,
        StepCompleted,
        StepFailed,
        Paused,
        Resumed,
        Completed,
        Failed,
        Cancelled,
        VariableUpdated,
        ConfigurationChanged
    }
}
