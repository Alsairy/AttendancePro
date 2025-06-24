using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using System.Text.Json;

namespace AttendancePlatform.Workflow.Api.Services
{
    public interface IAdvancedWorkflowService
    {
        Task<ApiResponse<WorkflowInstanceDto>> CreateWorkflowInstanceAsync(Guid tenantId, CreateWorkflowInstanceRequest request);
        Task<ApiResponse<WorkflowInstanceDto>> ExecuteWorkflowStepAsync(Guid workflowInstanceId, ExecuteStepRequest request);
        Task<ApiResponse<List<WorkflowInstanceDto>>> GetActiveWorkflowsAsync(Guid tenantId, string? workflowType = null);
        Task<ApiResponse<WorkflowInstanceDto>> GetWorkflowInstanceAsync(Guid workflowInstanceId);
        Task<ApiResponse<bool>> CancelWorkflowInstanceAsync(Guid workflowInstanceId, string reason);
        Task<ApiResponse<List<WorkflowTemplateDto>>> GetWorkflowTemplatesAsync(Guid tenantId);
        Task<ApiResponse<WorkflowTemplateDto>> CreateWorkflowTemplateAsync(Guid tenantId, CreateWorkflowTemplateRequest request);
        Task<ApiResponse<List<WorkflowExecutionLogDto>>> GetWorkflowExecutionLogsAsync(Guid workflowInstanceId);
        Task<ApiResponse<WorkflowMetricsDto>> GetWorkflowMetricsAsync(Guid tenantId, DateTime startDate, DateTime endDate);
        Task<ApiResponse<bool>> RetryFailedWorkflowStepAsync(Guid workflowInstanceId, Guid stepId);
    }

    public class AdvancedWorkflowService : IAdvancedWorkflowService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<AdvancedWorkflowService> _logger;
        private readonly IWorkflowExecutionEngine _executionEngine;
        private readonly INotificationService _notificationService;

        public AdvancedWorkflowService(
            AttendancePlatformDbContext context,
            ILogger<AdvancedWorkflowService> logger,
            IWorkflowExecutionEngine executionEngine,
            INotificationService notificationService)
        {
            _context = context;
            _logger = logger;
            _executionEngine = executionEngine;
            _notificationService = notificationService;
        }

