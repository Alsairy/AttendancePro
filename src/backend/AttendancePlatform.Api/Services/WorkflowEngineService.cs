using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IWorkflowEngineService
    {
        Task<WorkflowDefinitionDto> CreateWorkflowAsync(WorkflowDefinitionDto workflow);
        Task<WorkflowInstanceDto> StartWorkflowAsync(Guid workflowId, Dictionary<string, object> parameters);
        Task<WorkflowInstanceDto> GetWorkflowInstanceAsync(Guid instanceId);
        Task<List<WorkflowInstanceDto>> GetActiveWorkflowsAsync(Guid tenantId);
        Task<bool> CompleteTaskAsync(Guid taskId, Dictionary<string, object> outputs);
        Task<bool> CancelWorkflowAsync(Guid instanceId);
        Task<List<WorkflowTaskDto>> GetPendingTasksAsync(Guid userId);
        Task<List<WorkflowDefinitionDto>> GetWorkflowDefinitionsAsync(Guid tenantId);
        Task<WorkflowExecutionReportDto> GetExecutionReportAsync(Guid instanceId);
        Task<bool> UpdateWorkflowDefinitionAsync(Guid workflowId, WorkflowDefinitionDto workflow);
    }

    public class WorkflowEngineService : IWorkflowEngineService
    {
        private readonly ILogger<WorkflowEngineService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public WorkflowEngineService(ILogger<WorkflowEngineService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<WorkflowDefinitionDto> CreateWorkflowAsync(WorkflowDefinitionDto workflow)
        {
            try
            {
                var workflowEntity = new AttendancePlatform.Shared.Domain.Entities.WorkflowDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = workflow.Name,
                    Description = workflow.Description,
                    TenantId = workflow.TenantId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Version = 1
                };

                _context.WorkflowDefinitions.Add(workflowEntity);
                await _context.SaveChangesAsync();

                workflow.Id = workflowEntity.Id;
                workflow.CreatedAt = workflowEntity.CreatedAt;

                _logger.LogInformation("Workflow definition created: {WorkflowId}", workflowEntity.Id);
                return workflow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create workflow definition");
                throw;
            }
        }

        public async Task<WorkflowInstanceDto> StartWorkflowAsync(Guid workflowId, Dictionary<string, object> parameters)
        {
            try
            {
                var workflowDefinition = await _context.WorkflowDefinitions.FindAsync(workflowId);
                if (workflowDefinition == null)
                    throw new ArgumentException("Workflow definition not found");

                var instance = new AttendancePlatform.Shared.Domain.Entities.WorkflowInstance
                {
                    Id = Guid.NewGuid(),
                    WorkflowTemplateId = workflowId,
                    TenantId = workflowDefinition.TenantId,
                    WorkflowType = "General",
                    EntityId = Guid.NewGuid(),
                    EntityType = "Workflow",
                    InitiatedBy = Guid.NewGuid(),
                    Status = "Running",
                    StartedAt = DateTime.UtcNow,
                    CurrentStep = "Initial"
                };

                _context.WorkflowInstances.Add(instance);
                await _context.SaveChangesAsync();

                var instanceDto = new WorkflowInstanceDto
                {
                    Id = instance.Id,
                    WorkflowDefinitionId = workflowId,
                    Status = instance.Status,
                    StartedAt = instance.StartedAt,
                    CurrentStep = instance.CurrentStep,
                    Parameters = parameters
                };

                _logger.LogInformation("Workflow instance started: {InstanceId}", instance.Id);
                return instanceDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to start workflow instance");
                throw;
            }
        }

        public async Task<WorkflowInstanceDto> GetWorkflowInstanceAsync(Guid instanceId)
        {
            try
            {
                var instance = await _context.WorkflowInstances
                    .FirstOrDefaultAsync(w => w.Id == instanceId);

                if (instance == null) return null;

                return new WorkflowInstanceDto
                {
                    Id = instance.Id,
                    WorkflowDefinitionId = instance.WorkflowTemplateId,
                    Status = instance.Status,
                    StartedAt = instance.StartedAt,
                    CompletedAt = instance.CompletedAt,
                    CurrentStep = instance.CurrentStep,
                    WorkflowName = "Workflow Instance"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow instance: {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<List<WorkflowInstanceDto>> GetActiveWorkflowsAsync(Guid tenantId)
        {
            try
            {
                var instances = await _context.WorkflowInstances
                    .Where(w => w.TenantId == tenantId && w.Status == "Running")
                    .Select(w => new WorkflowInstanceDto
                    {
                        Id = w.Id,
                        WorkflowDefinitionId = w.WorkflowTemplateId,
                        Status = w.Status,
                        StartedAt = w.StartedAt,
                        CurrentStep = w.CurrentStep,
                        WorkflowName = "Active Workflow"
                    })
                    .ToListAsync();

                return instances;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get active workflows for tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<bool> CompleteTaskAsync(Guid taskId, Dictionary<string, object> outputs)
        {
            try
            {
                var task = await _context.WorkflowTasks.FindAsync(taskId);
                if (task == null) return false;

                task.Status = "Completed";
                task.CompletedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow task completed: {TaskId}", taskId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete workflow task: {TaskId}", taskId);
                throw;
            }
        }

        public async Task<bool> CancelWorkflowAsync(Guid instanceId)
        {
            try
            {
                var instance = await _context.WorkflowInstances.FindAsync(instanceId);
                if (instance == null) return false;

                instance.Status = "Cancelled";
                instance.CompletedAt = DateTime.UtcNow;
                instance.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow instance cancelled: {InstanceId}", instanceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel workflow instance: {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<List<WorkflowTaskDto>> GetPendingTasksAsync(Guid userId)
        {
            try
            {
                var tasks = await _context.WorkflowTasks
                    .Where(t => t.AssignedUserId == userId && t.Status == "Pending")
                    .Select(t => new WorkflowTaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        DueDate = t.DueDate,
                        Priority = t.Priority
                    })
                    .ToListAsync();

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pending tasks for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<WorkflowDefinitionDto>> GetWorkflowDefinitionsAsync(Guid tenantId)
        {
            try
            {
                var definitions = await _context.WorkflowDefinitions
                    .Where(w => w.TenantId == tenantId && w.IsActive)
                    .Select(w => new WorkflowDefinitionDto
                    {
                        Id = w.Id,
                        Name = w.Name,
                        Description = w.Description,
                        TenantId = w.TenantId,
                        IsActive = w.IsActive,
                        CreatedAt = w.CreatedAt,
                        Version = w.Version
                    })
                    .ToListAsync();

                return definitions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get workflow definitions for tenant: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<WorkflowExecutionReportDto> GetExecutionReportAsync(Guid instanceId)
        {
            try
            {
                var instance = await _context.WorkflowInstances
                    .FirstOrDefaultAsync(w => w.Id == instanceId);

                if (instance == null) return null;

                var tasks = await _context.WorkflowTasks
                    .Where(t => t.WorkflowInstanceId == instanceId)
                    .ToListAsync();

                return new WorkflowExecutionReportDto
                {
                    InstanceId = instanceId,
                    WorkflowName = "Execution Report",
                    Status = instance.Status,
                    StartedAt = instance.StartedAt,
                    CompletedAt = instance.CompletedAt,
                    Duration = instance.CompletedAt?.Subtract(instance.StartedAt),
                    TotalTasks = tasks.Count,
                    CompletedTasks = tasks.Count(t => t.Status == "Completed"),
                    PendingTasks = tasks.Count(t => t.Status == "Pending"),
                    FailedTasks = tasks.Count(t => t.Status == "Failed")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get execution report for instance: {InstanceId}", instanceId);
                throw;
            }
        }

        public async Task<bool> UpdateWorkflowDefinitionAsync(Guid workflowId, WorkflowDefinitionDto workflow)
        {
            try
            {
                var existingWorkflow = await _context.WorkflowDefinitions.FindAsync(workflowId);
                if (existingWorkflow == null) return false;

                existingWorkflow.Name = workflow.Name;
                existingWorkflow.Description = workflow.Description;
                existingWorkflow.UpdatedAt = DateTime.UtcNow;
                existingWorkflow.Version++;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Workflow definition updated: {WorkflowId}", workflowId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update workflow definition: {WorkflowId}", workflowId);
                throw;
            }
        }
    }


    public class WorkflowDefinitionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid TenantId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Version { get; set; }
    }

    public class WorkflowInstanceDto
    {
        public Guid Id { get; set; }
        public Guid WorkflowDefinitionId { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string CurrentStep { get; set; }
        public string WorkflowName { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }

    public class WorkflowTaskDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; }
    }

    public class WorkflowExecutionReportDto
    {
        public Guid InstanceId { get; set; }
        public string WorkflowName { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int PendingTasks { get; set; }
        public int FailedTasks { get; set; }
    }
}
