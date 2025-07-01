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

    public class ThreatDetectionDto
    {
        public Guid TenantId { get; set; }
        public int ThreatsDetected { get; set; }
        public int ThreatsStopped { get; set; }
        public int FalsePositives { get; set; }
        public int TruePositives { get; set; }
        public double DetectionAccuracy { get; set; }
        public double ResponseTime { get; set; }
        public string ThreatSeverity { get; set; } = string.Empty;
        public int DetectionRules { get; set; }
    }

    public class VulnerabilityAssessmentDto
    {
        public Guid TenantId { get; set; }
        public int TotalVulnerabilities { get; set; }
        public int CriticalVulnerabilities { get; set; }
        public int HighVulnerabilities { get; set; }
        public int MediumVulnerabilities { get; set; }
        public int LowVulnerabilities { get; set; }
        public double VulnerabilityScore { get; set; }
        public double RemediationTime { get; set; }
        public double PatchLevel { get; set; }
        public string SecurityPosture { get; set; } = string.Empty;
    }

    public class IncidentResponseDto
    {
        public Guid TenantId { get; set; }
        public int TotalIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int PendingIncidents { get; set; }
        public double AverageResponseTime { get; set; }
        public double AverageResolutionTime { get; set; }
        public string IncidentSeverity { get; set; } = string.Empty;
        public double ResponseEffectiveness { get; set; }
        public int LessonsLearned { get; set; }
    }

    public class SecurityMonitoringDto
    {
        public Guid TenantId { get; set; }
        public double MonitoringCoverage { get; set; }
        public int AlertsGenerated { get; set; }
        public int FalsePositives { get; set; }
        public int TruePositives { get; set; }
        public double MonitoringEfficiency { get; set; }
        public int SecurityEvents { get; set; }
        public string ThreatLevel { get; set; } = string.Empty;
        public double SystemUptime { get; set; }
    }

    public class AccessControlDto
    {
        public Guid TenantId { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int PrivilegedUsers { get; set; }
        public int AccessRequests { get; set; }
        public int AccessViolations { get; set; }
        public double ComplianceScore { get; set; }
        public string PasswordPolicy { get; set; } = string.Empty;
        public double MultiFactorEnabled { get; set; }
    }

    public class SecurityTrainingDto
    {
        public Guid TenantId { get; set; }
        public int TrainingPrograms { get; set; }
        public int TrainedEmployees { get; set; }
        public int TotalEmployees { get; set; }
        public double TrainingCompletion { get; set; }
        public double PhishingTestResults { get; set; }
        public double SecurityAwareness { get; set; }
        public double TrainingEffectiveness { get; set; }
        public double CertificationRate { get; set; }
    }

    public class ComplianceManagementDto
    {
        public Guid TenantId { get; set; }
        public int ComplianceFrameworks { get; set; }
        public double ComplianceScore { get; set; }
        public int AuditFindings { get; set; }
        public int RemediationItems { get; set; }
        public double PolicyCompliance { get; set; }
        public double RegulatoryCompliance { get; set; }
        public int ComplianceGaps { get; set; }
        public DateTime NextAuditDate { get; set; }
    }

    public class SecurityReportsDto
    {
        public Guid TenantId { get; set; }
        public int SecurityReports { get; set; }
        public int ThreatReports { get; set; }
        public int ComplianceReports { get; set; }
        public int IncidentReports { get; set; }
        public int VulnerabilityReports { get; set; }
        public int RiskAssessments { get; set; }
        public double ReportAccuracy { get; set; }
        public double ReportTimeliness { get; set; }
    }

    public class SecurityMetricsDto
    {
        public Guid TenantId { get; set; }
        public double SecurityScore { get; set; }
        public double RiskScore { get; set; }
        public string ThreatLevel { get; set; } = string.Empty;
        public string SecurityPosture { get; set; } = string.Empty;
        public double SecurityROI { get; set; }
        public decimal SecurityInvestment { get; set; }
        public double SecurityEffectiveness { get; set; }
        public double SecurityMaturity { get; set; }
    }

    public class CyberThreatIntelligenceDto
    {
        public Guid TenantId { get; set; }
        public int ThreatFeeds { get; set; }
        public int ThreatIndicators { get; set; }
        public int ThreatActors { get; set; }
        public int AttackPatterns { get; set; }
        public double ThreatIntelligence { get; set; }
        public double ThreatPrediction { get; set; }
        public double ThreatHunting { get; set; }
        public double IntelligenceSharing { get; set; }
    }

    public class DataExplorationDto
    {
        public Guid TenantId { get; set; }
        public int DataSources { get; set; }
        public int DataSets { get; set; }
        public decimal DataVolume { get; set; }
        public double DataQuality { get; set; }
        public int ExplorationProjects { get; set; }
        public int InsightsGenerated { get; set; }
        public double ExplorationEfficiency { get; set; }
        public int DataPatterns { get; set; }
    }

    public class ModelDevelopmentDto
    {
        public Guid TenantId { get; set; }
        public int TotalModels { get; set; }
        public int ActiveModels { get; set; }
        public int TrainingModels { get; set; }
        public double ModelAccuracy { get; set; }
        public double ModelPerformance { get; set; }
        public int ModelVersions { get; set; }
        public double DevelopmentTime { get; set; }
        public int ModelDeployments { get; set; }
    }

    public class StatisticalAnalysisDto
    {
        public Guid TenantId { get; set; }
        public int AnalysisProjects { get; set; }
        public int CompletedAnalyses { get; set; }
        public int StatisticalTests { get; set; }
        public double AnalysisAccuracy { get; set; }
        public int HypothesesTested { get; set; }
        public int SignificantFindings { get; set; }
        public double ConfidenceLevel { get; set; }
        public int DataPoints { get; set; }
    }

    public class DataVisualizationDto
    {
        public Guid TenantId { get; set; }
        public int Dashboards { get; set; }
        public int Charts { get; set; }
        public int Reports { get; set; }
        public int InteractiveVisuals { get; set; }
        public double UserEngagement { get; set; }
        public int DataConnections { get; set; }
        public double VisualizationEffectiveness { get; set; }
        public int CustomVisuals { get; set; }
    }

    public class PredictiveModelingDto
    {
        public Guid TenantId { get; set; }
        public int PredictiveModels { get; set; }
        public double PredictionAccuracy { get; set; }
        public int ForecastsGenerated { get; set; }
        public double ModelReliability { get; set; }
        public int PredictionHorizon { get; set; }
        public double ErrorRate { get; set; }
        public int ModelUpdates { get; set; }
        public double BusinessImpact { get; set; }
    }

    public class DataMiningDto
    {
        public Guid TenantId { get; set; }
        public int MiningProjects { get; set; }
        public int PatternsDiscovered { get; set; }
        public int DataRules { get; set; }
        public double MiningAccuracy { get; set; }
        public int AssociationRules { get; set; }
        public int ClusteringResults { get; set; }
        public double KnowledgeExtraction { get; set; }
        public int AnomaliesDetected { get; set; }
    }

    public class MachineLearningDto
    {
        public Guid TenantId { get; set; }
        public int MLModels { get; set; }
        public int TrainingDatasets { get; set; }
        public double ModelAccuracy { get; set; }
        public int FeatureEngineering { get; set; }
        public int AlgorithmsUsed { get; set; }
        public double TrainingTime { get; set; }
        public int ModelValidations { get; set; }
        public double PerformanceMetrics { get; set; }
    }

    public class DataReportsDto
    {
        public Guid TenantId { get; set; }
        public int TotalReports { get; set; }
        public int ScheduledReports { get; set; }
        public int AdHocReports { get; set; }
        public double ReportAccuracy { get; set; }
        public int AutomatedReports { get; set; }
        public double ReportGeneration { get; set; }
        public int CustomReports { get; set; }
        public double UserSatisfaction { get; set; }
    }

    public class TicketManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public double AverageResolutionTime { get; set; }
        public double CustomerSatisfaction { get; set; }
        public int EscalatedTickets { get; set; }
        public double FirstCallResolution { get; set; }
        public int TicketBacklog { get; set; }
    }

    public class CustomerSatisfactionDto
    {
        public Guid TenantId { get; set; }
        public double SatisfactionScore { get; set; }
        public int SurveysCompleted { get; set; }
        public double NetPromoterScore { get; set; }
        public double CustomerRetention { get; set; }
        public int ComplaintsReceived { get; set; }
        public int ComplaintsResolved { get; set; }
        public double ServiceQuality { get; set; }
        public double ResponseTime { get; set; }
    }

    public class AgentPerformanceDto
    {
        public Guid TenantId { get; set; }
        public int TotalAgents { get; set; }
        public double AverageHandleTime { get; set; }
        public double FirstCallResolution { get; set; }
        public double CustomerSatisfaction { get; set; }
        public int CallsHandled { get; set; }
        public double ProductivityScore { get; set; }
        public double QualityScore { get; set; }
        public int TrainingHours { get; set; }
    }

    public class ServiceLevelDto
    {
        public Guid TenantId { get; set; }
        public double ServiceLevelAgreement { get; set; }
        public double ServiceLevelAchievement { get; set; }
        public double ResponseTime { get; set; }
        public double ResolutionTime { get; set; }
        public double Availability { get; set; }
        public double PerformanceMetrics { get; set; }
        public int SLAViolations { get; set; }
        public double ServiceQuality { get; set; }
    }

    public class KnowledgeBaseDto
    {
        public Guid TenantId { get; set; }
        public int TotalArticles { get; set; }
        public int PublishedArticles { get; set; }
        public int ArticleViews { get; set; }
        public double ArticleRating { get; set; }
        public int SearchQueries { get; set; }
        public double SearchAccuracy { get; set; }
        public int ArticleUpdates { get; set; }
        public double KnowledgeUtilization { get; set; }
    }

    public class EscalationManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalEscalations { get; set; }
        public int ResolvedEscalations { get; set; }
        public double EscalationRate { get; set; }
        public double AverageEscalationTime { get; set; }
        public int EscalationLevels { get; set; }
        public double EscalationResolution { get; set; }
        public int CriticalEscalations { get; set; }
        public double EscalationSatisfaction { get; set; }
    }

    public class LiveChatSupportDto
    {
        public Guid TenantId { get; set; }
        public int ChatSessions { get; set; }
        public double AverageWaitTime { get; set; }
        public double AverageChatDuration { get; set; }
        public double ChatSatisfaction { get; set; }
        public int ConcurrentChats { get; set; }
        public double ChatResolution { get; set; }
        public int ChatTransfers { get; set; }
        public double AgentUtilization { get; set; }
    }

    public class CustomerFeedbackDto
    {
        public Guid TenantId { get; set; }
        public int TotalFeedback { get; set; }
        public int FeedbackReceived { get; set; }
        public int PositiveFeedback { get; set; }
        public int NegativeFeedback { get; set; }
        public int NeutralFeedback { get; set; }
        public double FeedbackScore { get; set; }
        public int FeedbackChannels { get; set; }
        public double ResponseRate { get; set; }
        public int ActionItems { get; set; }
        public double ActionableInsights { get; set; }
        public double ImprovementRate { get; set; }
        public List<string> ImprovementAreas { get; set; } = new();
    }

    public class ServiceReportsDto
    {
        public Guid TenantId { get; set; }
        public int TotalReports { get; set; }
        public int PerformanceReports { get; set; }
        public int QualityReports { get; set; }
        public int CustomerReports { get; set; }
        public double ReportAccuracy { get; set; }
        public int AutomatedReports { get; set; }
        public double ReportTimeliness { get; set; }
        public int CustomReports { get; set; }
    }

    public class TicketDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid AssignedTo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class TechnologyRoadmapDto
    {
        public Guid TenantId { get; set; }
        public required string RoadmapNumber { get; set; }
        public required string RoadmapName { get; set; }
        public int TotalInitiatives { get; set; }
        public int TotalTechnologies { get; set; }
        public int PlannedTechnologies { get; set; }
        public int InDevelopment { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public double RoadmapProgress { get; set; }
        public double TimelineAccuracy { get; set; }
        public double ResourceAllocation { get; set; }
        public double ProjectSuccessRate { get; set; }
        public decimal TotalInvestment { get; set; }
        public double ROI { get; set; }
        public double StrategicAlignment { get; set; }
        public int InnovationIndex { get; set; }
        public string TechnologyFocus { get; set; } = string.Empty;
    }

    public class ResearchDevelopmentDto
    {
        public Guid TenantId { get; set; }
        public int ResearchProjects { get; set; }
        public int ActiveResearchers { get; set; }
        public decimal ResearchBudget { get; set; }
        public int PublishedPapers { get; set; }
        public int Patents { get; set; }
        public double ResearchEfficiency { get; set; }
        public int CollaborativeProjects { get; set; }
        public double InnovationRate { get; set; }
    }

    public class InnovationLabsDto
    {
        public Guid TenantId { get; set; }
        public int TotalLabs { get; set; }
        public int ActiveProjects { get; set; }
        public int Prototypes { get; set; }
        public int SuccessfulLaunches { get; set; }
        public decimal LabBudget { get; set; }
        public double LabUtilization { get; set; }
        public int Researchers { get; set; }
        public double InnovationScore { get; set; }
    }

    public class TechnologyAssessmentDto
    {
        public Guid TenantId { get; set; }
        public int AssessmentsCompleted { get; set; }
        public double TechnologyMaturity { get; set; }
        public double AdoptionReadiness { get; set; }
        public int RiskFactors { get; set; }
        public double CostBenefit { get; set; }
        public int StrategicAlignment { get; set; }
        public double ImplementationComplexity { get; set; }
        public string RecommendedAction { get; set; } = string.Empty;
    }

    public class PatentManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalPatents { get; set; }
        public int PendingApplications { get; set; }
        public int GrantedPatents { get; set; }
        public int PatentCitations { get; set; }
        public decimal PatentValue { get; set; }
        public int LicensingDeals { get; set; }
        public decimal LicensingRevenue { get; set; }
        public double PatentPortfolioStrength { get; set; }
    }

    public class TechnologyTransferDto
    {
        public Guid TenantId { get; set; }
        public int TransferProjects { get; set; }
        public int SuccessfulTransfers { get; set; }
        public double TransferSuccessRate { get; set; }
        public decimal TransferValue { get; set; }
        public int PartnerOrganizations { get; set; }
        public int CommercializedTechnologies { get; set; }
        public decimal RevenueGenerated { get; set; }
        public double TechnologyImpact { get; set; }
    }

    public class InnovationMetricsDto
    {
        public Guid TenantId { get; set; }
        public double InnovationIndex { get; set; }
        public int IdeaSubmissions { get; set; }
        public int ImplementedIdeas { get; set; }
        public double IdeaImplementationRate { get; set; }
        public decimal InnovationInvestment { get; set; }
        public double InnovationROI { get; set; }
        public int InnovationAwards { get; set; }
        public double CreativityScore { get; set; }
    }

    public class TechnologyReportsDto
    {
        public Guid TenantId { get; set; }
        public int TotalReports { get; set; }
        public int TechnicalReports { get; set; }
        public int ProgressReports { get; set; }
        public int AssessmentReports { get; set; }
        public double ReportAccuracy { get; set; }
        public int AutomatedReports { get; set; }
        public double ReportTimeliness { get; set; }
        public int CustomReports { get; set; }
    }

    public class EmergingTechnologiesDto
    {
        public Guid TenantId { get; set; }
        public int TrackedTechnologies { get; set; }
        public int EvaluatedTechnologies { get; set; }
        public int AdoptedTechnologies { get; set; }
        public double TechnologyAdoptionRate { get; set; }
        public int TrendAnalyses { get; set; }
        public double DisruptionPotential { get; set; }
        public int TechnologyPartnerships { get; set; }
        public double FutureTechReadiness { get; set; }
    }

    public class TechnologyPortfolioDto
    {
        public Guid TenantId { get; set; }
        public int TotalTechnologies { get; set; }
        public int CoreTechnologies { get; set; }
        public int EmergingTechnologies { get; set; }
        public int LegacyTechnologies { get; set; }
        public double PortfolioBalance { get; set; }
        public decimal PortfolioValue { get; set; }
        public double TechnologyDiversification { get; set; }
        public double StrategicAlignment { get; set; }
    }

    public class ProductRoadmapDto
    {
        public Guid TenantId { get; set; }
        public int TotalProducts { get; set; }
        public int PlannedFeatures { get; set; }
        public int CompletedFeatures { get; set; }
        public double FeatureCompletionRate { get; set; }
        public int ActiveRoadmaps { get; set; }
        public double RoadmapAccuracy { get; set; }
        public int StakeholderAlignment { get; set; }
        public double TimeToMarket { get; set; }
    }

    public class FeatureManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalFeatures { get; set; }
        public int ActiveFeatures { get; set; }
        public int DeprecatedFeatures { get; set; }
        public double FeatureAdoptionRate { get; set; }
        public int FeatureRequests { get; set; }
        public double FeatureSatisfaction { get; set; }
        public int FeatureFlags { get; set; }
        public double FeaturePerformance { get; set; }
    }

    public class MarketResearchDto
    {
        public Guid TenantId { get; set; }
        public int ResearchProjects { get; set; }
        public int MarketSegments { get; set; }
        public int CompetitorAnalyses { get; set; }
        public double MarketShare { get; set; }
        public decimal MarketSize { get; set; }
        public double GrowthRate { get; set; }
        public int CustomerInsights { get; set; }
        public double ResearchAccuracy { get; set; }
    }

    public class UserFeedbackDto
    {
        public Guid TenantId { get; set; }
        public int TotalFeedback { get; set; }
        public int PositiveFeedback { get; set; }
        public int NegativeFeedback { get; set; }
        public double SatisfactionScore { get; set; }
        public int FeatureRequests { get; set; }
        public int BugReports { get; set; }
        public double ResponseRate { get; set; }
        public double ActionableInsights { get; set; }
    }

    public class ProductMetricsDto
    {
        public Guid TenantId { get; set; }
        public int ActiveUsers { get; set; }
        public double UserEngagement { get; set; }
        public double RetentionRate { get; set; }
        public double ChurnRate { get; set; }
        public decimal Revenue { get; set; }
        public double ConversionRate { get; set; }
        public int ProductViews { get; set; }
        public double ProductPerformance { get; set; }
        public decimal ProductRevenue { get; set; }
        public double RevenueGrowth { get; set; }
        public double CustomerSatisfaction { get; set; }
        public double NetPromoterScore { get; set; }
        public double ProductAdoption { get; set; }
        public int TimeToValue { get; set; }
        public double ProductFitScore { get; set; }
        public decimal CustomerLifetimeValue { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    public class LaunchManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalLaunches { get; set; }
        public int SuccessfulLaunches { get; set; }
        public double LaunchSuccessRate { get; set; }
        public double TimeToMarket { get; set; }
        public decimal LaunchCosts { get; set; }
        public double MarketPenetration { get; set; }
        public int LaunchMetrics { get; set; }
        public double LaunchROI { get; set; }
        public int PlannedLaunches { get; set; }
        public int CompletedLaunches { get; set; }
        public double LaunchSuccess { get; set; }
        public decimal LaunchBudget { get; set; }
        public int CustomerAcquisition { get; set; }
    }

    public class ProductPortfolioDto
    {
        public Guid TenantId { get; set; }
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int RetiredProducts { get; set; }
        public decimal PortfolioRevenue { get; set; }
        public decimal PortfolioValue { get; set; }
        public double PortfolioGrowth { get; set; }
        public int ProductCategories { get; set; }
        public double PortfolioBalance { get; set; }
        public double ProductSynergy { get; set; }
        public double StrategicFit { get; set; }
        public double StrategicAlignment { get; set; }
    }

    public class DigitalMaturityDto
    {
        public Guid TenantId { get; set; }
        public double MaturityScore { get; set; }
        public int DigitalCapabilities { get; set; }
        public double TechnologyAdoption { get; set; }
        public double DigitalCulture { get; set; }
        public int DigitalInitiatives { get; set; }
        public double DigitalROI { get; set; }
        public int DigitalSkills { get; set; }
        public double TransformationProgress { get; set; }
    }

    public class AutomationAnalysisDto
    {
        public Guid TenantId { get; set; }
        public int AutomatedProcesses { get; set; }
        public double AutomationRate { get; set; }
        public decimal CostSavings { get; set; }
        public double EfficiencyGains { get; set; }
        public int AutomationOpportunities { get; set; }
        public double ROI { get; set; }
        public int ImplementedSolutions { get; set; }
        public double ProcessOptimization { get; set; }
    }

    public class CloudAdoptionDto
    {
        public Guid TenantId { get; set; }
        public double AdoptionRate { get; set; }
        public int CloudServices { get; set; }
        public decimal CloudSpend { get; set; }
        public double CostOptimization { get; set; }
        public int MigratedApplications { get; set; }
        public double CloudMaturity { get; set; }
        public int SecurityCompliance { get; set; }
        public double PerformanceGains { get; set; }
    }

    public class DataDigitizationDto
    {
        public Guid TenantId { get; set; }
        public int DigitizedDocuments { get; set; }
        public double DigitizationRate { get; set; }
        public decimal DataVolume { get; set; }
        public double DataQuality { get; set; }
        public int DataSources { get; set; }
        public double AccessibilityImprovement { get; set; }
        public int AutomatedProcesses { get; set; }
        public double DigitizationROI { get; set; }
    }

    public class ProcessOptimizationDto
    {
        public Guid TenantId { get; set; }
        public int OptimizedProcesses { get; set; }
        public double EfficiencyGains { get; set; }
        public decimal CostReduction { get; set; }
        public double TimeReduction { get; set; }
        public int ProcessImprovements { get; set; }
        public double QualityImprovement { get; set; }
        public int AutomationLevel { get; set; }
        public double ProcessMaturity { get; set; }
    }

    public class TechnologyInvestmentDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal ROI { get; set; }
        public int InvestmentProjects { get; set; }
        public double PaybackPeriod { get; set; }
        public decimal CostSavings { get; set; }
        public double RiskAssessment { get; set; }
        public int SuccessfulProjects { get; set; }
        public double InvestmentEfficiency { get; set; }
    }

    public class DigitalSkillsDto
    {
        public Guid TenantId { get; set; }
        public int TotalEmployees { get; set; }
        public int DigitallySkilled { get; set; }
        public double SkillsGapAnalysis { get; set; }
        public int TrainingPrograms { get; set; }
        public double SkillsImprovement { get; set; }
        public int Certifications { get; set; }
        public double DigitalReadiness { get; set; }
        public int SkillsAssessments { get; set; }
    }

    public class DigitalStrategyDto
    {
        public Guid TenantId { get; set; }
        public int StrategicInitiatives { get; set; }
        public double StrategyAlignment { get; set; }
        public int DigitalGoals { get; set; }
        public double GoalAchievement { get; set; }
        public decimal StrategyInvestment { get; set; }
        public double StrategyROI { get; set; }
        public int Stakeholders { get; set; }
        public double StrategyMaturity { get; set; }
    }

    public class ChangeManagementDto
    {
        public Guid TenantId { get; set; }
        public int ChangeInitiatives { get; set; }
        public double ChangeSuccessRate { get; set; }
        public int AffectedEmployees { get; set; }
        public double EmployeeAdoption { get; set; }
        public int TrainingHours { get; set; }
        public double ResistanceLevel { get; set; }
        public int CommunicationChannels { get; set; }
        public double ChangeReadiness { get; set; }
    }

    public class DigitalMetricsDto
    {
        public Guid TenantId { get; set; }
        public double DigitalScore { get; set; }
        public int DigitalKPIs { get; set; }
        public double PerformanceImprovement { get; set; }
        public int DigitalTouchpoints { get; set; }
        public double CustomerExperience { get; set; }
        public int DigitalChannels { get; set; }
        public double DigitalEfficiency { get; set; }
        public double TransformationROI { get; set; }
    }

    public class PipelineManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalOpportunities { get; set; }
        public decimal PipelineValue { get; set; }
        public decimal TotalPipelineValue { get; set; }
        public decimal WeightedPipelineValue { get; set; }
        public int QualifiedOpportunities { get; set; }
        public int WonOpportunities { get; set; }
        public int LostOpportunities { get; set; }
        public double ConversionRate { get; set; }
        public double AverageDealSize { get; set; }
        public int SalesCycle { get; set; }
        public double WinRate { get; set; }
        public int QualifiedLeads { get; set; }
        public double PipelineVelocity { get; set; }
    }

    public class LeadQualificationDto
    {
        public Guid TenantId { get; set; }
        public int TotalLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int DisqualifiedLeads { get; set; }
        public int PendingLeads { get; set; }
        public double QualificationRate { get; set; }
        public double ConversionRate { get; set; }
        public double QualificationTime { get; set; }
        public int LeadSources { get; set; }
        public double LeadQuality { get; set; }
        public int ConvertedLeads { get; set; }
        public double LeadConversionRate { get; set; }
        public double LeadScore { get; set; }
    }

    public class OpportunityManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalOpportunities { get; set; }
        public int OpenOpportunities { get; set; }
        public int WonOpportunities { get; set; }
        public int LostOpportunities { get; set; }
        public int ClosedWonOpportunities { get; set; }
        public int ClosedLostOpportunities { get; set; }
        public decimal OpportunityValue { get; set; }
        public decimal AverageOpportunitySize { get; set; }
        public double WinRate { get; set; }
        public int AverageSalesCycle { get; set; }
        public double SalesCycleLength { get; set; }
        public double OpportunityScore { get; set; }
        public double OpportunityVelocity { get; set; }
        public double ForecastAccuracy { get; set; }
    }

    public class SalesForecastingDto
    {
        public Guid TenantId { get; set; }
        public decimal ForecastRevenue { get; set; }
        public decimal ForecastedRevenue { get; set; }
        public decimal CommittedRevenue { get; set; }
        public decimal BestCaseRevenue { get; set; }
        public decimal WorstCaseRevenue { get; set; }
        public decimal ActualRevenue { get; set; }
        public double ForecastAccuracy { get; set; }
        public double QuotaAttainment { get; set; }
        public string ForecastPeriod { get; set; } = string.Empty;
        public int ForecastPeriods { get; set; }
        public double VarianceAnalysis { get; set; }
        public int PredictiveModels { get; set; }
        public double ConfidenceLevel { get; set; }
        public double TrendAnalysis { get; set; }
    }

    public class TerritoryManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalTerritories { get; set; }
        public int ActiveTerritories { get; set; }
        public decimal TerritoryRevenue { get; set; }
        public double TerritoryPerformance { get; set; }
        public int SalesReps { get; set; }
        public double TerritoryBalance { get; set; }
        public int CustomerAccounts { get; set; }
        public double MarketPenetration { get; set; }
    }

    public class SalesPerformanceDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalRevenue { get; set; }
        public double RevenueGrowth { get; set; }
        public int SalesTeamSize { get; set; }
        public int TotalSalesReps { get; set; }
        public int TopPerformers { get; set; }
        public int QuotaAttainers { get; set; }
        public double AverageQuotaAttainment { get; set; }
        public double SalesProductivity { get; set; }
        public double ActivityMetrics { get; set; }
        public double PerformanceScore { get; set; }
        public double AveragePerformance { get; set; }
        public int QuotaAchievers { get; set; }
        public double QuotaAttainment { get; set; }
        public int SalesActivities { get; set; }
        public double SalesEfficiency { get; set; }
    }

    public class CommissionManagementDto
    {
        public Guid TenantId { get; set; }
        public decimal TotalCommissions { get; set; }
        public decimal PaidCommissions { get; set; }
        public decimal PendingCommissions { get; set; }
        public double CommissionRate { get; set; }
        public decimal TopEarner { get; set; }
        public int CommissionPlans { get; set; }
        public double AverageCommission { get; set; }
        public int EligibleSalesReps { get; set; }
        public double CommissionAccuracy { get; set; }
        public int PaymentCycles { get; set; }
        public double CommissionROI { get; set; }
        public double SalesMotivation { get; set; }
    }

    public class SalesReportsDto
    {
        public Guid TenantId { get; set; }
        public int TotalReports { get; set; }
        public int PerformanceReports { get; set; }
        public int ForecastReports { get; set; }
        public int ActivityReports { get; set; }
        public double ReportAccuracy { get; set; }
        public int AutomatedReports { get; set; }
        public double ReportTimeliness { get; set; }
        public int CustomReports { get; set; }
    }

    public class CampaignManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalCampaigns { get; set; }
        public int ActiveCampaigns { get; set; }
        public int CompletedCampaigns { get; set; }
        public decimal CampaignBudget { get; set; }
        public double CampaignROI { get; set; }
        public int LeadsGenerated { get; set; }
        public double ConversionRate { get; set; }
        public double CampaignEffectiveness { get; set; }
    }

    public class LeadManagementDto
    {
        public Guid TenantId { get; set; }
        public int TotalLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public double LeadConversionRate { get; set; }
        public int LeadSources { get; set; }
        public double LeadQuality { get; set; }
        public int NurturedLeads { get; set; }
        public double LeadVelocity { get; set; }
    }

    public class ContentMarketingDto
    {
        public Guid TenantId { get; set; }
        public int ContentPieces { get; set; }
        public int PublishedContent { get; set; }
        public int ContentViews { get; set; }
        public double EngagementRate { get; set; }
        public int ContentShares { get; set; }
        public double ContentROI { get; set; }
        public int ContentChannels { get; set; }
        public double ContentEffectiveness { get; set; }
    }

    public class SocialMediaDto
    {
        public Guid TenantId { get; set; }
        public int SocialPlatforms { get; set; }
        public int Followers { get; set; }
        public int Posts { get; set; }
        public double EngagementRate { get; set; }
        public int Shares { get; set; }
        public int Comments { get; set; }
        public double ReachRate { get; set; }
        public double SocialROI { get; set; }
    }

    public class EmailMarketingDto
    {
        public Guid TenantId { get; set; }
        public int EmailCampaigns { get; set; }
        public int EmailsSent { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
        public double ConversionRate { get; set; }
        public int Subscribers { get; set; }
        public double UnsubscribeRate { get; set; }
        public double EmailROI { get; set; }
    }

    public class MarketingAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int AnalyticsReports { get; set; }
        public double AttributionAccuracy { get; set; }
        public int TouchpointAnalysis { get; set; }
        public double CustomerJourney { get; set; }
        public int ConversionPaths { get; set; }
        public double MarketingROI { get; set; }
        public int PerformanceMetrics { get; set; }
        public double DataAccuracy { get; set; }
    }

    public class BrandManagementDto
    {
        public Guid TenantId { get; set; }
        public double BrandAwareness { get; set; }
        public double BrandSentiment { get; set; }
        public int BrandMentions { get; set; }
        public double BrandEquity { get; set; }
        public int BrandCampaigns { get; set; }
        public double BrandConsistency { get; set; }
        public int BrandAssets { get; set; }
        public double BrandPerformance { get; set; }
    }

    public class CampaignDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Budget { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CampaignType { get; set; } = string.Empty;
        public int TargetAudience { get; set; }
    }

    public class InnovationPipelineDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Stage { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double Budget { get; set; }
        public double Progress { get; set; }
        public List<string> TeamMembers { get; set; } = new();
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    public class TechnologyScoutingDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string TechnologyName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string MaturityLevel { get; set; } = string.Empty;
        public double PotentialImpact { get; set; }
        public double ImplementationCost { get; set; }
        public DateTime DiscoveryDate { get; set; }
        public string Source { get; set; } = string.Empty;
        public List<string> Applications { get; set; } = new();
        public Dictionary<string, object> TechnicalSpecs { get; set; } = new();
    }

    public class StartupPartnershipsDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string StartupName { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string PartnershipType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public double InvestmentAmount { get; set; }
        public double EquityPercentage { get; set; }
        public List<string> CollaborationAreas { get; set; } = new();
        public Dictionary<string, object> Milestones { get; set; } = new();
    }

    public class IdeaManagementDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid SubmittedBy { get; set; }
        public DateTime SubmissionDate { get; set; }
        public double Rating { get; set; }
        public int Votes { get; set; }
        public List<string> Tags { get; set; } = new();
        public Dictionary<string, object> Evaluation { get; set; } = new();
    }

    public class InnovationCultureDto
    {
        public Guid TenantId { get; set; }
        public double CultureScore { get; set; }
        public int EmployeeParticipation { get; set; }
        public int InnovationEvents { get; set; }
        public double TrainingHours { get; set; }
        public int CrossFunctionalProjects { get; set; }
        public double FailureToleranceIndex { get; set; }
        public List<string> CultureInitiatives { get; set; } = new();
        public Dictionary<string, double> CultureMetrics { get; set; } = new();
    }

    public class InnovationROIDto
    {
        public Guid TenantId { get; set; }
        public double TotalInvestment { get; set; }
        public double TotalReturns { get; set; }
        public double ROIPercentage { get; set; }
        public double PaybackPeriod { get; set; }
        public double NetPresentValue { get; set; }
        public double InternalRateOfReturn { get; set; }
        public List<string> RevenueStreams { get; set; } = new();
        public Dictionary<string, double> CostBreakdown { get; set; } = new();
    }

    public class ServiceAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalInteractions { get; set; }
        public Dictionary<string, int> ChannelDistribution { get; set; } = new();
        public List<string> PeakHours { get; set; } = new();
        public double ResolutionRate { get; set; }
        public double EscalationRate { get; set; }
        public double CustomerRetentionRate { get; set; }
        public double AverageResponseTime { get; set; }
        public double CustomerSatisfactionScore { get; set; }
        public int TotalTickets { get; set; }
        public Dictionary<string, object> Metrics { get; set; } = new();
    }

    public class DataQualityDto
    {
        public required string QualityNumber { get; set; }
        public required string DataAssetName { get; set; }
        public required string QualityDimension { get; set; }
        public required string QualityStatus { get; set; }
        public required string QualityTrend { get; set; }
        public required string ResponsibleTeam { get; set; }
        public required string Status { get; set; }
        public Guid TenantId { get; set; }
        public double QualityScore { get; set; }
        public double DataCompleteness { get; set; }
        public double DataAccuracy { get; set; }
        public double DataConsistency { get; set; }
        public double DataValidity { get; set; }
        public double DataUniqueness { get; set; }
        public int QualityChecks { get; set; }
        public Dictionary<string, object> QualityMetrics { get; set; } = new();
    }

    public class DataGovernanceDto
    {
        public Guid TenantId { get; set; }
        public int TotalPolicies { get; set; }
        public int GovernancePolicies { get; set; }
        public int DataStewards { get; set; }
        public int DataCatalogs { get; set; }
        public string DataLineage { get; set; } = string.Empty;
        public Dictionary<string, int> AccessControls { get; set; } = new();
        public double ComplianceScore { get; set; }
        public double GovernanceMaturity { get; set; }
        public double PolicyCompliance { get; set; }
        public Dictionary<string, object> GovernanceMetrics { get; set; } = new();
    }

}
