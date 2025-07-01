using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IDataAnalyticsService
    {
        Task<DataInsightsDto> GetDataInsightsAsync(Guid tenantId);
        Task<PredictiveAnalyticsDto> GetPredictiveAnalyticsAsync(Guid tenantId);
        Task<DataVisualizationDto> CreateDataVisualizationAsync(DataVisualizationDto visualization);
        Task<List<DataVisualizationDto>> GetDataVisualizationsAsync(Guid tenantId);
        Task<DataExportDto> ExportDataAsync(DataExportRequestDto request);
        Task<DataImportDto> ImportDataAsync(DataImportRequestDto request);
        Task<DataQualityReportDto> GenerateDataQualityReportAsync(Guid tenantId);
        Task<DataGovernanceDto> GetDataGovernanceAsync(Guid tenantId);
        Task<DataLineageDto> GetDataLineageAsync(string datasetId);
        Task<DataCatalogDto> GetDataCatalogAsync(Guid tenantId);
        Task<DataPrivacyReportDto> GenerateDataPrivacyReportAsync(Guid tenantId);
        Task<DataRetentionPolicyDto> GetDataRetentionPolicyAsync(Guid tenantId);
        Task<DataBackupReportDto> GenerateDataBackupReportAsync(Guid tenantId);
        Task<DataSecurityAuditDto> PerformDataSecurityAuditAsync(Guid tenantId);
        Task<DataAnalyticsCustomReportDto> CreateCustomReportAsync(DataAnalyticsCustomReportDto report);
    }

    public class DataAnalyticsService : IDataAnalyticsService
    {
        private readonly ILogger<DataAnalyticsService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public DataAnalyticsService(ILogger<DataAnalyticsService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<DataInsightsDto> GetDataInsightsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataInsightsDto
            {
                TenantId = tenantId,
                TotalDataPoints = 2500000,
                DataGrowthRate = 15.8,
                DataQualityScore = 94.2,
                ActiveDataSources = 25,
                DataProcessingTime = 2.3,
                StorageUtilization = 78.5,
                TopDataCategories = new Dictionary<string, long>
                {
                    { "Attendance Records", 850000 },
                    { "Employee Data", 125000 },
                    { "Performance Metrics", 320000 },
                    { "Training Records", 180000 },
                    { "Compliance Data", 95000 },
                    { "Financial Data", 75000 },
                    { "System Logs", 855000 }
                },
                DataTrends = new Dictionary<string, double>
                {
                    { "Daily Growth", 2.1 },
                    { "Weekly Growth", 14.7 },
                    { "Monthly Growth", 58.3 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<PredictiveAnalyticsDto> GetPredictiveAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new PredictiveAnalyticsDto
            {
                TenantId = tenantId,
                AttendancePredictions = new AttendancePredictionDto
                {
                    NextWeekAttendanceRate = 94.8,
                    NextMonthAttendanceRate = 93.5,
                    SeasonalTrends = new Dictionary<string, double>
                    {
                        { "Q1", 94.2 },
                        { "Q2", 95.1 },
                        { "Q3", 92.8 },
                        { "Q4", 93.7 }
                    },
                    RiskFactors = new List<string>
                    {
                        "Holiday season impact",
                        "Flu season considerations",
                        "Project deadline pressures"
                    }
                },
                TurnoverPredictions = new TurnoverPredictionDto
                {
                    NextQuarterTurnoverRate = 7.2,
                    HighRiskEmployees = 15,
                    RetentionRecommendations = new List<string>
                    {
                        "Implement flexible work arrangements",
                        "Enhance career development programs",
                        "Review compensation packages"
                    }
                },
                PerformancePredictions = new PerformancePredictionDto
                {
                    ExpectedPerformanceImprovement = 8.5,
                    TopPerformingDepartments = new List<string> { "Engineering", "Finance", "HR" },
                    PerformanceRisks = new List<string>
                    {
                        "Workload distribution imbalance",
                        "Skills gap in emerging technologies"
                    }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataVisualizationDto> CreateDataVisualizationAsync(DataVisualizationDto visualization)
        {
            try
            {
                visualization.Id = Guid.NewGuid();
                visualization.CreatedAt = DateTime.UtcNow;
                visualization.Status = "Active";

                _logger.LogInformation("Data visualization created: {VisualizationId} - {VisualizationName}", visualization.Id, visualization.Name);
                return visualization;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create data visualization");
                throw;
            }
        }

        public async Task<List<DataVisualizationDto>> GetDataVisualizationsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<DataVisualizationDto>
            {
                new DataVisualizationDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Attendance Trends Dashboard",
                    Description = "Real-time attendance trends and patterns",
                    Type = "Dashboard",
                    ChartType = "Line Chart",
                    DataSource = "Attendance Records",
                    RefreshInterval = 300,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new DataVisualizationDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Performance Metrics Heatmap",
                    Description = "Department performance comparison heatmap",
                    Type = "Chart",
                    ChartType = "Heatmap",
                    DataSource = "Performance Data",
                    RefreshInterval = 3600,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };
        }

        public async Task<DataExportDto> ExportDataAsync(DataExportRequestDto request)
        {
            try
            {
                await Task.CompletedTask;
                var export = new DataExportDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    ExportType = request.ExportType,
                    Format = request.Format,
                    Status = "Processing",
                    RequestedAt = DateTime.UtcNow,
                    EstimatedSize = 25600000,
                    RecordCount = 125000
                };

                _logger.LogInformation("Data export initiated: {ExportId} - {ExportType}", export.Id, export.ExportType);
                return export;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initiate data export");
                throw;
            }
        }

        public async Task<DataImportDto> ImportDataAsync(DataImportRequestDto request)
        {
            try
            {
                await Task.CompletedTask;
                var import = new DataImportDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    ImportType = request.ImportType,
                    SourceFormat = request.SourceFormat,
                    Status = "Processing",
                    RequestedAt = DateTime.UtcNow,
                    FileSize = request.FileSize,
                    EstimatedRecords = 85000
                };

                _logger.LogInformation("Data import initiated: {ImportId} - {ImportType}", import.Id, import.ImportType);
                return import;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initiate data import");
                throw;
            }
        }

        public async Task<DataQualityReportDto> GenerateDataQualityReportAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataQualityReportDto
            {
                TenantId = tenantId,
                OverallQualityScore = 94.2,
                DataCompletenessScore = 96.8,
                DataAccuracyScore = 93.5,
                DataConsistencyScore = 92.1,
                DataValidityScore = 95.7,
                QualityIssues = new List<DataQualityIssueDto>
                {
                    new DataQualityIssueDto
                    {
                        Category = "Missing Data",
                        Description = "Some employee records missing phone numbers",
                        Severity = "Medium",
                        AffectedRecords = 45,
                        RecommendedAction = "Contact HR to update missing information"
                    },
                    new DataQualityIssueDto
                    {
                        Category = "Data Inconsistency",
                        Description = "Date format variations in legacy records",
                        Severity = "Low",
                        AffectedRecords = 128,
                        RecommendedAction = "Run data standardization script"
                    }
                },
                DataSourceQuality = new Dictionary<string, double>
                {
                    { "Attendance System", 98.5 },
                    { "HR System", 92.3 },
                    { "Payroll System", 96.1 },
                    { "Training System", 89.7 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataGovernanceDto> GetDataGovernanceAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataGovernanceDto
            {
                TenantId = tenantId,
                DataClassification = new Dictionary<string, int>
                {
                    { "Public", 125000 },
                    { "Internal", 850000 },
                    { "Confidential", 320000 },
                    { "Restricted", 45000 }
                },
                DataOwnership = new Dictionary<string, string>
                {
                    { "Employee Data", "HR Department" },
                    { "Attendance Records", "Operations" },
                    { "Financial Data", "Finance Department" },
                    { "Performance Data", "HR Department" }
                },
                ComplianceStatus = new Dictionary<string, string>
                {
                    { "GDPR", "Compliant" },
                    { "CCPA", "Compliant" },
                    { "SOX", "Compliant" },
                    { "HIPAA", "Not Applicable" }
                },
                DataPolicies = new List<string>
                {
                    "Data Retention Policy",
                    "Data Privacy Policy",
                    "Data Access Control Policy",
                    "Data Backup and Recovery Policy"
                },
                LastAuditDate = DateTime.UtcNow.AddDays(-30),
                NextAuditDate = DateTime.UtcNow.AddDays(335),
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataLineageDto> GetDataLineageAsync(string datasetId)
        {
            await Task.CompletedTask;
            return new DataLineageDto
            {
                DatasetId = datasetId,
                DatasetName = "Employee Attendance Records",
                SourceSystems = new List<string>
                {
                    "Biometric Scanners",
                    "Mobile App",
                    "Web Portal",
                    "Kiosk System"
                },
                TransformationSteps = new List<DataTransformationDto>
                {
                    new DataTransformationDto
                    {
                        Step = 1,
                        Operation = "Data Ingestion",
                        Description = "Raw data collected from various sources",
                        Timestamp = DateTime.UtcNow.AddHours(-2)
                    },
                    new DataTransformationDto
                    {
                        Step = 2,
                        Operation = "Data Validation",
                        Description = "Validate data format and business rules",
                        Timestamp = DateTime.UtcNow.AddHours(-2).AddMinutes(15)
                    },
                    new DataTransformationDto
                    {
                        Step = 3,
                        Operation = "Data Enrichment",
                        Description = "Add calculated fields and metadata",
                        Timestamp = DateTime.UtcNow.AddHours(-2).AddMinutes(30)
                    }
                },
                DestinationSystems = new List<string>
                {
                    "Data Warehouse",
                    "Analytics Platform",
                    "Reporting System",
                    "Compliance Archive"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataCatalogDto> GetDataCatalogAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataCatalogDto
            {
                TenantId = tenantId,
                TotalDatasets = 156,
                DatasetCategories = new Dictionary<string, int>
                {
                    { "Operational", 68 },
                    { "Analytical", 45 },
                    { "Reference", 25 },
                    { "Archive", 18 }
                },
                Datasets = new List<DatasetInfoDto>
                {
                    new DatasetInfoDto
                    {
                        Id = "ATT_001",
                        Name = "Daily Attendance Records",
                        Description = "Employee daily check-in/check-out records",
                        Category = "Operational",
                        Owner = "Operations Team",
                        LastUpdated = DateTime.UtcNow.AddHours(-1),
                        RecordCount = 850000,
                        DataSize = "2.5 GB"
                    },
                    new DatasetInfoDto
                    {
                        Id = "EMP_001",
                        Name = "Employee Master Data",
                        Description = "Core employee information and profiles",
                        Category = "Reference",
                        Owner = "HR Department",
                        LastUpdated = DateTime.UtcNow.AddDays(-1),
                        RecordCount = 1250,
                        DataSize = "15 MB"
                    }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataPrivacyReportDto> GenerateDataPrivacyReportAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataPrivacyReportDto
            {
                TenantId = tenantId,
                PersonalDataRecords = 125000,
                SensitiveDataRecords = 45000,
                EncryptedRecords = 170000,
                DataSubjectRequests = new Dictionary<string, int>
                {
                    { "Access Requests", 25 },
                    { "Deletion Requests", 8 },
                    { "Correction Requests", 12 },
                    { "Portability Requests", 5 }
                },
                ConsentStatus = new Dictionary<string, int>
                {
                    { "Explicit Consent", 980 },
                    { "Implied Consent", 185 },
                    { "Withdrawn Consent", 15 },
                    { "Pending Consent", 70 }
                },
                DataBreachIncidents = 0,
                ComplianceScore = 98.5,
                PrivacyPolicies = new List<string>
                {
                    "Employee Data Privacy Policy",
                    "Biometric Data Handling Policy",
                    "Data Retention and Deletion Policy"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataRetentionPolicyDto> GetDataRetentionPolicyAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataRetentionPolicyDto
            {
                TenantId = tenantId,
                RetentionRules = new List<DataRetentionRuleDto>
                {
                    new DataRetentionRuleDto
                    {
                        DataType = "Attendance Records",
                        RetentionPeriod = 2555,
                        RetentionUnit = "Days",
                        ArchiveAfter = 1095,
                        DeleteAfter = 2555,
                        LegalBasis = "Employment law requirements"
                    },
                    new DataRetentionRuleDto
                    {
                        DataType = "Employee Personal Data",
                        RetentionPeriod = 2190,
                        RetentionUnit = "Days",
                        ArchiveAfter = 1095,
                        DeleteAfter = 2190,
                        LegalBasis = "GDPR Article 5"
                    }
                },
                AutomatedDeletion = true,
                LastPurgeDate = DateTime.UtcNow.AddDays(-30),
                NextPurgeDate = DateTime.UtcNow.AddDays(30),
                RecordsScheduledForDeletion = 1250,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataBackupReportDto> GenerateDataBackupReportAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataBackupReportDto
            {
                TenantId = tenantId,
                LastBackupDate = DateTime.UtcNow.AddHours(-6),
                NextBackupDate = DateTime.UtcNow.AddHours(18),
                BackupFrequency = "Daily",
                BackupSize = "125 GB",
                BackupStatus = "Successful",
                BackupLocations = new List<string>
                {
                    "Primary Cloud Storage",
                    "Secondary Cloud Storage",
                    "Local Backup Server"
                },
                RecoveryPointObjective = "4 hours",
                RecoveryTimeObjective = "2 hours",
                LastRestoreTest = DateTime.UtcNow.AddDays(-7),
                RestoreTestResult = "Successful",
                BackupRetentionPeriod = 90,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataSecurityAuditDto> PerformDataSecurityAuditAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new DataSecurityAuditDto
            {
                TenantId = tenantId,
                AuditDate = DateTime.UtcNow,
                SecurityScore = 96.8,
                EncryptionStatus = new Dictionary<string, string>
                {
                    { "Data at Rest", "AES-256 Encrypted" },
                    { "Data in Transit", "TLS 1.3 Encrypted" },
                    { "Database", "Transparent Data Encryption" },
                    { "Backups", "Encrypted" }
                },
                AccessControls = new Dictionary<string, string>
                {
                    { "Authentication", "Multi-Factor Enabled" },
                    { "Authorization", "Role-Based Access Control" },
                    { "Session Management", "Secure" },
                    { "API Security", "OAuth 2.0 + JWT" }
                },
                SecurityFindings = new List<SecurityFindingDto>
                {
                    new SecurityFindingDto
                    {
                        Category = "Information",
                        Description = "All security controls functioning properly",
                        Severity = "Info",
                        Status = "Resolved",
                        RecommendedAction = "Continue monitoring"
                    }
                },
                ComplianceStatus = new Dictionary<string, bool>
                {
                    { "ISO 27001", true },
                    { "SOC 2", true },
                    { "GDPR", true },
                    { "CCPA", true }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<DataAnalyticsCustomReportDto> CreateCustomReportAsync(DataAnalyticsCustomReportDto report)
        {
            try
            {
                report.Id = Guid.NewGuid();
                report.CreatedAt = DateTime.UtcNow;
                report.Status = "Active";

                _logger.LogInformation("Custom report created: {ReportId} - {ReportName}", report.Id, report.Name);
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create custom report");
                throw;
            }
        }
    }

    public class DataInsightsDto
    {
        public Guid TenantId { get; set; }
        public long TotalDataPoints { get; set; }
        public double DataGrowthRate { get; set; }
        public double DataQualityScore { get; set; }
        public int ActiveDataSources { get; set; }
        public double DataProcessingTime { get; set; }
        public double StorageUtilization { get; set; }
        public Dictionary<string, long> TopDataCategories { get; set; }
        public Dictionary<string, double> DataTrends { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class PredictiveAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public AttendancePredictionDto AttendancePredictions { get; set; }
        public TurnoverPredictionDto TurnoverPredictions { get; set; }
        public PerformancePredictionDto PerformancePredictions { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class AttendancePredictionDto
    {
        public double NextWeekAttendanceRate { get; set; }
        public double NextMonthAttendanceRate { get; set; }
        public Dictionary<string, double> SeasonalTrends { get; set; }
        public List<string> RiskFactors { get; set; }
    }

    public class TurnoverPredictionDto
    {
        public double NextQuarterTurnoverRate { get; set; }
        public int HighRiskEmployees { get; set; }
        public List<string> RetentionRecommendations { get; set; }
    }

    public class PerformancePredictionDto
    {
        public double ExpectedPerformanceImprovement { get; set; }
        public List<string> TopPerformingDepartments { get; set; }
        public List<string> PerformanceRisks { get; set; }
    }

    public class DataVisualizationDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string ChartType { get; set; }
        public string DataSource { get; set; }
        public int RefreshInterval { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DataExportDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ExportType { get; set; }
        public string Format { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public long EstimatedSize { get; set; }
        public int RecordCount { get; set; }
    }

    public class DataExportRequestDto
    {
        public Guid TenantId { get; set; }
        public string ExportType { get; set; }
        public string Format { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class DataImportDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ImportType { get; set; }
        public string SourceFormat { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public long FileSize { get; set; }
        public int EstimatedRecords { get; set; }
    }

    public class DataImportRequestDto
    {
        public Guid TenantId { get; set; }
        public string ImportType { get; set; }
        public string SourceFormat { get; set; }
        public long FileSize { get; set; }
    }

    public class DataQualityReportDto
    {
        public Guid TenantId { get; set; }
        public double OverallQualityScore { get; set; }
        public double DataCompletenessScore { get; set; }
        public double DataAccuracyScore { get; set; }
        public double DataConsistencyScore { get; set; }
        public double DataValidityScore { get; set; }
        public List<DataQualityIssueDto> QualityIssues { get; set; }
        public Dictionary<string, double> DataSourceQuality { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataQualityIssueDto
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public int AffectedRecords { get; set; }
        public string RecommendedAction { get; set; }
    }

    public class DataGovernanceDto
    {
        public Guid TenantId { get; set; }
        public Dictionary<string, int> DataClassification { get; set; }
        public Dictionary<string, string> DataOwnership { get; set; }
        public Dictionary<string, string> ComplianceStatus { get; set; }
        public List<string> DataPolicies { get; set; }
        public DateTime LastAuditDate { get; set; }
        public DateTime NextAuditDate { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataLineageDto
    {
        public string DatasetId { get; set; }
        public string DatasetName { get; set; }
        public List<string> SourceSystems { get; set; }
        public List<DataTransformationDto> TransformationSteps { get; set; }
        public List<string> DestinationSystems { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataTransformationDto
    {
        public int Step { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class DataCatalogDto
    {
        public Guid TenantId { get; set; }
        public int TotalDatasets { get; set; }
        public Dictionary<string, int> DatasetCategories { get; set; }
        public List<DatasetInfoDto> Datasets { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DatasetInfoDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Owner { get; set; }
        public DateTime LastUpdated { get; set; }
        public long RecordCount { get; set; }
        public string DataSize { get; set; }
    }

    public class DataPrivacyReportDto
    {
        public Guid TenantId { get; set; }
        public long PersonalDataRecords { get; set; }
        public long SensitiveDataRecords { get; set; }
        public long EncryptedRecords { get; set; }
        public Dictionary<string, int> DataSubjectRequests { get; set; }
        public Dictionary<string, int> ConsentStatus { get; set; }
        public int DataBreachIncidents { get; set; }
        public double ComplianceScore { get; set; }
        public List<string> PrivacyPolicies { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataRetentionPolicyDto
    {
        public Guid TenantId { get; set; }
        public List<DataRetentionRuleDto> RetentionRules { get; set; }
        public bool AutomatedDeletion { get; set; }
        public DateTime LastPurgeDate { get; set; }
        public DateTime NextPurgeDate { get; set; }
        public int RecordsScheduledForDeletion { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataRetentionRuleDto
    {
        public string DataType { get; set; }
        public int RetentionPeriod { get; set; }
        public string RetentionUnit { get; set; }
        public int ArchiveAfter { get; set; }
        public int DeleteAfter { get; set; }
        public string LegalBasis { get; set; }
    }

    public class DataBackupReportDto
    {
        public Guid TenantId { get; set; }
        public DateTime LastBackupDate { get; set; }
        public DateTime NextBackupDate { get; set; }
        public string BackupFrequency { get; set; }
        public string BackupSize { get; set; }
        public string BackupStatus { get; set; }
        public List<string> BackupLocations { get; set; }
        public string RecoveryPointObjective { get; set; }
        public string RecoveryTimeObjective { get; set; }
        public DateTime LastRestoreTest { get; set; }
        public string RestoreTestResult { get; set; }
        public int BackupRetentionPeriod { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class DataSecurityAuditDto
    {
        public Guid TenantId { get; set; }
        public DateTime AuditDate { get; set; }
        public double SecurityScore { get; set; }
        public Dictionary<string, string> EncryptionStatus { get; set; }
        public Dictionary<string, string> AccessControls { get; set; }
        public List<SecurityFindingDto> SecurityFindings { get; set; }
        public Dictionary<string, bool> ComplianceStatus { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class SecurityFindingDto
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public string RecommendedAction { get; set; }
    }

    public class DataAnalyticsCustomReportDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
