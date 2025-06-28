using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Workflow.Api.Services
{
    public interface IAdvancedWorkflowService
    {
        Task<WorkflowDefinitionDto> CreateWorkflowAsync(CreateWorkflowDto createWorkflowDto);
        Task<WorkflowDefinitionDto> UpdateWorkflowAsync(Guid workflowId, UpdateWorkflowDto updateWorkflowDto);
        Task<bool> DeleteWorkflowAsync(Guid workflowId);
        Task<WorkflowDefinitionDto> GetWorkflowAsync(Guid workflowId);
        Task<IEnumerable<WorkflowDefinitionDto>> GetWorkflowsAsync(Guid tenantId);
        
        Task<WorkflowInstanceDto> StartWorkflowAsync(StartWorkflowDto startWorkflowDto);
        Task<WorkflowInstanceDto> GetWorkflowInstanceAsync(Guid instanceId);
        Task<IEnumerable<WorkflowInstanceDto>> GetWorkflowInstancesAsync(Guid workflowId);
        Task<bool> CancelWorkflowInstanceAsync(Guid instanceId);
        
        Task<WorkflowStepDto> ExecuteStepAsync(Guid instanceId, Guid stepId, Dictionary<string, object> inputs);
        Task<bool> CompleteStepAsync(Guid instanceId, Guid stepId, Dictionary<string, object> outputs);
        
        Task<IEnumerable<WorkflowTemplateDto>> GetWorkflowTemplatesAsync();
        Task<WorkflowDefinitionDto> CreateWorkflowFromTemplateAsync(Guid templateId, CreateFromTemplateDto createFromTemplateDto);
        
        Task<WorkflowPerformanceDto> GetWorkflowPerformanceAsync(Guid workflowId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<WorkflowInstanceDto>> GetPendingWorkflowsAsync(Guid tenantId);
        
        Task<bool> ValidateWorkflowAsync(Guid workflowId);
        Task<WorkflowValidationResultDto> ValidateWorkflowDefinitionAsync(WorkflowDefinitionDto workflowDefinition);
        
        Task<IEnumerable<WorkflowStepDto>> GetAvailableStepsAsync();
        Task<WorkflowStepDto> CreateCustomStepAsync(CreateCustomStepDto createCustomStepDto);
        
        Task<bool> SetWorkflowVariableAsync(Guid instanceId, string variableName, object value);
        Task<object> GetWorkflowVariableAsync(Guid instanceId, string variableName);
        
        Task<IEnumerable<WorkflowAuditLogDto>> GetWorkflowAuditLogAsync(Guid instanceId);
        Task<WorkflowStatisticsDto> GetWorkflowStatisticsAsync(Guid tenantId, DateTime startDate, DateTime endDate);
    }
}
