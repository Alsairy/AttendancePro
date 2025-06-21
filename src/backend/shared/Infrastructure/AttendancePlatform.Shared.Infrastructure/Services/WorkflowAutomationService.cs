using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AttendancePlatform.Shared.Infrastructure.Services;

public class WorkflowAutomationService : IWorkflowAutomationService
{
    private readonly AttendancePlatformDbContext _context;
    private readonly ILogger<WorkflowAutomationService> _logger;
    private readonly INotificationService _notificationService;

    public WorkflowAutomationService(
        AttendancePlatformDbContext context,
        ILogger<WorkflowAutomationService> logger,
        INotificationService notificationService)
    {
        _context = context;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<WorkflowDefinition> CreateWorkflowAsync(CreateWorkflowRequest request)
    {
        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TenantId = Guid.Parse(request.TenantId),
            Category = request.Category,
            Steps = JsonSerializer.Serialize(request.Steps),
            Triggers = JsonSerializer.Serialize(request.Triggers),
            Variables = JsonSerializer.Serialize(request.Variables),
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };

        _context.WorkflowDefinitions.Add(workflow);
        await _context.SaveChangesAsync();
        return workflow;
    }

    public async Task<WorkflowDefinition> UpdateWorkflowAsync(string workflowId, UpdateWorkflowRequest request)
    {
        var workflow = await _context.WorkflowDefinitions.FirstOrDefaultAsync(w => w.Id == Guid.Parse(workflowId));
        if (workflow == null)
            throw new ArgumentException($"Workflow {workflowId} not found");

        if (!string.IsNullOrEmpty(request.Name))
            workflow.Name = request.Name;
        if (!string.IsNullOrEmpty(request.Description))
            workflow.Description = request.Description;
        if (!string.IsNullOrEmpty(request.Category))
            workflow.Category = request.Category;
        if (request.Steps != null)
            workflow.Steps = JsonSerializer.Serialize(request.Steps);
        if (request.Triggers != null)
            workflow.Triggers = JsonSerializer.Serialize(request.Triggers);
        if (request.Variables != null)
            workflow.Variables = JsonSerializer.Serialize(request.Variables);
        if (request.IsActive.HasValue)
            workflow.IsActive = request.IsActive.Value;

        workflow.UpdatedAt = DateTime.UtcNow;
        workflow.UpdatedBy = request.UpdatedBy;

        await _context.SaveChangesAsync();
        return workflow;
    }

