using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Workflow.Api.Services
{
    public class ApprovalWorkflowService : IApprovalWorkflowService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<ApprovalWorkflowService> _logger;

        public ApprovalWorkflowService(AttendancePlatformDbContext context, ILogger<ApprovalWorkflowService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApprovalWorkflowDto> CreateApprovalWorkflowAsync(CreateApprovalWorkflowRequestDto request)
        {
            try
            {
                var workflow = new WorkflowTemplate
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    WorkflowType = "Approval",
                    Name = request.Name,
                    Description = request.Description,
                    StepDefinitions = request.StepDefinitions ?? "[]",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                };

                _context.WorkflowTemplates.Add(workflow);
                await _context.SaveChangesAsync();

                return new ApprovalWorkflowDto
                {
                    Id = workflow.Id,
                    TenantId = workflow.TenantId,
                    Name = workflow.Name,
                    Description = workflow.Description,
                    IsActive = workflow.IsActive,
                    CreatedAt = workflow.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating approval workflow");
                throw;
            }
        }

        public async Task<List<ApprovalWorkflowDto>> GetApprovalWorkflowsAsync(Guid tenantId)
        {
            try
            {
                var workflows = await _context.WorkflowTemplates
                    .Where(w => w.TenantId == tenantId && w.WorkflowType == "Approval")
                    .Select(w => new ApprovalWorkflowDto
                    {
                        Id = w.Id,
                        TenantId = w.TenantId,
                        Name = w.Name,
                        Description = w.Description,
                        IsActive = w.IsActive,
                        CreatedAt = w.CreatedAt
                    })
                    .ToListAsync();

                return workflows;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approval workflows for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<ApprovalWorkflowDto> GetApprovalWorkflowAsync(Guid workflowId)
        {
            try
            {
                var workflow = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(w => w.Id == workflowId && w.WorkflowType == "Approval");

                if (workflow == null)
                    throw new ArgumentException($"Approval workflow with ID {workflowId} not found");

                return new ApprovalWorkflowDto
                {
                    Id = workflow.Id,
                    TenantId = workflow.TenantId,
                    Name = workflow.Name,
                    Description = workflow.Description,
                    IsActive = workflow.IsActive,
                    CreatedAt = workflow.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approval workflow {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<bool> DeleteApprovalWorkflowAsync(Guid workflowId)
        {
            try
            {
                var workflow = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(w => w.Id == workflowId && w.WorkflowType == "Approval");

                if (workflow == null)
                    return false;

                _context.WorkflowTemplates.Remove(workflow);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting approval workflow {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<ApprovalWorkflowDto> UpdateApprovalWorkflowAsync(Guid workflowId, UpdateApprovalWorkflowRequestDto request)
        {
            try
            {
                var workflow = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(w => w.Id == workflowId && w.WorkflowType == "Approval");

                if (workflow == null)
                    throw new ArgumentException($"Approval workflow with ID {workflowId} not found");

                workflow.Name = request.Name;
                workflow.Description = request.Description;
                workflow.StepDefinitions = request.StepDefinitions ?? workflow.StepDefinitions;
                workflow.IsActive = request.IsActive;
                workflow.UpdatedAt = DateTime.UtcNow;
                workflow.UpdatedBy = request.UpdatedBy;

                await _context.SaveChangesAsync();

                return new ApprovalWorkflowDto
                {
                    Id = workflow.Id,
                    TenantId = workflow.TenantId,
                    Name = workflow.Name,
                    Description = workflow.Description,
                    IsActive = workflow.IsActive,
                    CreatedAt = workflow.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating approval workflow {WorkflowId}", workflowId);
                throw;
            }
        }

        public async Task<ApprovalRequestDto> SubmitForApprovalAsync(SubmitApprovalRequestDto request)
        {
            try
            {
                var workflowInstance = new WorkflowInstance
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    WorkflowTemplateId = request.WorkflowTemplateId,
                    WorkflowType = "Approval",
                    EntityId = request.EntityId,
                    EntityType = request.EntityType,
                    InitiatedBy = request.InitiatedBy,
                    Status = "Pending",
                    Priority = request.Priority ?? "Medium",
                    InputData = request.InputData ?? "{}",
                    CurrentStepIndex = 0,
                    CurrentStep = "Initial",
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.InitiatedBy
                };

                _context.WorkflowInstances.Add(workflowInstance);
                await _context.SaveChangesAsync();

                return new ApprovalRequestDto
                {
                    Id = workflowInstance.Id,
                    TenantId = workflowInstance.TenantId,
                    WorkflowInstanceId = workflowInstance.Id,
                    WorkflowTemplateId = workflowInstance.WorkflowTemplateId,
                    EntityId = workflowInstance.EntityId,
                    EntityType = workflowInstance.EntityType,
                    InitiatedBy = workflowInstance.InitiatedBy,
                    InitiatorName = "System",
                    Status = workflowInstance.Status,
                    Priority = workflowInstance.Priority,
                    SubmittedAt = workflowInstance.StartedAt,
                    InputData = workflowInstance.InputData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting approval request");
                throw;
            }
        }

        public async Task<ApprovalRequestDto> ProcessApprovalAsync(Guid approvalId, ProcessApprovalRequestDto request)
        {
            try
            {
                var approval = await _context.WorkflowApprovals
                    .Include(a => a.WorkflowInstance)
                    .FirstOrDefaultAsync(a => a.Id == approvalId);

                if (approval == null)
                    throw new ArgumentException($"Approval with ID {approvalId} not found");

                approval.Status = request.Decision;
                approval.Comments = request.Comments;
                approval.ApprovedAt = DateTime.UtcNow;

                if (request.Decision == "Approved")
                {
                    approval.WorkflowInstance.Status = "Approved";
                    approval.WorkflowInstance.CompletedAt = DateTime.UtcNow;
                }
                else if (request.Decision == "Rejected")
                {
                    approval.WorkflowInstance.Status = "Rejected";
                    approval.WorkflowInstance.CompletedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return new ApprovalRequestDto
                {
                    Id = approval.Id,
                    TenantId = approval.WorkflowInstance.TenantId,
                    WorkflowInstanceId = approval.WorkflowInstanceId,
                    WorkflowTemplateId = approval.WorkflowInstance.WorkflowTemplateId,
                    EntityId = approval.WorkflowInstance.EntityId,
                    EntityType = approval.WorkflowInstance.EntityType,
                    InitiatedBy = approval.WorkflowInstance.InitiatedBy,
                    InitiatorName = "System",
                    Status = approval.Status,
                    Priority = approval.WorkflowInstance.Priority,
                    SubmittedAt = approval.WorkflowInstance.StartedAt,
                    ProcessedAt = approval.ApprovedAt,
                    ApproverId = approval.ApproverId,
                    ApproverName = "System",
                    Comments = approval.Comments,
                    InputData = approval.WorkflowInstance.InputData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing approval {ApprovalId}", approvalId);
                throw;
            }
        }

        public async Task<List<ApprovalRequestDto>> GetPendingApprovalsAsync(Guid tenantId, Guid? approverId = null)
        {
            try
            {
                var query = _context.WorkflowApprovals
                    .Include(a => a.WorkflowInstance)
                    .Where(a => a.WorkflowInstance.TenantId == tenantId && a.Status == "Pending");

                if (approverId.HasValue)
                {
                    query = query.Where(a => a.ApproverId == approverId.Value);
                }

                var approvals = await query
                    .Select(a => new ApprovalRequestDto
                    {
                        Id = a.Id,
                        TenantId = a.WorkflowInstance.TenantId,
                        WorkflowInstanceId = a.WorkflowInstanceId,
                        WorkflowTemplateId = a.WorkflowInstance.WorkflowTemplateId,
                        EntityType = a.WorkflowInstance.EntityType,
                        EntityId = a.WorkflowInstance.EntityId,
                        InitiatedBy = a.WorkflowInstance.InitiatedBy,
                        InitiatorName = "System",
                        Status = a.Status,
                        Priority = a.WorkflowInstance.Priority,
                        SubmittedAt = a.WorkflowInstance.StartedAt,
                        ApproverId = a.ApproverId,
                        ApproverName = "System",
                        Comments = a.Comments,
                        InputData = a.WorkflowInstance.InputData
                    })
                    .ToListAsync();

                return approvals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending approvals for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<ApprovalRequestDto>> GetApprovalHistoryAsync(Guid tenantId, Guid? entityId = null)
        {
            try
            {
                var query = _context.WorkflowApprovals
                    .Include(a => a.WorkflowInstance)
                    .Where(a => a.WorkflowInstance.TenantId == tenantId && a.Status != "Pending");

                if (entityId.HasValue)
                {
                    query = query.Where(a => a.WorkflowInstance.EntityId == entityId.Value);
                }

                var history = await query
                    .OrderByDescending(a => a.ApprovedAt)
                    .Select(a => new ApprovalRequestDto
                    {
                        Id = a.Id,
                        TenantId = a.WorkflowInstance.TenantId,
                        WorkflowInstanceId = a.WorkflowInstanceId,
                        WorkflowTemplateId = a.WorkflowInstance.WorkflowTemplateId,
                        EntityType = a.WorkflowInstance.EntityType,
                        EntityId = a.WorkflowInstance.EntityId,
                        InitiatedBy = a.WorkflowInstance.InitiatedBy,
                        InitiatorName = "System",
                        Status = a.Status,
                        Priority = a.WorkflowInstance.Priority,
                        SubmittedAt = a.WorkflowInstance.StartedAt,
                        ProcessedAt = a.ApprovedAt,
                        ApproverId = a.ApproverId,
                        ApproverName = "System",
                        Comments = a.Comments,
                        InputData = a.WorkflowInstance.InputData
                    })
                    .ToListAsync();

                return history;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approval history for tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
