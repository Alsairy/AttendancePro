namespace AttendancePlatform.Shared.Domain.DTOs
{
    public class FinancialReportDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ReportType { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public DateTime GeneratedDate { get; set; }
    }

    public class BudgetAnalysisDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public double BudgetUtilization { get; set; }
        public decimal Variance { get; set; }
        public decimal ForecastedSpend { get; set; }
    }

    public class CashFlowDto
    {
        public Guid TenantId { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal CashInflows { get; set; }
        public decimal CashOutflows { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal ClosingBalance { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    public class ProfitLossDto
    {
        public Guid TenantId { get; set; }
        public decimal Revenue { get; set; }
        public decimal CostOfGoodsSold { get; set; }
        public decimal GrossProfit { get; set; }
        public decimal OperatingExpenses { get; set; }
        public decimal OperatingIncome { get; set; }
        public decimal NetIncome { get; set; }
        public string Period { get; set; } = string.Empty;
    }

    public class BalanceSheetDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalAssets { get; set; }
        public decimal CurrentAssets { get; set; }
        public decimal FixedAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal CurrentLiabilities { get; set; }
        public decimal LongTermLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public DateTime AsOfDate { get; set; }
    }

    public class FinancialTransactionDto
    {
        public Guid TenantId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
    }

    public class ExpenseReportDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class RevenueAnalysisDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RecurringRevenue { get; set; }
        public decimal OneTimeRevenue { get; set; }
        public double RevenueGrowth { get; set; }
        public decimal MonthlyRecurringRevenue { get; set; }
        public decimal AverageRevenuePerUser { get; set; }
    }

    public class TaxComplianceDto
    {
        public Guid TenantId { get; set; }
        public string ComplianceStatus { get; set; } = string.Empty;
        public DateTime LastFilingDate { get; set; }
        public DateTime NextFilingDue { get; set; }
        public decimal TaxLiability { get; set; }
        public decimal TaxesPaid { get; set; }
        public decimal OutstandingAmount { get; set; }
    }

    public class AuditTrailDto
    {
        public Guid TenantId { get; set; }
        public int TotalTransactions { get; set; }
        public int AuditedTransactions { get; set; }
        public int PendingAudits { get; set; }
        public double ComplianceScore { get; set; }
        public DateTime LastAuditDate { get; set; }
        public DateTime NextAuditDue { get; set; }
    }

    public class PurchaseOrderDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDelivery { get; set; }
    }

    public class VendorAnalysisDto
    {
        public Guid TenantId { get; set; }
        public int TotalVendors { get; set; }
        public int ActiveVendors { get; set; }
        public int PreferredVendors { get; set; }
        public double AverageRating { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal TopVendorSpend { get; set; }
        public double VendorDiversityScore { get; set; }
    }

    public class ContractManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int ExpiringContracts { get; set; }
        public decimal ContractValue { get; set; }
        public double ComplianceRate { get; set; }
        public double RenewalRate { get; set; }
    }

    public class SupplierPerformanceDto
    {
        public Guid TenantId { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public double QualityScore { get; set; }
        public double CostEfficiencyScore { get; set; }
        public double OverallPerformanceScore { get; set; }
        public int TopPerformingSuppliers { get; set; }
        public int UnderperformingSuppliers { get; set; }
    }

    public class ProcurementDashboardDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal MonthlySpend { get; set; }
        public decimal CostSavings { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessedOrders { get; set; }
        public double AverageProcessingTime { get; set; }
        public double BudgetUtilization { get; set; }
    }

    public class RfqDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string RfqNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public int ResponsesReceived { get; set; }
    }

    public class SpendAnalysisDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal DirectSpend { get; set; }
        public decimal IndirectSpend { get; set; }
        public Dictionary<string, decimal> SpendByCategory { get; set; } = new();
        public double SpendTrend { get; set; }
    }

    public class ComplianceReportDto
    {
        public Guid TenantId { get; set; }
        public double ComplianceScore { get; set; }
        public double PolicyAdherence { get; set; }
        public double DocumentationComplete { get; set; }
        public double ApprovalProcess { get; set; }
        public int AuditFindings { get; set; }
        public int CriticalIssues { get; set; }
        public int RecommendedActions { get; set; }
    }

    public class CostSavingsDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalSavings { get; set; }
        public double SavingsPercentage { get; set; }
        public decimal NegotiatedSavings { get; set; }
        public decimal ProcessImprovementSavings { get; set; }
        public decimal VolumeSavings { get; set; }
        public decimal TargetSavings { get; set; }
        public double SavingsAchievement { get; set; }
    }

    public class EmployeeLifecycleDto
    {
        public Guid TenantId { get; set; }
        public int TotalEmployees { get; set; }
        public int NewHires { get; set; }
        public int Terminations { get; set; }
        public int Promotions { get; set; }
        public int Transfers { get; set; }
        public double RetentionRate { get; set; }
        public double TurnoverRate { get; set; }
        public double AverageEmployeeTenure { get; set; }
    }

    public class TalentAcquisitionDto
    {
        public Guid TenantId { get; set; }
        public int OpenPositions { get; set; }
        public int ApplicationsReceived { get; set; }
        public int InterviewsScheduled { get; set; }
        public int OffersExtended { get; set; }
        public int OffersAccepted { get; set; }
        public double TimeToHire { get; set; }
        public decimal CostPerHire { get; set; }
        public double QualityOfHire { get; set; }
    }

    public class PerformanceManagementDto
    {
        public Guid TenantId { get; set; }
        public int CompletedReviews { get; set; }
        public int PendingReviews { get; set; }
        public double AverageRating { get; set; }
        public int HighPerformers { get; set; }
        public int LowPerformers { get; set; }
        public int GoalsSet { get; set; }
        public int GoalsAchieved { get; set; }
        public double GoalCompletionRate { get; set; }
    }

    public class CompensationAnalysisDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalPayroll { get; set; }
        public decimal AverageSalary { get; set; }
        public decimal MedianSalary { get; set; }
        public double PayEquityRatio { get; set; }
        public decimal BonusDistribution { get; set; }
        public decimal BenefitsCost { get; set; }
        public double CompensationRatio { get; set; }
        public string MarketPositioning { get; set; } = string.Empty;
    }

    public class EmployeeEngagementDto
    {
        public Guid TenantId { get; set; }
        public double EngagementScore { get; set; }
        public double SatisfactionScore { get; set; }
        public int NetPromoterScore { get; set; }
        public double ParticipationRate { get; set; }
        public int EngagedEmployees { get; set; }
        public int DisengagedEmployees { get; set; }
        public List<string> ImprovementAreas { get; set; } = new();
    }

    public class SuccessionPlanningDto
    {
        public Guid TenantId { get; set; }
        public int KeyPositions { get; set; }
        public int PositionsWithSuccessors { get; set; }
        public int ReadyNowCandidates { get; set; }
        public int ReadyIn1YearCandidates { get; set; }
        public int ReadyIn2YearsCandidates { get; set; }
        public double SuccessionCoverage { get; set; }
        public double TalentPoolDepth { get; set; }
        public int CriticalRoles { get; set; }
    }

    public class LearningDevelopmentDto
    {
        public Guid TenantId { get; set; }
        public int TrainingPrograms { get; set; }
        public int EmployeesEnrolled { get; set; }
        public double CompletionRate { get; set; }
        public int TrainingHours { get; set; }
        public decimal TrainingCost { get; set; }
        public int SkillsAssessed { get; set; }
        public int CertificationsEarned { get; set; }
        public double LearningROI { get; set; }
    }

    public class DiversityInclusionDto
    {
        public Guid TenantId { get; set; }
        public double GenderDiversity { get; set; }
        public double EthnicDiversity { get; set; }
        public double AgeDiversity { get; set; }
        public double LeadershipDiversity { get; set; }
        public double HiringDiversity { get; set; }
        public double PromotionDiversity { get; set; }
        public double InclusionScore { get; set; }
        public double PayEquityScore { get; set; }
    }

    public class WorkforceAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public double HeadcountTrend { get; set; }
        public double ProductivityIndex { get; set; }
        public double AbsenteeismRate { get; set; }
        public int OvertimeHours { get; set; }
        public double WorkforceUtilization { get; set; }
        public int SkillsGapAnalysis { get; set; }
        public int FutureWorkforceNeeds { get; set; }
        public double WorkforceFlexibility { get; set; }
    }

    public class HRComplianceDto
    {
        public Guid TenantId { get; set; }
        public double ComplianceScore { get; set; }
        public double PolicyCompliance { get; set; }
        public double TrainingCompliance { get; set; }
        public double DocumentationCompliance { get; set; }
        public int AuditFindings { get; set; }
        public int CriticalIssues { get; set; }
        public double ComplianceTraining { get; set; }
        public int RegulatoryUpdates { get; set; }
    }
}
