using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Workflow.Api.Services
{
    public class WorkflowExecutionService : IWorkflowExecutionService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<WorkflowExecutionService> _logger;

        public WorkflowExecutionService(AttendancePlatformDbContext context, ILogger<WorkflowExecutionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WorkflowExecutionResultDto> ExecuteWorkflowAsync(Guid workflowId, Dictionary<string, object> context)
        {
            try
            {
                var workflow = await _context.WorkflowInstances
                    .Include(w => w.WorkflowTemplate)
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                if (workflow == null)
                    throw new ArgumentException($"Workflow instance with ID {workflowId} not found");

                var result = new WorkflowExecutionResultDto
                {
                    WorkflowId = workflowId,
                    Status = "InProgress",
                    StartedAt = DateTime.UtcNow,
                    Context = System.Text.Json.JsonSerializer.Serialize(context)
                };

                workflow.Status = "Running";
                workflow.LastUpdatedAt = DateTime.UtcNow;
                workflow.Context = result.Context;

                var executionLog = new WorkflowExecutionLog
                {
                    Id = Guid.NewGuid(),
                    WorkflowInstanceId = workflowId,
                    Action = "WorkflowStarted",
                    ExecutedAt = DateTime.UtcNow,
                    Comments = "Workflow execution initiated",
                    InputData = result.Context,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = Guid.Empty
                };

                _context.WorkflowExecutionLogs.Add(executionLog);
                await _context.SaveChangesAsync();

                result.Status = "Completed";
                result.CompletedAt = DateTime.UtcNow;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<List<WorkflowExecutionLogDto>> GetExecutionLogsAsync(Guid tenantId, Guid? workflowId = null)
        {
            try
            {
                var query = _context.WorkflowExecutionLogs
                    .Include(l => l.WorkflowInstance)
                    .Where(l => l.WorkflowInstance.TenantId == tenantId);

                if (workflowId.HasValue)
                {
                    query = query.Where(l => l.WorkflowInstanceId == workflowId.Value);
                }

                var logs = await query
                    .OrderByDescending(l => l.ExecutedAt)
                    .Select(l => new WorkflowExecutionLogDto
                    {
                        Id = l.Id,
                        WorkflowInstanceId = l.WorkflowInstanceId,
                        Action = l.Action,
                        ExecutedAt = l.ExecutedAt,
                        Comments = l.Comments,
                        InputData = l.InputData,
                        OutputData = l.OutputData
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving execution logs for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<WorkflowExecutionMetricsDto> GetExecutionMetricsAsync(Guid tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var query = _context.WorkflowInstances
                    .Where(w => w.TenantId == tenantId);

                if (startDate.HasValue)
                {
                    query = query.Where(w => w.StartedAt >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(w => w.StartedAt <= endDate.Value);
                }

                var totalWorkflows = await query.CountAsync();
                var completedWorkflows = await query.CountAsync(w => w.Status == "Completed");
                var failedWorkflows = await query.CountAsync(w => w.Status == "Failed");
                var runningWorkflows = await query.CountAsync(w => w.Status == "Running");

                var metrics = new WorkflowExecutionMetricsDto
                {
                    TotalWorkflows = totalWorkflows,
                    CompletedWorkflows = completedWorkflows,
                    FailedWorkflows = failedWorkflows,
                    RunningWorkflows = runningWorkflows,
                    SuccessRate = totalWorkflows > 0 ? (double)completedWorkflows / totalWorkflows * 100 : 0,
                    PeriodStart = startDate ?? DateTime.UtcNow.AddDays(-30),
                    PeriodEnd = endDate ?? DateTime.UtcNow
                };

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving execution metrics for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<WorkflowPerformanceDto> GetWorkflowPerformanceAsync(Guid workflowId)
        {
            try
            {
                var workflow = await _context.WorkflowInstances
                    .Include(w => w.WorkflowExecutionLogs)
                    .FirstOrDefaultAsync(w => w.Id == workflowId);

                if (workflow == null)
                    throw new ArgumentException($"Workflow instance with ID {workflowId} not found");

                var performance = new WorkflowPerformanceDto
                {
                    WorkflowId = workflowId,
                    StartTime = workflow.StartedAt,
                    EndTime = workflow.CompletedAt,
                    Duration = workflow.CompletedAt.HasValue 
                        ? workflow.CompletedAt.Value - workflow.StartedAt 
                        : DateTime.UtcNow - workflow.StartedAt,
                    Status = workflow.Status,
                    StepCount = workflow.CurrentStepIndex + 1,
                    ExecutionLogs = workflow.WorkflowExecutionLogs.Count
                };

                return performance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow performance for {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<List<WorkflowMetricsDto>> GetWorkflowMetricsAsync(Guid tenantId)
        {
            try
            {
                var workflows = await _context.WorkflowInstances
                    .Where(w => w.TenantId == tenantId)
                    .ToListAsync();

                var totalWorkflows = workflows.Count;
                var completedWorkflows = workflows.Count(w => w.Status == "Completed");
                var rejectedWorkflows = workflows.Count(w => w.Status == "Rejected");
                var cancelledWorkflows = workflows.Count(w => w.Status == "Cancelled");
                var activeWorkflows = workflows.Count(w => w.Status == "Running" || w.Status == "Pending");

                var completedWithTimes = workflows.Where(w => w.Status == "Completed" && w.CompletedAt.HasValue && w.StartedAt.HasValue);
                var averageCompletionTime = completedWithTimes.Any() 
                    ? completedWithTimes.Average(w => (w.CompletedAt!.Value - w.StartedAt).TotalMinutes)
                    : 0;

                var workflowsByType = workflows
                    .GroupBy(w => w.WorkflowType)
                    .ToDictionary(g => g.Key, g => g.Count());

                var completionRate = totalWorkflows > 0 ? (double)completedWorkflows / totalWorkflows * 100 : 0;

                var metrics = new WorkflowMetricsDto
                {
                    TotalWorkflows = totalWorkflows,
                    CompletedWorkflows = completedWorkflows,
                    RejectedWorkflows = rejectedWorkflows,
                    CancelledWorkflows = cancelledWorkflows,
                    ActiveWorkflows = activeWorkflows,
                    AverageCompletionTime = averageCompletionTime,
                    WorkflowsByType = workflowsByType,
                    CompletionRate = completionRate
                };

                return new List<WorkflowMetricsDto> { metrics };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow metrics for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