        public async Task<ApiResponse<WorkflowInstanceDto>> CreateWorkflowInstanceAsync(Guid tenantId, CreateWorkflowInstanceRequest request)
        {
            try
            {
                _logger.LogInformation("Creating workflow instance for tenant {TenantId}, type {WorkflowType}", tenantId, request.WorkflowType);

                var template = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(wt => wt.TenantId == tenantId && wt.WorkflowType == request.WorkflowType && wt.IsActive);

                if (template == null)
                {
                    return ApiResponse<WorkflowInstanceDto>.ErrorResult($"No active workflow template found for type {request.WorkflowType}");
                }

                var workflowInstance = new WorkflowInstance
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    WorkflowTemplateId = template.Id,
                    WorkflowType = request.WorkflowType,
                    EntityId = request.EntityId,
                    EntityType = request.EntityType,
                    InitiatedBy = request.InitiatedBy,
                    Status = "Running",
                    Priority = request.Priority ?? "Medium",
                    InputData = JsonSerializer.Serialize(request.InputData ?? new Dictionary<string, object>()),
                    CurrentStepIndex = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.WorkflowInstances.Add(workflowInstance);

                var firstStep = JsonSerializer.Deserialize<List<WorkflowStepDefinition>>(template.StepDefinitions)?.FirstOrDefault();
                if (firstStep != null)
                {
                    var workflowStep = new WorkflowStep
                    {
                        Id = Guid.NewGuid(),
                        WorkflowInstanceId = workflowInstance.Id,
                        StepName = firstStep.Name,
                        StepType = firstStep.Type,
                        StepIndex = 0,
                        Status = "Pending",
                        AssignedTo = firstStep.AssignedTo,
                        DueDate = firstStep.DueDays.HasValue ? DateTime.UtcNow.AddDays(firstStep.DueDays.Value) : null,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.WorkflowSteps.Add(workflowStep);
                }

                await _context.SaveChangesAsync();

                if (firstStep?.IsAutomated == true)
                {
                    await _executionEngine.ExecuteAutomatedStepAsync(workflowInstance.Id, firstStep);
                }

                var result = await GetWorkflowInstanceAsync(workflowInstance.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow instance for tenant {TenantId}", tenantId);
                return ApiResponse<WorkflowInstanceDto>.ErrorResult("Failed to create workflow instance");
            }
        }

        public async Task<ApiResponse<WorkflowInstanceDto>> ExecuteWorkflowStepAsync(Guid workflowInstanceId, ExecuteStepRequest request)
        {
            try
            {
                _logger.LogInformation("Executing workflow step for instance {WorkflowInstanceId}", workflowInstanceId);

                var workflowInstance = await _context.WorkflowInstances
                    .Include(wi => wi.WorkflowSteps)
                    .Include(wi => wi.WorkflowTemplate)
                    .FirstOrDefaultAsync(wi => wi.Id == workflowInstanceId);

                if (workflowInstance == null)
                {
                    return ApiResponse<WorkflowInstanceDto>.ErrorResult("Workflow instance not found");
                }

                var currentStep = workflowInstance.WorkflowSteps
                    .FirstOrDefault(ws => ws.StepIndex == workflowInstance.CurrentStepIndex && ws.Status == "Pending");

                if (currentStep == null)
                {
                    return ApiResponse<WorkflowInstanceDto>.ErrorResult("No pending step found for execution");
                }

                currentStep.Status = request.Action == "Approve" ? "Completed" : "Rejected";
                currentStep.CompletedAt = DateTime.UtcNow;
                currentStep.CompletedBy = request.CompletedBy;
                currentStep.Comments = request.Comments;
                currentStep.OutputData = JsonSerializer.Serialize(request.OutputData ?? new Dictionary<string, object>());

                var executionLog = new WorkflowExecutionLog
                {
                    Id = Guid.NewGuid(),
                    WorkflowInstanceId = workflowInstanceId,
                    StepId = currentStep.Id,
                    Action = request.Action,
                    ExecutedBy = request.CompletedBy,
                    ExecutedAt = DateTime.UtcNow,
                    Comments = request.Comments,
                    InputData = JsonSerializer.Serialize(request.OutputData ?? new Dictionary<string, object>())
                };

                _context.WorkflowExecutionLogs.Add(executionLog);

                if (request.Action == "Approve")
                {
                    var stepDefinitions = JsonSerializer.Deserialize<List<WorkflowStepDefinition>>(workflowInstance.WorkflowTemplate.StepDefinitions);
                    var nextStepIndex = workflowInstance.CurrentStepIndex + 1;

                    if (nextStepIndex < stepDefinitions?.Count)
                    {
                        var nextStepDef = stepDefinitions[nextStepIndex];
                        var nextStep = new WorkflowStep
                        {
                            Id = Guid.NewGuid(),
                            WorkflowInstanceId = workflowInstanceId,
                            StepName = nextStepDef.Name,
                            StepType = nextStepDef.Type,
                            StepIndex = nextStepIndex,
                            Status = "Pending",
                            AssignedTo = nextStepDef.AssignedTo,
                            DueDate = nextStepDef.DueDays.HasValue ? DateTime.UtcNow.AddDays(nextStepDef.DueDays.Value) : null,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.WorkflowSteps.Add(nextStep);
                        workflowInstance.CurrentStepIndex = nextStepIndex;

                        if (!string.IsNullOrEmpty(nextStepDef.AssignedTo))
                        {
                            await _notificationService.SendWorkflowNotificationAsync(
                                workflowInstanceId, 
                                nextStepDef.AssignedTo, 
                                $"New workflow step assigned: {nextStepDef.Name}");
                        }

                        if (nextStepDef.IsAutomated)
                        {
                            await _executionEngine.ExecuteAutomatedStepAsync(workflowInstanceId, nextStepDef);
                        }
                    }
                    else
                    {
                        workflowInstance.Status = "Completed";
                        workflowInstance.CompletedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    workflowInstance.Status = "Rejected";
                    workflowInstance.CompletedAt = DateTime.UtcNow;
                }

                workflowInstance.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var result = await GetWorkflowInstanceAsync(workflowInstanceId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow step for instance {WorkflowInstanceId}", workflowInstanceId);
                return ApiResponse<WorkflowInstanceDto>.ErrorResult("Failed to execute workflow step");
            }
        }

        public async Task<ApiResponse<List<WorkflowInstanceDto>>> GetActiveWorkflowsAsync(Guid tenantId, string? workflowType = null)
        {
            try
            {
                _logger.LogInformation("Getting active workflows for tenant {TenantId}", tenantId);

                var query = _context.WorkflowInstances
                    .Where(wi => wi.TenantId == tenantId && wi.Status == "Running")
                    .Include(wi => wi.WorkflowSteps)
                    .Include(wi => wi.WorkflowTemplate);

                if (!string.IsNullOrEmpty(workflowType))
                {
                    query = query.Where(wi => wi.WorkflowType == workflowType);
                }

                var workflows = await query
                    .OrderByDescending(wi => wi.CreatedAt)
                    .ToListAsync();

                var result = workflows.Select(wi => new WorkflowInstanceDto
                {
                    Id = wi.Id,
                    TenantId = wi.TenantId,
                    WorkflowType = wi.WorkflowType,
                    EntityId = wi.EntityId,
                    EntityType = wi.EntityType,
                    Status = wi.Status,
                    Priority = wi.Priority,
                    CurrentStepIndex = wi.CurrentStepIndex,
                    CurrentStepName = wi.WorkflowSteps.FirstOrDefault(ws => ws.StepIndex == wi.CurrentStepIndex)?.StepName ?? "",
                    InitiatedBy = wi.InitiatedBy,
                    CreatedAt = wi.CreatedAt,
                    UpdatedAt = wi.UpdatedAt,
                    CompletedAt = wi.CompletedAt,
                    TotalSteps = wi.WorkflowTemplate != null ? 
                        JsonSerializer.Deserialize<List<WorkflowStepDefinition>>(wi.WorkflowTemplate.StepDefinitions)?.Count ?? 0 : 0
                }).ToList();

                return ApiResponse<List<WorkflowInstanceDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active workflows for tenant {TenantId}", tenantId);
                return ApiResponse<List<WorkflowInstanceDto>>.ErrorResult("Failed to get active workflows");
            }
        }

        public async Task<ApiResponse<WorkflowInstanceDto>> GetWorkflowInstanceAsync(Guid workflowInstanceId)
        {
            try
            {
                var workflowInstance = await _context.WorkflowInstances
                    .Include(wi => wi.WorkflowSteps)
                    .Include(wi => wi.WorkflowTemplate)
                    .FirstOrDefaultAsync(wi => wi.Id == workflowInstanceId);

                if (workflowInstance == null)
                {
                    return ApiResponse<WorkflowInstanceDto>.ErrorResult("Workflow instance not found");
                }

                var result = new WorkflowInstanceDto
                {
                    Id = workflowInstance.Id,
                    TenantId = workflowInstance.TenantId,
                    WorkflowType = workflowInstance.WorkflowType,
                    EntityId = workflowInstance.EntityId,
                    EntityType = workflowInstance.EntityType,
                    Status = workflowInstance.Status,
                    Priority = workflowInstance.Priority,
                    CurrentStepIndex = workflowInstance.CurrentStepIndex,
                    CurrentStepName = workflowInstance.WorkflowSteps.FirstOrDefault(ws => ws.StepIndex == workflowInstance.CurrentStepIndex)?.StepName ?? "",
                    InitiatedBy = workflowInstance.InitiatedBy,
                    CreatedAt = workflowInstance.CreatedAt,
                    UpdatedAt = workflowInstance.UpdatedAt,
                    CompletedAt = workflowInstance.CompletedAt,
                    TotalSteps = workflowInstance.WorkflowTemplate != null ? 
                        JsonSerializer.Deserialize<List<WorkflowStepDefinition>>(workflowInstance.WorkflowTemplate.StepDefinitions)?.Count ?? 0 : 0,
                    Steps = workflowInstance.WorkflowSteps.Select(ws => new WorkflowStepDto
                    {
                        Id = ws.Id,
                        StepName = ws.StepName,
                        StepType = ws.StepType,
                        StepIndex = ws.StepIndex,
                        Status = ws.Status,
                        AssignedTo = ws.AssignedTo,
                        DueDate = ws.DueDate,
                        CreatedAt = ws.CreatedAt,
                        CompletedAt = ws.CompletedAt,
                        CompletedBy = ws.CompletedBy,
                        Comments = ws.Comments
                    }).OrderBy(ws => ws.StepIndex).ToList()
                };

                return ApiResponse<WorkflowInstanceDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow instance {WorkflowInstanceId}", workflowInstanceId);
                return ApiResponse<WorkflowInstanceDto>.ErrorResult("Failed to get workflow instance");
            }
        }

        public async Task<ApiResponse<bool>> CancelWorkflowInstanceAsync(Guid workflowInstanceId, string reason)
        {
            try
            {
                _logger.LogInformation("Cancelling workflow instance {WorkflowInstanceId}", workflowInstanceId);

                var workflowInstance = await _context.WorkflowInstances
                    .FirstOrDefaultAsync(wi => wi.Id == workflowInstanceId);

                if (workflowInstance == null)
                {
                    return ApiResponse<bool>.ErrorResult("Workflow instance not found");
                }

                workflowInstance.Status = "Cancelled";
                workflowInstance.CompletedAt = DateTime.UtcNow;
                workflowInstance.UpdatedAt = DateTime.UtcNow;

                var pendingSteps = await _context.WorkflowSteps
                    .Where(ws => ws.WorkflowInstanceId == workflowInstanceId && ws.Status == "Pending")
                    .ToListAsync();

                foreach (var step in pendingSteps)
                {
                    step.Status = "Cancelled";
                    step.Comments = reason;
                }

                var executionLog = new WorkflowExecutionLog
                {
                    Id = Guid.NewGuid(),
                    WorkflowInstanceId = workflowInstanceId,
                    Action = "Cancel",
                    ExecutedAt = DateTime.UtcNow,
                    Comments = reason
                };

                _context.WorkflowExecutionLogs.Add(executionLog);
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling workflow instance {WorkflowInstanceId}", workflowInstanceId);
                return ApiResponse<bool>.ErrorResult("Failed to cancel workflow instance");
            }
        }

        public async Task<ApiResponse<List<WorkflowTemplateDto>>> GetWorkflowTemplatesAsync(Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Getting workflow templates for tenant {TenantId}", tenantId);

                var templates = await _context.WorkflowTemplates
                    .Where(wt => wt.TenantId == tenantId && wt.IsActive)
                    .OrderBy(wt => wt.WorkflowType)
                    .ToListAsync();

                var result = templates.Select(wt => new WorkflowTemplateDto
                {
                    Id = wt.Id,
                    TenantId = wt.TenantId,
                    WorkflowType = wt.WorkflowType,
                    Name = wt.Name,
                    Description = wt.Description,
                    IsActive = wt.IsActive,
                    CreatedAt = wt.CreatedAt,
                    UpdatedAt = wt.UpdatedAt,
                    StepCount = JsonSerializer.Deserialize<List<WorkflowStepDefinition>>(wt.StepDefinitions)?.Count ?? 0
                }).ToList();

                return ApiResponse<List<WorkflowTemplateDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow templates for tenant {TenantId}", tenantId);
                return ApiResponse<List<WorkflowTemplateDto>>.ErrorResult("Failed to get workflow templates");
            }
        }

        public async Task<ApiResponse<WorkflowTemplateDto>> CreateWorkflowTemplateAsync(Guid tenantId, CreateWorkflowTemplateRequest request)
        {
            try
            {
                _logger.LogInformation("Creating workflow template for tenant {TenantId}, type {WorkflowType}", tenantId, request.WorkflowType);

                var template = new WorkflowTemplate
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    WorkflowType = request.WorkflowType,
                    Name = request.Name,
                    Description = request.Description,
                    StepDefinitions = JsonSerializer.Serialize(request.Steps),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.WorkflowTemplates.Add(template);
                await _context.SaveChangesAsync();

                var result = new WorkflowTemplateDto
                {
                    Id = template.Id,
                    TenantId = template.TenantId,
                    WorkflowType = template.WorkflowType,
                    Name = template.Name,
                    Description = template.Description,
                    IsActive = template.IsActive,
                    CreatedAt = template.CreatedAt,
                    UpdatedAt = template.UpdatedAt,
                    StepCount = request.Steps.Count
                };

                return ApiResponse<WorkflowTemplateDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow template for tenant {TenantId}", tenantId);
                return ApiResponse<WorkflowTemplateDto>.ErrorResult("Failed to create workflow template");
            }
        }

        public async Task<ApiResponse<List<WorkflowExecutionLogDto>>> GetWorkflowExecutionLogsAsync(Guid workflowInstanceId)
        {
            try
            {
                _logger.LogInformation("Getting workflow execution logs for instance {WorkflowInstanceId}", workflowInstanceId);

                var logs = await _context.WorkflowExecutionLogs
                    .Where(wel => wel.WorkflowInstanceId == workflowInstanceId)
                    .OrderByDescending(wel => wel.ExecutedAt)
                    .ToListAsync();

                var result = logs.Select(log => new WorkflowExecutionLogDto
                {
                    Id = log.Id,
                    WorkflowInstanceId = log.WorkflowInstanceId,
                    StepId = log.StepId,
                    Action = log.Action,
                    ExecutedBy = log.ExecutedBy,
                    ExecutedAt = log.ExecutedAt,
                    Comments = log.Comments
                }).ToList();

                return ApiResponse<List<WorkflowExecutionLogDto>>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow execution logs for instance {WorkflowInstanceId}", workflowInstanceId);
                return ApiResponse<List<WorkflowExecutionLogDto>>.ErrorResult("Failed to get workflow execution logs");
            }
        }

        public async Task<ApiResponse<WorkflowMetricsDto>> GetWorkflowMetricsAsync(Guid tenantId, DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting workflow metrics for tenant {TenantId}", tenantId);

                var workflows = await _context.WorkflowInstances
                    .Where(wi => wi.TenantId == tenantId && wi.CreatedAt >= startDate && wi.CreatedAt <= endDate)
                    .ToListAsync();

                var metrics = new WorkflowMetricsDto
                {
                    TotalWorkflows = workflows.Count,
                    CompletedWorkflows = workflows.Count(w => w.Status == "Completed"),
                    RejectedWorkflows = workflows.Count(w => w.Status == "Rejected"),
                    CancelledWorkflows = workflows.Count(w => w.Status == "Cancelled"),
                    ActiveWorkflows = workflows.Count(w => w.Status == "Running"),
                    AverageCompletionTime = workflows.Where(w => w.CompletedAt.HasValue)
                        .Select(w => (w.CompletedAt!.Value - w.CreatedAt).TotalHours)
                        .DefaultIfEmpty(0)
                        .Average(),
                    WorkflowsByType = workflows.GroupBy(w => w.WorkflowType)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    CompletionRate = workflows.Count > 0 ? 
                        (double)workflows.Count(w => w.Status == "Completed") / workflows.Count * 100 : 0
                };

                return ApiResponse<WorkflowMetricsDto>.SuccessResult(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow metrics for tenant {TenantId}", tenantId);
                return ApiResponse<WorkflowMetricsDto>.ErrorResult("Failed to get workflow metrics");
            }
        }

        public async Task<ApiResponse<bool>> RetryFailedWorkflowStepAsync(Guid workflowInstanceId, Guid stepId)
        {
            try
            {
                _logger.LogInformation("Retrying failed workflow step {StepId} for instance {WorkflowInstanceId}", stepId, workflowInstanceId);

                var step = await _context.WorkflowSteps
                    .FirstOrDefaultAsync(ws => ws.Id == stepId && ws.WorkflowInstanceId == workflowInstanceId);

                if (step == null)
                {
                    return ApiResponse<bool>.ErrorResult("Workflow step not found");
                }

                if (step.Status != "Failed")
                {
                    return ApiResponse<bool>.ErrorResult("Only failed steps can be retried");
                }

                step.Status = "Pending";
                step.RetryCount = (step.RetryCount ?? 0) + 1;
                step.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying workflow step {StepId}", stepId);
                return ApiResponse<bool>.ErrorResult("Failed to retry workflow step");
            }
        }
    }

    public interface IWorkflowExecutionEngine
    {
        Task<bool> ExecuteAutomatedStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition);
    }

    public class WorkflowExecutionEngine : IWorkflowExecutionEngine
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<WorkflowExecutionEngine> _logger;
        private readonly IServiceProvider _serviceProvider;

        public WorkflowExecutionEngine(
            AttendancePlatformDbContext context,
            ILogger<WorkflowExecutionEngine> logger,
            IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<bool> ExecuteAutomatedStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition)
        {
            try
            {
                _logger.LogInformation("Executing automated step {StepName} for workflow {WorkflowInstanceId}", 
                    stepDefinition.Name, workflowInstanceId);

                var step = await _context.WorkflowSteps
                    .FirstOrDefaultAsync(ws => ws.WorkflowInstanceId == workflowInstanceId && 
                                              ws.StepName == stepDefinition.Name && 
                                              ws.Status == "Pending");

                if (step == null)
                {
                    return false;
                }

                step.Status = "InProgress";
                step.StartedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                bool success = false;

                switch (stepDefinition.Type.ToLower())
                {
                    case "notification":
                        success = await ExecuteNotificationStepAsync(workflowInstanceId, stepDefinition);
                        break;
                    case "approval":
                        success = await ExecuteApprovalStepAsync(workflowInstanceId, stepDefinition);
                        break;
                    case "calculation":
                        success = await ExecuteCalculationStepAsync(workflowInstanceId, stepDefinition);
                        break;
                    case "integration":
                        success = await ExecuteIntegrationStepAsync(workflowInstanceId, stepDefinition);
                        break;
                    default:
                        _logger.LogWarning("Unknown automated step type: {StepType}", stepDefinition.Type);
                        break;
                }

                step.Status = success ? "Completed" : "Failed";
                step.CompletedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing automated step {StepName}", stepDefinition.Name);
                return false;
            }
        }

        private async Task<bool> ExecuteNotificationStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition)
        {
            try
            {
                var notificationService = _serviceProvider.GetService<INotificationService>();
                if (notificationService != null && !string.IsNullOrEmpty(stepDefinition.AssignedTo))
                {
                    await notificationService.SendWorkflowNotificationAsync(
                        workflowInstanceId, 
                        stepDefinition.AssignedTo, 
                        stepDefinition.Description ?? "Workflow notification");
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing notification step");
                return false;
            }
        }

        private async Task<bool> ExecuteApprovalStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition)
        {
            return true;
        }

        private async Task<bool> ExecuteCalculationStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition)
        {
            return true;
        }

        private async Task<bool> ExecuteIntegrationStepAsync(Guid workflowInstanceId, WorkflowStepDefinition stepDefinition)
        {
            return true;
        }
    }

    // Request/Response DTOs
    public class CreateWorkflowInstanceRequest
    {
        public string WorkflowType { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public Guid InitiatedBy { get; set; }
        public string? Priority { get; set; }
        public Dictionary<string, object>? InputData { get; set; }
    }

    public class ExecuteStepRequest
    {
        public string Action { get; set; } = string.Empty; // Approve, Reject
        public Guid CompletedBy { get; set; }
        public string? Comments { get; set; }
        public Dictionary<string, object>? OutputData { get; set; }
    }

    public class CreateWorkflowTemplateRequest
    {
        public string WorkflowType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WorkflowStepDefinition> Steps { get; set; } = new();
    }

    public class WorkflowStepDefinition
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }
        public int? DueDays { get; set; }
        public bool IsAutomated { get; set; }
        public Dictionary<string, object>? Configuration { get; set; }
    }