    public async Task DeleteWorkflowAsync(string workflowId)
    {
        var workflow = await _context.WorkflowDefinitions.FirstOrDefaultAsync(w => w.Id == Guid.Parse(workflowId));
        if (workflow != null)
        {
            _context.WorkflowDefinitions.Remove(workflow);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<WorkflowDefinition?> GetWorkflowAsync(string workflowId)
    {
        return await _context.WorkflowDefinitions.FirstOrDefaultAsync(w => w.Id == Guid.Parse(workflowId));
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetWorkflowsAsync(string tenantId, bool activeOnly = true)
    {
        var query = _context.WorkflowDefinitions.Where(w => w.TenantId == Guid.Parse(tenantId));
        if (activeOnly)
            query = query.Where(w => w.IsActive);
        return await query.ToListAsync();
    }

    public async Task<WorkflowInstance> StartWorkflowAsync(string workflowId, object? inputData = null, string? userId = null)
    {
        var workflow = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(w => w.Id == Guid.Parse(workflowId) && w.IsActive);

        if (workflow == null)
            throw new ArgumentException($"Workflow {workflowId} not found or inactive");

        var instance = new WorkflowInstance
        {
            Id = Guid.NewGuid(),
            WorkflowDefinitionId = workflowId,
            Status = "running",
            InputData = inputData != null ? JsonSerializer.Serialize(inputData) : "{}",
            Variables = workflow.Variables,
            StartedAt = DateTime.UtcNow,
            StartedBy = userId,
            CorrelationId = Guid.NewGuid().ToString()
        };

        var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(workflow.Steps) ?? new List<WorkflowStep>();
        if (steps.Any())
        {
            instance.CurrentStepId = steps.OrderBy(s => s.Order).First().Id;
        }

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();

        await LogExecutionAsync(instance.Id.ToString(), "workflow_started", "running", "Workflow started", inputData, userId);

        if (!string.IsNullOrEmpty(instance.CurrentStepId))
        {
            await ExecuteNextStepAsync(instance.Id.ToString());
        }

        return instance;
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId)
    {
        return await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .Include(i => i.Tasks)
            .Include(i => i.ExecutionLogs)
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(instanceId));
    }

    public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(string workflowId, string? status = null, int page = 1, int pageSize = 50)
    {
        var query = _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .Where(i => i.WorkflowDefinitionId == workflowId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(i => i.Status == status);

        return await query
            .OrderByDescending(i => i.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task CompleteTaskAsync(string instanceId, string taskId, object? outputData = null, string? userId = null)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .FirstOrDefaultAsync(t => t.WorkflowInstanceId == instanceId && t.StepId == taskId);

        if (task == null)
            throw new ArgumentException($"Task {taskId} not found in instance {instanceId}");

        if (task.Status != "pending")
            throw new InvalidOperationException($"Task {taskId} is not in pending status");

        task.Status = "completed";
        task.CompletedAt = DateTime.UtcNow;
        task.CompletedBy = userId;
        task.OutputData = outputData != null ? JsonSerializer.Serialize(outputData) : "{}";

        await _context.SaveChangesAsync();

        await LogExecutionAsync(instanceId, taskId, "completed", "Task completed", outputData, userId);

        await ExecuteNextStepAsync(instanceId);
    }

    public async Task CancelWorkflowInstanceAsync(string instanceId, string reason, string? userId = null)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(instanceId));

        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

        if (instance.Status == "completed" || instance.Status == "cancelled")
            return;

        instance.Status = "cancelled";
        instance.CompletedAt = DateTime.UtcNow;
        instance.CompletedBy = userId;
        instance.ErrorMessage = reason;

        var pendingTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId && t.Status == "pending")
            .ToListAsync();

        foreach (var task in pendingTasks)
        {
            task.Status = "cancelled";
            task.CompletedAt = DateTime.UtcNow;
            task.CompletedBy = userId;
        }

        await _context.SaveChangesAsync();

        await LogExecutionAsync(instanceId, "workflow_cancelled", "cancelled", reason ?? "Workflow cancelled", null, userId);
    }

    public async Task<bool> RetryWorkflowAsync(string instanceId, string? userId = null)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(instanceId));

        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

        if (instance.Status != "failed")
            throw new InvalidOperationException("Only failed workflows can be retried");

        instance.Status = "running";
        instance.RetryCount++;
        instance.ErrorMessage = null;

