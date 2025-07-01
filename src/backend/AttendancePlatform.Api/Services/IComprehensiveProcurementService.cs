using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Services
{
    public interface IComprehensiveProcurementService
    {
        Task<ProcurementDashboardDto> GetProcurementDashboardAsync(Guid tenantId);
        Task<List<PurchaseOrderDto>> GetPurchaseOrdersAsync(Guid tenantId);
        Task<VendorAnalysisDto> GetVendorAnalysisAsync(Guid tenantId);
        Task<ContractManagementDto> GetContractManagementAsync(Guid tenantId);
        Task<SupplierPerformanceDto> GetSupplierPerformanceAsync(Guid tenantId);
        Task<SpendAnalysisDto> GetSpendAnalysisAsync(Guid tenantId);
        Task<List<RfqDto>> GetRequestForQuotationsAsync(Guid tenantId);
        Task<ComplianceReportDto> GetProcurementComplianceAsync(Guid tenantId);
        Task<CostSavingsDto> GetCostSavingsAnalysisAsync(Guid tenantId);
        Task<bool> CreatePurchaseOrderAsync(PurchaseOrderDto purchaseOrder);
    }
}
