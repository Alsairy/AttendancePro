using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Services
{
    public interface IComprehensiveHRService
    {
        Task<EmployeeLifecycleDto> GetEmployeeLifecycleAsync(Guid tenantId);
        Task<TalentAcquisitionDto> GetTalentAcquisitionAsync(Guid tenantId);
        Task<PerformanceManagementDto> GetPerformanceManagementAsync(Guid tenantId);
        Task<CompensationAnalysisDto> GetCompensationAnalysisAsync(Guid tenantId);
        Task<EmployeeEngagementDto> GetEmployeeEngagementAsync(Guid tenantId);
        Task<SuccessionPlanningDto> GetSuccessionPlanningAsync(Guid tenantId);
        Task<LearningDevelopmentDto> GetLearningDevelopmentAsync(Guid tenantId);
        Task<DiversityInclusionDto> GetDiversityInclusionAsync(Guid tenantId);
        Task<WorkforceAnalyticsDto> GetWorkforceAnalyticsAsync(Guid tenantId);
        Task<HRComplianceDto> GetHRComplianceAsync(Guid tenantId);
    }
}
