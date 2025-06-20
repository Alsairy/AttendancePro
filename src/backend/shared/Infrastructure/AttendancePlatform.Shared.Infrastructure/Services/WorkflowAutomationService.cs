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
            Name = request.Name,
            Description = request.Description,
            TenantId = request.TenantId,
            Category = request.Category,
            Steps = JsonSerializer.Serialize(request.Steps),
            Triggers = JsonSerializer.Serialize(request.Triggers),
            Variables = JsonSerializer.Serialize(request.Variables),
            IsActive = request.IsActive,
            CreatedBy = request.CreatedBy,
            Version = 1
        };

        _context.WorkflowDefinitions.Add(workflow);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created workflow {WorkflowId} for tenant {TenantId}", workflow.Id, request.TenantId);
        return workflow;
    }

    public async Task<WorkflowDefinition> UpdateWorkflowAsync(string workflowId, UpdateWorkflowRequest request)
    {
        var workflow = await _context.WorkflowDefinitions.FindAsync(workflowId);
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

        workflow.UpdatedBy = request.UpdatedBy;
        workflow.UpdatedAt = DateTime.UtcNow;
        workflow.Version++;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated workflow {WorkflowId}", workflowId);
        return workflow;
    }

    public async Task DeleteWorkflowAsync(string workflowId)
    {
        var workflow = await _context.WorkflowDefinitions.FindAsync(workflowId);
        if (workflow != null)
        {
            _context.WorkflowDefinitions.Remove(workflow);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted workflow {WorkflowId}", workflowId);
        }
    }

    public async Task<WorkflowDefinition?> GetWorkflowAsync(string workflowId)
    {
        return await _context.WorkflowDefinitions.FindAsync(workflowId);
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetWorkflowsAsync(string tenantId, bool activeOnly = true)
    {
        var query = _context.WorkflowDefinitions.Where(w => w.TenantId == tenantId);
        
        if (activeOnly)
            query = query.Where(w => w.IsActive);

        return await query.OrderBy(w => w.Name).ToListAsync();
    }

    public async Task<WorkflowInstance> StartWorkflowAsync(string workflowId, object? inputData = null, string? userId = null)
    {
        var workflow = await _context.WorkflowDefinitions.FindAsync(workflowId);
        if (workflow == null)
            throw new ArgumentException($"Workflow {workflowId} not found");

        if (!workflow.IsActive)
            throw new InvalidOperationException($"Workflow {workflowId} is not active");

        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = workflowId,
            TenantId = workflow.TenantId,
            Status = "running",
            InputData = inputData != null ? JsonSerializer.Serialize(inputData) : "{}",
            Variables = workflow.Variables,
            StartedAt = DateTime.UtcNow,
            StartedBy = userId,
            CorrelationId = Guid.NewGuid().ToString()
        };

        var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(workflow.Steps) ?? new List<WorkflowStep>();
        var firstStep = steps.OrderBy(s => s.Order).FirstOrDefault();
        if (firstStep != null)
        {
            instance.CurrentStepId = firstStep.Id;
        }

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync();

        await LogWorkflowEventAsync(instance.Id, "WORKFLOW_STARTED", "Workflow instance started", userId);

        if (firstStep != null)
        {
            await ExecuteWorkflowStepAsync(instance.Id, firstStep.Id, inputData);
        }

        _logger.LogInformation("Started workflow instance {InstanceId} for workflow {WorkflowId}", instance.Id, workflowId);
        return instance;
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId)
    {
        return await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .Include(i => i.Tasks)
            .Include(i => i.ExecutionLogs)
            .FirstOrDefaultAsync(i => i.Id == instanceId);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(string workflowId, string? status = null, int page = 1, int pageSize = 50)
    {
        var query = _context.WorkflowInstances.Where(i => i.WorkflowDefinitionId == workflowId);

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

        if (task.Status == "completed")
            throw new InvalidOperationException($"Task {taskId} is already completed");

        task.Status = "completed";
        task.CompletedAt = DateTime.UtcNow;
        task.CompletedBy = userId;
        task.OutputData = outputData != null ? JsonSerializer.Serialize(outputData) : "{}";

        await _context.SaveChangesAsync();

        await LogWorkflowEventAsync(instanceId, "TASK_COMPLETED", $"Task {taskId} completed", userId);

        var workflow = await _context.WorkflowDefinitions.FindAsync(task.WorkflowInstance.WorkflowDefinitionId);
        if (workflow != null)
        {
            var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(workflow.Steps) ?? new List<WorkflowStep>();
            var currentStep = steps.FirstOrDefault(s => s.Id == taskId);
            
            if (currentStep?.NextSteps.Any() == true)
            {
                foreach (var nextStepId in currentStep.NextSteps)
                {
                    await ExecuteWorkflowStepAsync(instanceId, nextStepId, outputData);
                }
            }
            else
            {
                await CompleteWorkflowInstanceAsync(instanceId, userId);
            }
        }

        _logger.LogInformation("Completed task {TaskId} in workflow instance {InstanceId}", taskId, instanceId);
    }

    public async Task CancelWorkflowInstanceAsync(string instanceId, string reason, string? userId = null)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

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
            task.ErrorMessage = "Workflow cancelled";
        }

        await _context.SaveChangesAsync();

        await LogWorkflowEventAsync(instanceId, "WORKFLOW_CANCELLED", $"Workflow cancelled: {reason}", userId);

        _logger.LogInformation("Cancelled workflow instance {InstanceId}: {Reason}", instanceId, reason);
    }

    public async Task<IEnumerable<WorkflowTask>> GetPendingTasksAsync(string userId)
    {
        return await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .ThenInclude(i => i.WorkflowDefinition)
            .Where(t => t.AssignedTo == userId && t.Status == "pending")
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowTask>> GetWorkflowTasksAsync(string instanceId)
    {
        return await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task AssignTaskAsync(string taskId, string userId, string? assignedBy = null)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null)
            throw new ArgumentException($"Task {taskId} not found");

        task.AssignedTo = userId;
        task.AssignedBy = assignedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(new NotificationRequest
        {
            UserId = userId,
            Title = "New Task Assigned",
            Message = $"You have been assigned a new task: {task.Name}",
            Type = "task_assignment",
            ActionUrl = $"/workflows/tasks/{taskId}",
            Channels = new List<string> { "realtime", "email" }
        });

        _logger.LogInformation("Assigned task {TaskId} to user {UserId}", taskId, userId);
    }

    public async Task ReassignTaskAsync(string taskId, string fromUserId, string toUserId, string? reassignedBy = null)
    {
        var task = await _context.WorkflowTasks.FindAsync(taskId);
        if (task == null)
            throw new ArgumentException($"Task {taskId} not found");

        if (task.AssignedTo != fromUserId)
            throw new InvalidOperationException($"Task {taskId} is not assigned to user {fromUserId}");

        task.AssignedTo = toUserId;
        task.AssignedBy = reassignedBy;
        task.AssignedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _notificationService.SendNotificationAsync(new NotificationRequest
        {
            UserId = toUserId,
            Title = "Task Reassigned",
            Message = $"A task has been reassigned to you: {task.Name}",
            Type = "task_reassignment",
            ActionUrl = $"/workflows/tasks/{taskId}",
            Channels = new List<string> { "realtime", "email" }
        });

        _logger.LogInformation("Reassigned task {TaskId} from {FromUserId} to {ToUserId}", taskId, fromUserId, toUserId);
    }

    public async Task<WorkflowExecutionResult> ExecuteWorkflowStepAsync(string instanceId, string stepId, object? inputData = null)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId);

        if (instance == null)
            throw new ArgumentException($"Workflow instance {instanceId} not found");

        var steps = JsonSerializer.Deserialize<List<WorkflowStep>>(instance.WorkflowDefinition.Steps) ?? new List<WorkflowStep>();
        var step = steps.FirstOrDefault(s => s.Id == stepId);

        if (step == null)
            throw new ArgumentException($"Step {stepId} not found in workflow");

        var result = new WorkflowExecutionResult();

        try
        {
            await LogWorkflowEventAsync(instanceId, "STEP_STARTED", $"Started executing step {stepId}", null);

            switch (step.Type.ToLower())
            {
                case "approval":
                    result = await ExecuteApprovalStepAsync(instance, step, inputData);
                    break;
                case "notification":
                    result = await ExecuteNotificationStepAsync(instance, step, inputData);
                    break;
                case "condition":
                    result = await ExecuteConditionStepAsync(instance, step, inputData);
                    break;
                case "delay":
                    result = await ExecuteDelayStepAsync(instance, step, inputData);
                    break;
                default:
                    result = await ExecuteCustomStepAsync(instance, step, inputData);
                    break;
            }

            if (result.IsSuccess)
            {
                instance.CurrentStepId = result.NextStepId;
                await _context.SaveChangesAsync();

                await LogWorkflowEventAsync(instanceId, "STEP_COMPLETED", $"Completed step {stepId}", null);

                if (result.IsCompleted)
                {
                    await CompleteWorkflowInstanceAsync(instanceId, null);
                }
            }
            else
            {
                await LogWorkflowEventAsync(instanceId, "STEP_FAILED", $"Step {stepId} failed: {result.ErrorMessage}", null);
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;
            
            await LogWorkflowEventAsync(instanceId, "STEP_ERROR", $"Step {stepId} error: {ex.Message}", null);
            _logger.LogError(ex, "Error executing workflow step {StepId} in instance {InstanceId}", stepId, instanceId);
        }

        return result;
    }

    public async Task<IEnumerable<WorkflowTemplate>> GetWorkflowTemplatesAsync()
    {
        return await _context.WorkflowTemplates
            .Where(t => t.IsPublic)
            .OrderBy(t => t.Category)
            .ThenBy(t => t.Name)
            .ToListAsync();
    }

    public async Task<WorkflowDefinition> CreateWorkflowFromTemplateAsync(string templateId, string tenantId, Dictionary<string, object>? parameters = null)
    {
        var template = await _context.WorkflowTemplates.FindAsync(templateId);
        if (template == null)
            throw new ArgumentException($"Workflow template {templateId} not found");

        var definition = JsonSerializer.Deserialize<CreateWorkflowRequest>(template.Definition);
        if (definition == null)
            throw new InvalidOperationException($"Invalid template definition for {templateId}");

        definition.TenantId = tenantId;

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                definition.Variables[param.Key] = param.Value;
            }
        }

        var workflow = await CreateWorkflowAsync(definition);

        template.UsageCount++;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created workflow {WorkflowId} from template {TemplateId}", workflow.Id, templateId);
        return workflow;
    }

    private async Task<WorkflowExecutionResult> ExecuteApprovalStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var task = new WorkflowTask
        {
            WorkflowInstanceId = instance.Id,
            StepId = step.Id,
            Name = step.Name,
            Description = step.Properties.GetValueOrDefault("description", "").ToString() ?? "",
            Type = "approval",
            Status = "pending",
            InputData = inputData != null ? JsonSerializer.Serialize(inputData) : "{}",
            Priority = step.Properties.GetValueOrDefault("priority", "normal").ToString() ?? "normal"
        };

        if (step.Properties.ContainsKey("assignedTo"))
        {
            task.AssignedTo = step.Properties["assignedTo"].ToString();
            task.AssignedAt = DateTime.UtcNow;
        }

        if (step.Properties.ContainsKey("dueDate"))
        {
            if (DateTime.TryParse(step.Properties["dueDate"].ToString(), out var dueDate))
            {
                task.DueDate = dueDate;
            }
        }

        _context.WorkflowTasks.Add(task);
        await _context.SaveChangesAsync();

        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = step.Id,
            IsCompleted = false
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteNotificationStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var message = step.Properties.GetValueOrDefault("message", "").ToString() ?? "";
        var recipients = step.Properties.GetValueOrDefault("recipients", "").ToString() ?? "";
        var notificationType = step.Properties.GetValueOrDefault("type", "info").ToString() ?? "info";

        if (!string.IsNullOrEmpty(recipients))
        {
            var recipientList = recipients.Split(',').Select(r => r.Trim()).ToList();
            
            foreach (var recipient in recipientList)
            {
                await _notificationService.SendNotificationAsync(new NotificationRequest
                {
                    UserId = recipient,
                    Title = $"Workflow Notification: {instance.WorkflowDefinition.Name}",
                    Message = message,
                    Type = notificationType,
                    Data = inputData,
                    Channels = new List<string> { "realtime", "email" }
                });
            }
        }

        var nextStepId = step.NextSteps.FirstOrDefault() ?? "";
        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = nextStepId,
            IsCompleted = string.IsNullOrEmpty(nextStepId)
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteConditionStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var conditions = step.Conditions;
        var variables = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Variables) ?? new Dictionary<string, object>();
        
        bool conditionMet = EvaluateConditions(conditions, variables, inputData);
        
        var nextStepId = conditionMet 
            ? step.Properties.GetValueOrDefault("trueStep", "").ToString() ?? ""
            : step.Properties.GetValueOrDefault("falseStep", "").ToString() ?? "";

        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = nextStepId,
            IsCompleted = string.IsNullOrEmpty(nextStepId)
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteDelayStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        var delayMinutes = int.Parse(step.Properties.GetValueOrDefault("delayMinutes", "0").ToString() ?? "0");
        
        if (delayMinutes > 0)
        {
            await Task.Delay(TimeSpan.FromMinutes(delayMinutes));
        }

        var nextStepId = step.NextSteps.FirstOrDefault() ?? "";
        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = nextStepId,
            IsCompleted = string.IsNullOrEmpty(nextStepId)
        };
    }

    private async Task<WorkflowExecutionResult> ExecuteCustomStepAsync(WorkflowInstance instance, WorkflowStep step, object? inputData)
    {
        _logger.LogWarning("Custom step execution not implemented for step type {StepType}", step.Type);
        
        var nextStepId = step.NextSteps.FirstOrDefault() ?? "";
        return new WorkflowExecutionResult
        {
            IsSuccess = true,
            NextStepId = nextStepId,
            IsCompleted = string.IsNullOrEmpty(nextStepId)
        };
    }

    private bool EvaluateConditions(List<WorkflowCondition> conditions, Dictionary<string, object> variables, object? inputData)
    {
        if (!conditions.Any()) return true;

        foreach (var condition in conditions)
        {
            var fieldValue = GetFieldValue(condition.Field, variables, inputData);
            var conditionValue = condition.Value;

            bool result = condition.Operator.ToLower() switch
            {
                "equals" => Equals(fieldValue, conditionValue),
                "notequals" => !Equals(fieldValue, conditionValue),
                "greaterthan" => CompareValues(fieldValue, conditionValue) > 0,
                "lessthan" => CompareValues(fieldValue, conditionValue) < 0,
                "contains" => fieldValue?.ToString()?.Contains(conditionValue?.ToString() ?? "") == true,
                _ => false
            };

            if (condition.LogicalOperator.ToUpper() == "OR" && result)
                return true;
            if (condition.LogicalOperator.ToUpper() == "AND" && !result)
                return false;
        }

        return true;
    }

    private object? GetFieldValue(string field, Dictionary<string, object> variables, object? inputData)
    {
        if (variables.ContainsKey(field))
            return variables[field];

        if (inputData != null)
        {
            var inputDict = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(inputData));
            if (inputDict?.ContainsKey(field) == true)
                return inputDict[field];
        }

        return null;
    }

    private int CompareValues(object? value1, object? value2)
    {
        if (value1 == null && value2 == null) return 0;
        if (value1 == null) return -1;
        if (value2 == null) return 1;

        if (double.TryParse(value1.ToString(), out var num1) && double.TryParse(value2.ToString(), out var num2))
        {
            return num1.CompareTo(num2);
        }

        return string.Compare(value1.ToString(), value2.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private async Task CompleteWorkflowInstanceAsync(string instanceId, string? userId)
    {
        var instance = await _context.WorkflowInstances.FindAsync(instanceId);
        if (instance != null)
        {
            instance.Status = "completed";
            instance.CompletedAt = DateTime.UtcNow;
            instance.CompletedBy = userId;

            await _context.SaveChangesAsync();

            await LogWorkflowEventAsync(instanceId, "WORKFLOW_COMPLETED", "Workflow instance completed", userId);
            _logger.LogInformation("Completed workflow instance {InstanceId}", instanceId);
        }
    }

    private async Task LogWorkflowEventAsync(string instanceId, string action, string message, string? userId)
    {
        var log = new WorkflowExecutionLog
        {
            WorkflowInstanceId = instanceId,
            Action = action,
            Status = "success",
            Message = message,
            UserId = userId,
            ExecutedAt = DateTime.UtcNow
        };

        _context.WorkflowExecutionLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
