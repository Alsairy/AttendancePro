using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Services
{
    public interface IComprehensiveFinanceService
    {
        Task<FinancialReportDto> GetFinancialDashboardAsync(Guid tenantId);
        Task<List<FinancialReportDto>> GetFinancialReportsAsync(Guid tenantId);
        Task<BudgetAnalysisDto> GetBudgetAnalysisAsync(Guid tenantId);
        Task<CashFlowDto> GetCashFlowAnalysisAsync(Guid tenantId);
        Task<ProfitLossDto> GetProfitLossStatementAsync(Guid tenantId);
        Task<BalanceSheetDto> GetBalanceSheetAsync(Guid tenantId);
        Task<bool> CreateFinancialTransactionAsync(FinancialTransactionDto transaction);
        Task<List<ExpenseReportDto>> GetExpenseReportsAsync(Guid tenantId);
        Task<RevenueAnalysisDto> GetRevenueAnalysisAsync(Guid tenantId);
        Task<TaxComplianceDto> GetTaxComplianceStatusAsync(Guid tenantId);
        Task<AuditTrailDto> GetFinancialAuditTrailAsync(Guid tenantId);
    }
}