        var failedTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId && t.Status == "failed")
            .ToListAsync();

        foreach (var task in failedTasks)
        {
            task.Status = "pending";
            task.RetryCount++;
            task.ErrorMessage = null;
        }

        await _context.SaveChangesAsync();

        await LogExecutionAsync(instanceId, "workflow_retried", "running", "Workflow retried", null, userId);

        await ExecuteNextStepAsync(instanceId);

        return true;
    }

    public async Task<IEnumerable<WorkflowTask>> GetWorkflowTasksAsync(string instanceId)
    {
        return await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowTask>> GetPendingTasksAsync(string userId)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t => t.Status == "pending" && t.AssignedTo == userId)
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AssignTaskAsync(string taskId, string userId, string? assignedBy = null)
    {
        var task = await _context.WorkflowTasks.FirstOrDefaultAsync(t => t.Id.ToString() == taskId);
        if (task == null)
            throw new ArgumentException($"Task {taskId} not found");

        task.AssignedTo = userId;
        task.AssignedBy = assignedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await LogExecutionAsync(task.WorkflowInstanceId, taskId, "assigned", $"Task assigned to {userId}", null, assignedBy);
    }

    public async Task ReassignTaskAsync(string taskId, string fromUserId, string toUserId, string? reassignedBy = null)
    {
        var task = await _context.WorkflowTasks.FirstOrDefaultAsync(t => t.Id.ToString() == taskId);
        if (task == null)
            throw new ArgumentException($"Task {taskId} not found");

        if (task.AssignedTo != fromUserId)
            throw new InvalidOperationException($"Task {taskId} is not assigned to {fromUserId}");

        task.AssignedTo = toUserId;
        task.AssignedBy = reassignedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await LogExecutionAsync(task.WorkflowInstanceId, taskId, "reassigned", $"Task reassigned from {fromUserId} to {toUserId}", null, reassignedBy);
    }

    public async Task<WorkflowExecutionResult> ExecuteWorkflowStepAsync(string instanceId, string stepId, object? inputData = null)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(instanceId));

        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

        var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(instance.WorkflowDefinition.Steps) ?? new List<WorkflowStep>();
        var step = steps.FirstOrDefault(s => s.Id == stepId);

        if (step == null)
            throw new ArgumentException($"Step {stepId} not found in workflow");

        return await ExecuteStepAsync(instance, step);
    }

    public async Task<IEnumerable<WorkflowTemplate>> GetWorkflowTemplatesAsync()
    {
        return await _context.WorkflowTemplates.ToListAsync();
    }

    public async Task<WorkflowDefinition> CreateWorkflowFromTemplateAsync(string templateId, string tenantId, Dictionary<string, object>? parameters = null)
    {
        var template = await _context.WorkflowTemplates.FirstOrDefaultAsync(t => t.Id == Guid.Parse(templateId));
        if (template == null)
            throw new ArgumentException($"Template {templateId} not found");

        var workflow = new WorkflowDefinition
        {
            Id = Guid.NewGuid(),
            Name = template.Name,
            Description = template.Description,
            TenantId = Guid.Parse(tenantId),
            Category = template.Category,
            Steps = template.Steps,
            Triggers = template.Triggers,
            Variables = parameters != null ? JsonSerializer.Serialize(parameters) : template.Variables,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.WorkflowDefinitions.Add(workflow);
        await _context.SaveChangesAsync();
        return workflow;
    }

    private async Task ExecuteNextStepAsync(string instanceId)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == Guid.Parse(instanceId));

        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

        if (instance.Status != "running")
            return;

        var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(instance.WorkflowDefinition.Steps) ?? new List<WorkflowStep>();
        var currentStep = steps.FirstOrDefault(s => s.Id == instance.CurrentStepId);

        if (currentStep == null)
        {
            instance.Status = "completed";
            instance.CompletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await LogExecutionAsync(instanceId, "workflow_completed", "completed", "Workflow completed", null, null);
            return;
        }

        try
        {
            var result = await ExecuteStepAsync(instance, currentStep);

            if (result.IsSuccess)
            {
                var nextStepId = result.NextStepId;
                if (!string.IsNullOrEmpty(nextStepId))
                {
                    instance.CurrentStepId = nextStepId;
                    await _context.SaveChangesAsync();
                    await ExecuteNextStepAsync(instanceId);
                }
                else
                {
                    instance.Status = "completed";
                    instance.CompletedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    await LogExecutionAsync(instanceId, "workflow_completed", "completed", "Workflow completed", null, null);
                }
            }
            else
            {
                if (instance.RetryCount < instance.MaxRetries)
                {
                    instance.RetryCount++;
                    await _context.SaveChangesAsync();
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, instance.RetryCount)));
                    await ExecuteNextStepAsync(instanceId);
                }
                else
                {
                    instance.Status = "failed";
                    instance.CompletedAt = DateTime.UtcNow;
                    instance.ErrorMessage = result.ErrorMessage;
                    await _context.SaveChangesAsync();
                    await LogExecutionAsync(instanceId, currentStep.Id, "failed", result.ErrorMessage ?? "Step execution failed", null, null);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing step {StepId} in workflow instance {InstanceId}", currentStep.Id, instanceId);
            
            instance.Status = "failed";
            instance.CompletedAt = DateTime.UtcNow;
            instance.ErrorMessage = ex.Message;
            await _context.SaveChangesAsync();
            await LogExecutionAsync(instanceId, currentStep.Id, "failed", ex.Message, null, null);
        }
    }

    private async Task<WorkflowExecutionResult> ExecuteStepAsync(WorkflowInstance instance, WorkflowStep step)
    {
        var variables = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Variables) ?? new Dictionary<string, object>();
        var inputData = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.InputData) ?? new Dictionary<string, object>();

        return step.Type.ToLower() switch
        {
            "approval" => await ExecuteApprovalStepAsync(instance, step, inputData),
            "notification" => await ExecuteNotificationStepAsync(instance, step, inputData),
            "condition" => await ExecuteConditionStepAsync(instance, step, inputData),
            "delay" => await ExecuteDelayStepAsync(instance, step, inputData),
            "webhook" => await ExecuteWebhookStepAsync(instance, step, inputData),
            _ => new WorkflowExecutionResult { IsSuccess = false, ErrorMessage = $"Unknown step type: {step.Type}" }
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteApprovalStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var task = new WorkflowTask
        {
            Id = Guid.NewGuid(),
            WorkflowInstanceId = instance.Id.ToString(),
            StepId = step.Id,
            Name = step.Name,
            Description = step.Properties.GetValueOrDefault("description", "")?.ToString() ?? "",
            Type = "approval",
            Status = "pending",
            InputData = inputData != null ? JsonSerializer.Serialize(inputData) : "{}",
            Priority = step.Properties.GetValueOrDefault("priority", "normal")?.ToString() ?? "normal"
        };

        if (step.Properties.ContainsKey("assignedTo"))
        {
            task.AssignedTo = step.Properties["assignedTo"]?.ToString();
            task.AssignedAt = DateTime.UtcNow;
        }

        if (step.Properties.ContainsKey("dueDate"))
        {
            if (DateTime.TryParse(step.Properties["dueDate"]?.ToString(), out var dueDate))
            {
                task.DueDate = dueDate;
            }
        }

        _context.WorkflowTasks.Add(task);
        await _context.SaveChangesAsync();

        await LogExecutionAsync(instance.Id.ToString(), step.Id, "pending", "Approval task created", inputData, null);

        return new WorkflowExecutionResult { IsSuccess = true, RequiresManualAction = true };
    }

    private async Task<WorkflowExecutionResult> ExecuteNotificationStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var message = step.Properties.GetValueOrDefault("message", "")?.ToString() ?? "";
        var recipients = step.Properties.GetValueOrDefault("recipients", "")?.ToString() ?? "";
        var notificationType = step.Properties.GetValueOrDefault("type", "info")?.ToString() ?? "info";

        if (!string.IsNullOrEmpty(recipients))
        {
            var recipientList = recipients.Split(',').Select(r => r.Trim()).ToList();
            foreach (var recipient in recipientList)
            {
                await _notificationService.SendNotificationAsync(new NotificationRequest
                {
                    UserId = recipient,
                    Title = "Workflow Notification", 
                    Message = message,
                    Type = notificationType
                });
            }
        }

        await LogExecutionAsync(instance.Id.ToString(), step.Id, "completed", "Notification sent", inputData, null);

        var nextStepId = step.NextSteps.FirstOrDefault();
        return new WorkflowExecutionResult { IsSuccess = true, NextStepId = nextStepId };
    }

    private async Task<WorkflowExecutionResult> ExecuteConditionStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var variables = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Variables) ?? new Dictionary<string, object>();
        var conditions = step.Conditions;
        
        bool conditionMet = EvaluateConditions(conditions, variables, inputData);
        
        var nextStepId = conditionMet 
            ? step.Properties.GetValueOrDefault("trueStep", "")?.ToString() ?? ""
            : step.Properties.GetValueOrDefault("falseStep", "")?.ToString() ?? "";

        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = nextStepId
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteDelayStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var delaySeconds = 0;
        if (step.Properties.ContainsKey("delaySeconds"))
        {
            int.TryParse(step.Properties["delaySeconds"]?.ToString(), out delaySeconds);
        }

        if (delaySeconds > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }

        await LogExecutionAsync(instance.Id.ToString(), step.Id, "completed", $"Delayed for {delaySeconds} seconds", inputData, null);

        var nextStepId = step.NextSteps.FirstOrDefault();
        return new WorkflowExecutionResult { IsSuccess = true, NextStepId = nextStepId };
    }

    private async Task<WorkflowExecutionResult> ExecuteWebhookStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        return new WorkflowExecutionResult { IsSuccess = true, NextStepId = step.NextSteps.FirstOrDefault() };
    }

    private bool EvaluateConditions(List<WorkflowCondition> conditions, Dictionary<string, object> variables, object? inputData)
    {
        return true;
    }

    private async Task LogExecutionAsync(string instanceId, string stepId, string status, string? message, object? data, string? userId)
    {
        var log = new WorkflowExecutionLog
        {
            Id = Guid.NewGuid(),
            WorkflowInstanceId = instanceId,
            StepId = stepId,
            Action = "execute",
            Status = status,
            Message = message,
            Data = data != null ? JsonSerializer.Serialize(data) : null,
            UserId = userId,
            ExecutedAt = DateTime.UtcNow
        };

        _context.WorkflowExecutionLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
