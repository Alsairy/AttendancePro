using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Text.Json;
using System.Linq.Dynamic.Core;

namespace AttendancePlatform.Workflow.Api.Services
{
    // Core Workflow Interfaces
    public interface IWorkflowEngineService
    {
        Task<WorkflowInstanceDto> StartWorkflowAsync(StartWorkflowRequestDto request);
        Task<WorkflowInstanceDto> GetWorkflowInstanceAsync(Guid instanceId);
        Task<List<WorkflowInstanceDto>> GetActiveWorkflowsAsync(Guid tenantId);
        Task<WorkflowInstanceDto> ExecuteWorkflowStepAsync(Guid instanceId, ExecuteStepRequestDto request);
        Task<bool> CancelWorkflowAsync(Guid instanceId, string reason);
        Task<List<WorkflowHistoryDto>> GetWorkflowHistoryAsync(Guid instanceId);
    }

    public interface IBusinessRulesService
    {
        Task<BusinessRuleDto> CreateRuleAsync(CreateBusinessRuleRequestDto request);
        Task<List<BusinessRuleDto>> GetRulesAsync(Guid tenantId, string category);
        Task<BusinessRuleEvaluationResultDto> EvaluateRuleAsync(Guid ruleId, Dictionary<string, object> context);
        Task<List<BusinessRuleEvaluationResultDto>> EvaluateRulesAsync(Guid tenantId, string category, Dictionary<string, object> context);
        Task<BusinessRuleDto> UpdateRuleAsync(Guid ruleId, UpdateBusinessRuleRequestDto request);
        Task<bool> DeleteRuleAsync(Guid ruleId);
        Task<List<BusinessRuleTemplateDto>> GetRuleTemplatesAsync();
    }

    public interface IApprovalWorkflowService
    {
        Task<ApprovalWorkflowDto> CreateApprovalWorkflowAsync(CreateApprovalWorkflowRequestDto request);
        Task<List<ApprovalWorkflowDto>> GetApprovalWorkflowsAsync(Guid tenantId);
        Task<ApprovalRequestDto> SubmitForApprovalAsync(SubmitApprovalRequestDto request);
        Task<ApprovalRequestDto> ProcessApprovalAsync(Guid approvalId, ProcessApprovalRequestDto request);
        Task<List<ApprovalRequestDto>> GetPendingApprovalsAsync(Guid tenantId, Guid? approverId = null);
        Task<List<ApprovalRequestDto>> GetApprovalHistoryAsync(Guid tenantId, Guid? requesterId = null);
        Task<ApprovalWorkflowDto> UpdateApprovalWorkflowAsync(Guid workflowId, UpdateApprovalWorkflowRequestDto request);
    }

    public interface IAutomationService
    {
        Task<AutomationRuleDto> CreateAutomationRuleAsync(CreateAutomationRuleRequestDto request);
        Task<List<AutomationRuleDto>> GetAutomationRulesAsync(Guid tenantId);
        Task<AutomationExecutionResultDto> ExecuteAutomationAsync(Guid ruleId, Dictionary<string, object> context);
        Task<List<AutomationExecutionLogDto>> GetExecutionLogsAsync(Guid tenantId, Guid? ruleId = null);
        Task<AutomationRuleDto> UpdateAutomationRuleAsync(Guid ruleId, UpdateAutomationRuleRequestDto request);
        Task<bool> EnableDisableAutomationAsync(Guid ruleId, bool enabled);
        Task<List<AutomationTemplateDto>> GetAutomationTemplatesAsync();
    }

    public interface IWorkflowTemplateService
    {
        Task<WorkflowTemplateDto> CreateTemplateAsync(CreateWorkflowTemplateRequestDto request);
        Task<List<WorkflowTemplateDto>> GetTemplatesAsync(Guid tenantId, string category);
        Task<WorkflowTemplateDto> GetTemplateAsync(Guid templateId);
        Task<WorkflowTemplateDto> UpdateTemplateAsync(Guid templateId, UpdateWorkflowTemplateRequestDto request);
        Task<bool> DeleteTemplateAsync(Guid templateId);
        Task<List<WorkflowTemplateCategoryDto>> GetTemplateCategoriesAsync();
    }

    public interface IWorkflowExecutionService
    {
        Task<WorkflowExecutionResultDto> ExecuteWorkflowAsync(Guid workflowId, Dictionary<string, object> context);
        Task<List<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid tenantId, Guid? workflowId = null);
        Task<WorkflowPerformanceDto> GetWorkflowPerformanceAsync(Guid workflowId);
        Task<List<WorkflowMetricsDto>> GetWorkflowMetricsAsync(Guid tenantId);
    }

    // Core Workflow Engine Implementation
    public class WorkflowEngineService : IWorkflowEngineService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<WorkflowEngineService> _logger;

        public WorkflowEngineService(AttendancePlatformDbContext context, ILogger<WorkflowEngineService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WorkflowInstanceDto> StartWorkflowAsync(StartWorkflowRequestDto request)
        {
            try
            {
                var workflowTemplate = await _context.WorkflowTemplates.FindAsync(request.WorkflowTemplateId);
                if (workflowTemplate == null)
                    throw new ArgumentException("Workflow template not found");

                var instance = new WorkflowInstance
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    WorkflowTemplateId = request.WorkflowTemplateId,
                    InitiatedBy = request.InitiatedBy,
                    Status = "Running",
                    CurrentStep = 0,
                    Context = JsonSerializer.Serialize(request.Context),
                    StartedAt = DateTime.UtcNow,
                    LastUpdatedAt = DateTime.UtcNow
                };

                _context.WorkflowInstances.Add(instance);
                await _context.SaveChangesAsync();

                // Log workflow start
                await LogWorkflowEvent(instance.Id, "WorkflowStarted", "Workflow instance started", request.Context);

                // Execute first step
                await ExecuteNextStep(instance);

                return await MapToWorkflowInstanceDto(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting workflow");
                throw;
            }
        }

        public async Task<WorkflowInstanceDto> GetWorkflowInstanceAsync(Guid instanceId)
        {
            try
            {
                var instance = await _context.WorkflowInstances
                    .Include(wi => wi.WorkflowTemplate)
                    .FirstOrDefaultAsync(wi => wi.Id == instanceId);

                if (instance == null)
                    throw new ArgumentException("Workflow instance not found");

                return await MapToWorkflowInstanceDto(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow instance {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<List<WorkflowInstanceDto>> GetActiveWorkflowsAsync(Guid tenantId)
        {
            try
            {
                var instances = await _context.WorkflowInstances
                    .Where(wi => wi.TenantId == tenantId && wi.Status == "Running")
                    .Include(wi => wi.WorkflowTemplate)
                    .OrderByDescending(wi => wi.StartedAt)
                    .ToListAsync();

                var result = new List<WorkflowInstanceDto>();
                foreach (var instance in instances)
                {
                    result.Add(await MapToWorkflowInstanceDto(instance));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active workflows for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<WorkflowInstanceDto> ExecuteWorkflowStepAsync(Guid instanceId, ExecuteStepRequestDto request)
        {
            try
            {
                var instance = await _context.WorkflowInstances.FindAsync(instanceId);
                if (instance == null)
                    throw new ArgumentException("Workflow instance not found");

                if (instance.Status != "Running")
                    throw new InvalidOperationException("Workflow is not in running state");

                // Update context with new data
                var context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context);
                foreach (var kvp in request.StepData)
                {
                    context[kvp.Key] = kvp.Value;
                }
                instance.Context = JsonSerializer.Serialize(context);

                // Log step execution
                await LogWorkflowEvent(instanceId, "StepExecuted", $"Step {instance.CurrentStep} executed", request.StepData);

                // Move to next step
                instance.CurrentStep++;
                instance.LastUpdatedAt = DateTime.UtcNow;

                await ExecuteNextStep(instance);
                await _context.SaveChangesAsync();

                return await MapToWorkflowInstanceDto(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow step for instance {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<bool> CancelWorkflowAsync(Guid instanceId, string reason)
        {
            try
            {
                var instance = await _context.WorkflowInstances.FindAsync(instanceId);
                if (instance == null) return false;

                instance.Status = "Cancelled";
                instance.CompletedAt = DateTime.UtcNow;
                instance.LastUpdatedAt = DateTime.UtcNow;

                await LogWorkflowEvent(instanceId, "WorkflowCancelled", reason, new Dictionary<string, object> { { "reason", reason } });
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling workflow {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<List<WorkflowHistoryDto>> GetWorkflowHistoryAsync(Guid instanceId)
        {
            try
            {
                var history = await _context.WorkflowHistory
                    .Where(wh => wh.WorkflowInstanceId == instanceId)
                    .OrderBy(wh => wh.Timestamp)
                    .ToListAsync();

                return history.Select(h => new WorkflowHistoryDto
                {
                    Id = h.Id,
                    WorkflowInstanceId = h.WorkflowInstanceId,
                    EventType = h.EventType,
                    Description = h.Description,
                    Data = JsonSerializer.Deserialize<Dictionary<string, object>>(h.Data ?? "{}"),
                    Timestamp = h.Timestamp,
                    UserId = h.UserId
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow history for instance {InstanceId}", instanceId);
                throw;
            }
        }

        private async Task ExecuteNextStep(WorkflowInstance instance)
        {
            var template = await _context.WorkflowTemplates.FindAsync(instance.WorkflowTemplateId);
            if (template == null) return;

            var steps = JsonSerializer.Deserialize<List<WorkflowStepDto>>(template.Steps);
            if (instance.CurrentStep >= steps.Count)
            {
                // Workflow completed
                instance.Status = "Completed";
                instance.CompletedAt = DateTime.UtcNow;
                await LogWorkflowEvent(instance.Id, "WorkflowCompleted", "Workflow completed successfully", new Dictionary<string, object>());
                return;
            }

            var currentStep = steps[instance.CurrentStep];
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context);

            // Execute step based on type
            await ExecuteStepByType(instance, currentStep, context);
        }

        private async Task ExecuteStepByType(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            switch (step.Type.ToLower())
            {
                case "approval":
                    await ExecuteApprovalStep(instance, step, context);
                    break;
                case "notification":
                    await ExecuteNotificationStep(instance, step, context);
                    break;
                case "automation":
                    await ExecuteAutomationStep(instance, step, context);
                    break;
                case "condition":
                    await ExecuteConditionStep(instance, step, context);
                    break;
                case "delay":
                    await ExecuteDelayStep(instance, step, context);
                    break;
                default:
                    await ExecuteCustomStep(instance, step, context);
                    break;
            }
        }

        private async Task ExecuteApprovalStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            // Create approval request
            var approvalRequest = new ApprovalRequest
            {
                Id = Guid.NewGuid(),
                TenantId = instance.TenantId,
                WorkflowInstanceId = instance.Id,
                RequestType = step.Configuration.GetValueOrDefault("requestType", "General").ToString(),
                Title = step.Configuration.GetValueOrDefault("title", "Approval Required").ToString(),
                Description = step.Configuration.GetValueOrDefault("description", "").ToString(),
                RequestData = JsonSerializer.Serialize(context),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Determine approvers
            var approvers = GetApprovers(step.Configuration, context);
            foreach (var approverId in approvers)
            {
                var approver = new ApprovalRequestApprover
                {
                    Id = Guid.NewGuid(),
                    ApprovalRequestId = approvalRequest.Id,
                    ApproverId = approverId,
                    Status = "Pending",
                    Order = 1 // Simplified - in real implementation, handle approval order
                };
                _context.ApprovalRequestApprovers.Add(approver);
            }

            _context.ApprovalRequests.Add(approvalRequest);

            // Pause workflow until approval
            instance.Status = "WaitingForApproval";
            await LogWorkflowEvent(instance.Id, "ApprovalRequested", "Waiting for approval", step.Configuration);
        }

        private async Task ExecuteNotificationStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            // Send notification
            var recipients = GetNotificationRecipients(step.Configuration, context);
            var message = ReplaceTokens(step.Configuration.GetValueOrDefault("message", "").ToString(), context);
            var subject = ReplaceTokens(step.Configuration.GetValueOrDefault("subject", "").ToString(), context);

            // In a real implementation, this would integrate with the notification service
            await LogWorkflowEvent(instance.Id, "NotificationSent", $"Notification sent to {recipients.Count} recipients", step.Configuration);
        }

        private async Task ExecuteAutomationStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            var automationType = step.Configuration.GetValueOrDefault("automationType", "").ToString();
            
            switch (automationType.ToLower())
            {
                case "updaterecord":
                    await ExecuteUpdateRecordAutomation(step.Configuration, context);
                    break;
                case "sendnotification":
                    await ExecuteSendNotificationAutomation(step.Configuration, context);
                    break;
                case "createtask":
                    await ExecuteCreateTaskAutomation(step.Configuration, context);
                    break;
            }

            await LogWorkflowEvent(instance.Id, "AutomationExecuted", $"Automation '{automationType}' executed", step.Configuration);
        }

        private async Task ExecuteConditionStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            var condition = step.Configuration.GetValueOrDefault("condition", "").ToString();
            var result = EvaluateCondition(condition, context);

            if (result)
            {
                var trueSteps = step.Configuration.GetValueOrDefault("trueSteps", new List<object>()) as List<object>;
                // Execute true branch steps
            }
            else
            {
                var falseSteps = step.Configuration.GetValueOrDefault("falseSteps", new List<object>()) as List<object>;
                // Execute false branch steps
            }

            await LogWorkflowEvent(instance.Id, "ConditionEvaluated", $"Condition evaluated to {result}", new Dictionary<string, object> { { "condition", condition }, { "result", result } });
        }

        private async Task ExecuteDelayStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            var delayMinutes = Convert.ToInt32(step.Configuration.GetValueOrDefault("delayMinutes", 0));
            
            // Schedule workflow to resume after delay
            instance.Status = "Delayed";
            instance.ResumeAt = DateTime.UtcNow.AddMinutes(delayMinutes);

            await LogWorkflowEvent(instance.Id, "WorkflowDelayed", $"Workflow delayed for {delayMinutes} minutes", step.Configuration);
        }

        private async Task ExecuteCustomStep(WorkflowInstance instance, WorkflowStepDto step, Dictionary<string, object> context)
        {
            // Execute custom step logic
            await LogWorkflowEvent(instance.Id, "CustomStepExecuted", $"Custom step '{step.Type}' executed", step.Configuration);
        }

        private List<Guid> GetApprovers(Dictionary<string, object> configuration, Dictionary<string, object> context)
        {
            var approvers = new List<Guid>();
            
            if (configuration.ContainsKey("approvers"))
            {
                var approverIds = configuration["approvers"] as List<object>;
                if (approverIds != null)
                {
                    approvers.AddRange(approverIds.Select(id => Guid.Parse(id.ToString())));
                }
            }

            // Add dynamic approver logic based on context
            if (configuration.ContainsKey("approverRule"))
            {
                var rule = configuration["approverRule"].ToString();
                var dynamicApprovers = GetDynamicApprovers(rule, context);
                approvers.AddRange(dynamicApprovers);
            }

            return approvers;
        }

        private List<Guid> GetDynamicApprovers(string rule, Dictionary<string, object> context)
        {
            // Implement dynamic approver selection logic
            return new List<Guid>();
        }

        private List<Guid> GetNotificationRecipients(Dictionary<string, object> configuration, Dictionary<string, object> context)
        {
            var recipients = new List<Guid>();
            
            if (configuration.ContainsKey("recipients"))
            {
                var recipientIds = configuration["recipients"] as List<object>;
                if (recipientIds != null)
                {
                    recipients.AddRange(recipientIds.Select(id => Guid.Parse(id.ToString())));
                }
            }

            return recipients;
        }

        private string ReplaceTokens(string template, Dictionary<string, object> context)
        {
            var result = template;
            foreach (var kvp in context)
            {
                result = result.Replace($"{{{kvp.Key}}}", kvp.Value?.ToString() ?? "");
            }
            return result;
        }

        private bool EvaluateCondition(string condition, Dictionary<string, object> context)
        {
            try
            {
                // Simple condition evaluation - in a real implementation, use a proper expression evaluator
                // For now, return true as placeholder
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task ExecuteUpdateRecordAutomation(Dictionary<string, object> configuration, Dictionary<string, object> context)
        {
            // Implementation for updating records
        }

        private async Task ExecuteSendNotificationAutomation(Dictionary<string, object> configuration, Dictionary<string, object> context)
        {
            // Implementation for sending notifications
        }

        private async Task ExecuteCreateTaskAutomation(Dictionary<string, object> configuration, Dictionary<string, object> context)
        {
            // Implementation for creating tasks
        }

        private async Task LogWorkflowEvent(Guid instanceId, string eventType, string description, Dictionary<string, object> data)
        {
            var historyEntry = new WorkflowHistory
            {
                Id = Guid.NewGuid(),
                WorkflowInstanceId = instanceId,
                EventType = eventType,
                Description = description,
                Data = JsonSerializer.Serialize(data),
                Timestamp = DateTime.UtcNow
            };

            _context.WorkflowHistory.Add(historyEntry);
        }

        private async Task<WorkflowInstanceDto> MapToWorkflowInstanceDto(WorkflowInstance instance)
        {
            return new WorkflowInstanceDto
            {
                Id = instance.Id,
                TenantId = instance.TenantId,
                WorkflowTemplateId = instance.WorkflowTemplateId,
                WorkflowTemplateName = instance.WorkflowTemplate?.Name ?? "",
                InitiatedBy = instance.InitiatedBy,
                Status = instance.Status,
                CurrentStep = instance.CurrentStep,
                Context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context ?? "{}"),
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                LastUpdatedAt = instance.LastUpdatedAt,
                ResumeAt = instance.ResumeAt
            };
        }
    }

    // Business Rules Service Implementation
    public class BusinessRulesService : IBusinessRulesService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<BusinessRulesService> _logger;

        public BusinessRulesService(AttendancePlatformDbContext context, ILogger<BusinessRulesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BusinessRuleDto> CreateRuleAsync(CreateBusinessRuleRequestDto request)
        {
            try
            {
                var rule = new BusinessRule
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Description = request.Description,
                    Category = request.Category,
                    RuleType = request.RuleType,
                    Conditions = JsonSerializer.Serialize(request.Conditions),
                    Actions = JsonSerializer.Serialize(request.Actions),
                    Priority = request.Priority,
                    IsActive = request.IsActive,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BusinessRules.Add(rule);
                await _context.SaveChangesAsync();

                return MapToBusinessRuleDto(rule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating business rule");
                throw;
            }
        }

        public async Task<List<BusinessRuleDto>> GetRulesAsync(Guid tenantId, string category)
        {
            try
            {
                var query = _context.BusinessRules.Where(br => br.TenantId == tenantId);
                
                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(br => br.Category == category);
                }

                var rules = await query.OrderBy(br => br.Priority).ToListAsync();
                return rules.Select(MapToBusinessRuleDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rules");
                throw;
            }
        }

        public async Task<BusinessRuleEvaluationResultDto> EvaluateRuleAsync(Guid ruleId, Dictionary<string, object> context)
        {
            try
            {
                var rule = await _context.BusinessRules.FindAsync(ruleId);
                if (rule == null)
                    throw new ArgumentException("Business rule not found");

                var conditions = JsonSerializer.Deserialize<List<BusinessRuleConditionDto>>(rule.Conditions);
                var actions = JsonSerializer.Deserialize<List<BusinessRuleActionDto>>(rule.Actions);

                var conditionsMet = EvaluateConditions(conditions, context);
                var actionsExecuted = new List<string>();

                if (conditionsMet)
                {
                    actionsExecuted = await ExecuteActions(actions, context);
                }

                return new BusinessRuleEvaluationResultDto
                {
                    RuleId = ruleId,
                    RuleName = rule.Name,
                    ConditionsMet = conditionsMet,
                    ActionsExecuted = actionsExecuted,
                    EvaluatedAt = DateTime.UtcNow,
                    Context = context
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating business rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<List<BusinessRuleEvaluationResultDto>> EvaluateRulesAsync(Guid tenantId, string category, Dictionary<string, object> context)
        {
            try
            {
                var rules = await GetRulesAsync(tenantId, category);
                var results = new List<BusinessRuleEvaluationResultDto>();

                foreach (var rule in rules.Where(r => r.IsActive))
                {
                    var result = await EvaluateRuleAsync(rule.Id, context);
                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating business rules for category {Category}", category);
                throw;
            }
        }

        public async Task<BusinessRuleDto> UpdateRuleAsync(Guid ruleId, UpdateBusinessRuleRequestDto request)
        {
            try
            {
                var rule = await _context.BusinessRules.FindAsync(ruleId);
                if (rule == null)
                    throw new ArgumentException("Business rule not found");

                rule.Name = request.Name ?? rule.Name;
                rule.Description = request.Description ?? rule.Description;
                rule.Conditions = request.Conditions != null ? JsonSerializer.Serialize(request.Conditions) : rule.Conditions;
                rule.Actions = request.Actions != null ? JsonSerializer.Serialize(request.Actions) : rule.Actions;
                rule.Priority = request.Priority ?? rule.Priority;
                rule.IsActive = request.IsActive ?? rule.IsActive;
                rule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return MapToBusinessRuleDto(rule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating business rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<bool> DeleteRuleAsync(Guid ruleId)
        {
            try
            {
                var rule = await _context.BusinessRules.FindAsync(ruleId);
                if (rule == null) return false;

                _context.BusinessRules.Remove(rule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting business rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<List<BusinessRuleTemplateDto>> GetRuleTemplatesAsync()
        {
            try
            {
                return new List<BusinessRuleTemplateDto>
                {
                    new BusinessRuleTemplateDto
                    {
                        Id = "attendance-late-arrival",
                        Name = "Late Arrival Alert",
                        Description = "Send alert when employee arrives late",
                        Category = "Attendance",
                        ConditionTemplate = new List<BusinessRuleConditionDto>
                        {
                            new BusinessRuleConditionDto
                            {
                                Field = "CheckInTime",
                                Operator = "GreaterThan",
                                Value = "09:00",
                                DataType = "Time"
                            }
                        },
                        ActionTemplate = new List<BusinessRuleActionDto>
                        {
                            new BusinessRuleActionDto
                            {
                                Type = "SendNotification",
                                Configuration = new Dictionary<string, object>
                                {
                                    { "recipients", new[] { "manager", "hr" } },
                                    { "message", "Employee {EmployeeName} arrived late at {CheckInTime}" }
                                }
                            }
                        }
                    },
                    new BusinessRuleTemplateDto
                    {
                        Id = "overtime-approval",
                        Name = "Overtime Approval Required",
                        Description = "Require approval for overtime work",
                        Category = "Attendance",
                        ConditionTemplate = new List<BusinessRuleConditionDto>
                        {
                            new BusinessRuleConditionDto
                            {
                                Field = "WorkHours",
                                Operator = "GreaterThan",
                                Value = "8",
                                DataType = "Number"
                            }
                        },
                        ActionTemplate = new List<BusinessRuleActionDto>
                        {
                            new BusinessRuleActionDto
                            {
                                Type = "RequireApproval",
                                Configuration = new Dictionary<string, object>
                                {
                                    { "approvers", new[] { "manager" } },
                                    { "message", "Overtime approval required for {EmployeeName}" }
                                }
                            }
                        }
                    },
                    new BusinessRuleTemplateDto
                    {
                        Id = "leave-balance-warning",
                        Name = "Low Leave Balance Warning",
                        Description = "Warn when leave balance is low",
                        Category = "Leave",
                        ConditionTemplate = new List<BusinessRuleConditionDto>
                        {
                            new BusinessRuleConditionDto
                            {
                                Field = "LeaveBalance",
                                Operator = "LessThan",
                                Value = "5",
                                DataType = "Number"
                            }
                        },
                        ActionTemplate = new List<BusinessRuleActionDto>
                        {
                            new BusinessRuleActionDto
                            {
                                Type = "SendNotification",
                                Configuration = new Dictionary<string, object>
                                {
                                    { "recipients", new[] { "employee", "hr" } },
                                    { "message", "Leave balance is low: {LeaveBalance} days remaining" }
                                }
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting business rule templates");
                throw;
            }
        }

        private bool EvaluateConditions(List<BusinessRuleConditionDto> conditions, Dictionary<string, object> context)
        {
            foreach (var condition in conditions)
            {
                if (!EvaluateCondition(condition, context))
                {
                    return false; // All conditions must be met
                }
            }
            return true;
        }

        private bool EvaluateCondition(BusinessRuleConditionDto condition, Dictionary<string, object> context)
        {
            if (!context.ContainsKey(condition.Field))
                return false;

            var fieldValue = context[condition.Field];
            var conditionValue = condition.Value;

            return condition.Operator.ToLower() switch
            {
                "equals" => CompareValues(fieldValue, conditionValue, (a, b) => a.Equals(b)),
                "notequals" => CompareValues(fieldValue, conditionValue, (a, b) => !a.Equals(b)),
                "greaterthan" => CompareValues(fieldValue, conditionValue, (a, b) => Comparer<object>.Default.Compare(a, b) > 0),
                "lessthan" => CompareValues(fieldValue, conditionValue, (a, b) => Comparer<object>.Default.Compare(a, b) < 0),
                "contains" => fieldValue.ToString().Contains(conditionValue.ToString()),
                "startswith" => fieldValue.ToString().StartsWith(conditionValue.ToString()),
                "endswith" => fieldValue.ToString().EndsWith(conditionValue.ToString()),
                _ => false
            };
        }

        private bool CompareValues(object fieldValue, object conditionValue, Func<object, object, bool> comparer)
        {
            try
            {
                // Convert values to appropriate types for comparison
                if (fieldValue is DateTime dtField && DateTime.TryParse(conditionValue.ToString(), out var dtCondition))
                {
                    return comparer(dtField, dtCondition);
                }
                
                if (double.TryParse(fieldValue.ToString(), out var numField) && double.TryParse(conditionValue.ToString(), out var numCondition))
                {
                    return comparer(numField, numCondition);
                }

                return comparer(fieldValue.ToString(), conditionValue.ToString());
            }
            catch
            {
                return false;
            }
        }

        private async Task<List<string>> ExecuteActions(List<BusinessRuleActionDto> actions, Dictionary<string, object> context)
        {
            var executedActions = new List<string>();

            foreach (var action in actions)
            {
                try
                {
                    await ExecuteAction(action, context);
                    executedActions.Add(action.Type);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing action {ActionType}", action.Type);
                }
            }

            return executedActions;
        }

        private async Task ExecuteAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            switch (action.Type.ToLower())
            {
                case "sendnotification":
                    await ExecuteSendNotificationAction(action, context);
                    break;
                case "requireapproval":
                    await ExecuteRequireApprovalAction(action, context);
                    break;
                case "updatefield":
                    await ExecuteUpdateFieldAction(action, context);
                    break;
                case "createtask":
                    await ExecuteCreateTaskAction(action, context);
                    break;
                case "logaudit":
                    await ExecuteLogAuditAction(action, context);
                    break;
            }
        }

        private async Task ExecuteSendNotificationAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            // Implementation for sending notifications
            _logger.LogInformation("Executing SendNotification action");
        }

        private async Task ExecuteRequireApprovalAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            // Implementation for requiring approval
            _logger.LogInformation("Executing RequireApproval action");
        }

        private async Task ExecuteUpdateFieldAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            // Implementation for updating fields
            _logger.LogInformation("Executing UpdateField action");
        }

        private async Task ExecuteCreateTaskAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            // Implementation for creating tasks
            _logger.LogInformation("Executing CreateTask action");
        }

        private async Task ExecuteLogAuditAction(BusinessRuleActionDto action, Dictionary<string, object> context)
        {
            // Implementation for logging audit events
            _logger.LogInformation("Executing LogAudit action");
        }

        private BusinessRuleDto MapToBusinessRuleDto(BusinessRule rule)
        {
            return new BusinessRuleDto
            {
                Id = rule.Id,
                TenantId = rule.TenantId,
                Name = rule.Name,
                Description = rule.Description,
                Category = rule.Category,
                RuleType = rule.RuleType,
                Conditions = JsonSerializer.Deserialize<List<BusinessRuleConditionDto>>(rule.Conditions),
                Actions = JsonSerializer.Deserialize<List<BusinessRuleActionDto>>(rule.Actions),
                Priority = rule.Priority,
                IsActive = rule.IsActive,
                CreatedAt = rule.CreatedAt,
                UpdatedAt = rule.UpdatedAt
            };
        }
    }

    // Entity Models for Workflow Engine
    public class WorkflowTemplate
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Steps { get; set; } = string.Empty; // JSON
        public string Configuration { get; set; } = string.Empty; // JSON
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WorkflowInstance
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid WorkflowTemplateId { get; set; }
        public Guid InitiatedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentStep { get; set; }
        public string Context { get; set; } = string.Empty; // JSON
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime? ResumeAt { get; set; }
        
        public WorkflowTemplate WorkflowTemplate { get; set; } = null!;
    }

    public class WorkflowHistory
    {
        public Guid Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Data { get; set; } // JSON
        public DateTime Timestamp { get; set; }
        public Guid? UserId { get; set; }
    }

    public class BusinessRule
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty;
        public string Conditions { get; set; } = string.Empty; // JSON
        public string Actions { get; set; } = string.Empty; // JSON
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ApprovalRequest
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? WorkflowInstanceId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestData { get; set; } = string.Empty; // JSON
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Comments { get; set; }
    }

    public class ApprovalRequestApprover
    {
        public Guid Id { get; set; }
        public Guid ApprovalRequestId { get; set; }
        public Guid ApproverId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Comments { get; set; }
    }

    // DTOs for Workflow Services
    public class StartWorkflowRequestDto
    {
        public Guid TenantId { get; set; }
        public Guid WorkflowTemplateId { get; set; }
        public Guid InitiatedBy { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public class ExecuteStepRequestDto
    {
        public Dictionary<string, object> StepData { get; set; } = new();
        public string? Comments { get; set; }
    }

    public class WorkflowInstanceDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid WorkflowTemplateId { get; set; }
        public string WorkflowTemplateName { get; set; } = string.Empty;
        public Guid InitiatedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentStep { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public DateTime? ResumeAt { get; set; }
    }

    public class WorkflowHistoryDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowInstanceId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> Data { get; set; } = new();
        public DateTime Timestamp { get; set; }
        public Guid? UserId { get; set; }
    }

    public class WorkflowStepDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
        public int Order { get; set; }
    }

    public class CreateBusinessRuleRequestDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty;
        public List<BusinessRuleConditionDto> Conditions { get; set; } = new();
        public List<BusinessRuleActionDto> Actions { get; set; } = new();
        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateBusinessRuleRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<BusinessRuleConditionDto>? Conditions { get; set; }
        public List<BusinessRuleActionDto>? Actions { get; set; }
        public int? Priority { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BusinessRuleDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string RuleType { get; set; } = string.Empty;
        public List<BusinessRuleConditionDto> Conditions { get; set; } = new();
        public List<BusinessRuleActionDto> Actions { get; set; } = new();
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BusinessRuleConditionDto
    {
        public string Field { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public object Value { get; set; } = new();
        public string DataType { get; set; } = string.Empty;
        public string? LogicalOperator { get; set; } // AND, OR
    }

    public class BusinessRuleActionDto
    {
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
    }

    public class BusinessRuleEvaluationResultDto
    {
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public bool ConditionsMet { get; set; }
        public List<string> ActionsExecuted { get; set; } = new();
        public DateTime EvaluatedAt { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public class BusinessRuleTemplateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<BusinessRuleConditionDto> ConditionTemplate { get; set; } = new();
        public List<BusinessRuleActionDto> ActionTemplate { get; set; } = new();
    }

    public class CreateApprovalWorkflowRequestDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public List<ApprovalStepDto> Steps { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class UpdateApprovalWorkflowRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<ApprovalStepDto>? Steps { get; set; }
        public bool? IsActive { get; set; }
    }

    public class ApprovalWorkflowDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public List<ApprovalStepDto> Steps { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ApprovalStepDto
    {
        public int Order { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Guid> Approvers { get; set; } = new();
        public string ApprovalType { get; set; } = string.Empty; // Sequential, Parallel, Any
        public bool IsRequired { get; set; } = true;
    }

    public class SubmitApprovalRequestDto
    {
        public Guid TenantId { get; set; }
        public Guid WorkflowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> RequestData { get; set; } = new();
        public Guid RequestedBy { get; set; }
    }

    public class ProcessApprovalRequestDto
    {
        public string Action { get; set; } = string.Empty; // Approve, Reject, RequestInfo
        public string? Comments { get; set; }
        public Guid ProcessedBy { get; set; }
    }

    public class ApprovalRequestDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? WorkflowInstanceId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Dictionary<string, object> RequestData { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Comments { get; set; }
        public List<ApprovalRequestApproverDto> Approvers { get; set; } = new();
    }

    public class ApprovalRequestApproverDto
    {
        public Guid Id { get; set; }
        public Guid ApproverId { get; set; }
        public string ApproverName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Order { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Comments { get; set; }
    }

    public class CreateAutomationRuleRequestDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TriggerType { get; set; } = string.Empty;
        public Dictionary<string, object> TriggerConfiguration { get; set; } = new();
        public List<BusinessRuleConditionDto> Conditions { get; set; } = new();
        public List<AutomationActionDto> Actions { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class UpdateAutomationRuleRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, object>? TriggerConfiguration { get; set; }
        public List<BusinessRuleConditionDto>? Conditions { get; set; }
        public List<AutomationActionDto>? Actions { get; set; }
        public bool? IsActive { get; set; }
    }

    public class AutomationRuleDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TriggerType { get; set; } = string.Empty;
        public Dictionary<string, object> TriggerConfiguration { get; set; } = new();
        public List<BusinessRuleConditionDto> Conditions { get; set; } = new();
        public List<AutomationActionDto> Actions { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastExecuted { get; set; }
        public int ExecutionCount { get; set; }
    }

    public class AutomationActionDto
    {
        public string Type { get; set; } = string.Empty;
        public Dictionary<string, object> Configuration { get; set; } = new();
        public int Order { get; set; }
        public bool IsEnabled { get; set; } = true;
    }

    public class AutomationExecutionResultDto
    {
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public List<string> ActionsExecuted { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public DateTime ExecutedAt { get; set; }
        public long ExecutionTimeMs { get; set; }
    }

    public class AutomationExecutionLogDto
    {
        public Guid Id { get; set; }
        public Guid RuleId { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ExecutedAt { get; set; }
        public long ExecutionTimeMs { get; set; }
        public Dictionary<string, object> Context { get; set; } = new();
    }

    public class AutomationTemplateDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string TriggerType { get; set; } = string.Empty;
        public Dictionary<string, object> TriggerTemplate { get; set; } = new();
        public List<BusinessRuleConditionDto> ConditionTemplate { get; set; } = new();
        public List<AutomationActionDto> ActionTemplate { get; set; } = new();
    }

    public class CreateWorkflowTemplateRequestDto
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<WorkflowStepDto> Steps { get; set; } = new();
        public Dictionary<string, object> Configuration { get; set; } = new();
        public bool IsActive { get; set; } = true;
    }

    public class UpdateWorkflowTemplateRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<WorkflowStepDto>? Steps { get; set; }
        public Dictionary<string, object>? Configuration { get; set; }
        public bool? IsActive { get; set; }
    }

    public class WorkflowTemplateDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<WorkflowStepDto> Steps { get; set; } = new();
        public Dictionary<string, object> Configuration { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class WorkflowTemplateCategoryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TemplateCount { get; set; }
    }

    public class WorkflowExecutionResultDto
    {
        public Guid WorkflowId { get; set; }
        public Guid InstanceId { get; set; }
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
}

