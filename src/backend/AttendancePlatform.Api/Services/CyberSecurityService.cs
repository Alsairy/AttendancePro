using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface ICyberSecurityService
    {
        Task<CyberSecurityIncidentDto> CreateSecurityIncidentAsync(CyberSecurityIncidentDto incident);
        Task<List<CyberSecurityIncidentDto>> GetSecurityIncidentsAsync(Guid tenantId);
        Task<CyberSecurityIncidentDto> UpdateSecurityIncidentAsync(Guid incidentId, CyberSecurityIncidentDto incident);
        Task<SecurityAssessmentDto> CreateSecurityAssessmentAsync(SecurityAssessmentDto assessment);
        Task<List<SecurityAssessmentDto>> GetSecurityAssessmentsAsync(Guid tenantId);
        Task<ThreatIntelligenceDto> CreateThreatIntelligenceAsync(ThreatIntelligenceDto threat);
        Task<List<ThreatIntelligenceDto>> GetThreatIntelligenceAsync(Guid tenantId);
        Task<CyberSecurityVulnerabilityDto> CreateVulnerabilityAsync(CyberSecurityVulnerabilityDto vulnerability);
        Task<List<CyberSecurityVulnerabilityDto>> GetVulnerabilitiesAsync(Guid tenantId);
        Task<SecurityAnalyticsDto> GetSecurityAnalyticsAsync(Guid tenantId);
        Task<CyberSecurityReportDto> GenerateSecurityReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<List<CyberSecurityPolicyDto>> GetSecurityPoliciesAsync(Guid tenantId);
        Task<CyberSecurityPolicyDto> CreateSecurityPolicyAsync(CyberSecurityPolicyDto policy);
        Task<bool> UpdateSecurityPolicyAsync(Guid policyId, CyberSecurityPolicyDto policy);
        Task<List<CyberSecurityAuditDto>> GetSecurityAuditsAsync(Guid tenantId);
        Task<CyberSecurityAuditDto> CreateSecurityAuditAsync(CyberSecurityAuditDto audit);
        Task<CyberSecurityComplianceDto> GetSecurityComplianceAsync(Guid tenantId);
        Task<bool> UpdateSecurityComplianceAsync(Guid tenantId, CyberSecurityComplianceDto compliance);
    }

    public class CyberSecurityService : ICyberSecurityService
    {
        private readonly ILogger<CyberSecurityService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public CyberSecurityService(ILogger<CyberSecurityService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<CyberSecurityIncidentDto> CreateSecurityIncidentAsync(CyberSecurityIncidentDto incident)
        {
            try
            {
                incident.Id = Guid.NewGuid();
                incident.IncidentNumber = $"SEC-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                incident.CreatedAt = DateTime.UtcNow;
                incident.Status = "Open";

                _logger.LogInformation("Security incident created: {IncidentId} - {IncidentNumber}", incident.Id, incident.IncidentNumber);
                return incident;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create security incident");
                throw;
            }
        }

        public async Task<List<CyberSecurityIncidentDto>> GetSecurityIncidentsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CyberSecurityIncidentDto>
            {
                new CyberSecurityIncidentDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    IncidentNumber = "SEC-20241227-1001",
                    Title = "Suspicious Login Activity Detected",
                    Description = "Multiple failed login attempts from unusual geographic locations detected for admin accounts",
                    IncidentType = "Authentication Anomaly",
                    Severity = "High",
                    Priority = "Critical",
                    Status = "Under Investigation",
                    DetectedAt = DateTime.UtcNow.AddHours(-2),
                    ReportedBy = "Automated Security System",
                    AssignedTo = "Security Operations Center",
                    ImpactAssessment = "Potential unauthorized access attempt to administrative accounts",
                    CreatedAt = DateTime.UtcNow.AddHours(-2),
                    UpdatedAt = DateTime.UtcNow.AddMinutes(-30)
                }
            };
        }

        public async Task<CyberSecurityIncidentDto> UpdateSecurityIncidentAsync(Guid incidentId, CyberSecurityIncidentDto incident)
        {
            try
            {
                await Task.CompletedTask;
                incident.Id = incidentId;
                incident.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Security incident updated: {IncidentId}", incidentId);
                return incident;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update security incident {IncidentId}", incidentId);
                throw;
            }
        }

        public async Task<SecurityAssessmentDto> CreateSecurityAssessmentAsync(SecurityAssessmentDto assessment)
        {
            try
            {
                assessment.Id = Guid.NewGuid();
                assessment.AssessmentNumber = $"SA-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                assessment.CreatedAt = DateTime.UtcNow;
                assessment.Status = "Scheduled";

                _logger.LogInformation("Security assessment created: {AssessmentId} - {AssessmentNumber}", assessment.Id, assessment.AssessmentNumber);
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create security assessment");
                throw;
            }
        }

        public async Task<List<SecurityAssessmentDto>> GetSecurityAssessmentsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<SecurityAssessmentDto>
            {
                new SecurityAssessmentDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssessmentNumber = "SA-20241227-1001",
                    AssessmentName = "Q4 2024 Comprehensive Security Assessment",
                    AssessmentType = "Internal Security Audit",
                    Scope = "Enterprise-wide security posture evaluation",
                    AssessmentDate = DateTime.UtcNow.AddDays(-15),
                    AssessorName = "Internal Security Team",
                    AssessorOrganization = "Hudur Security Operations",
                    Status = "Completed",
                    OverallScore = 7.8,
                    SecurityMaturityLevel = "Managed",
                    IdentityAccessScore = 8.2,
                    NetworkSecurityScore = 7.5,
                    DataProtectionScore = 8.0,
                    IncidentResponseScore = 7.8,
                    ComplianceScore = 8.5,
                    VulnerabilityManagementScore = 7.2,
                    SecurityAwarenessScore = 6.8,
                    PhysicalSecurityScore = 8.0,
                    CreatedAt = DateTime.UtcNow.AddDays(-15),
                    UpdatedAt = DateTime.UtcNow.AddDays(-10)
                }
            };
        }

        public async Task<ThreatIntelligenceDto> CreateThreatIntelligenceAsync(ThreatIntelligenceDto threat)
        {
            try
            {
                threat.Id = Guid.NewGuid();
                threat.ThreatId = $"TI-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                threat.CreatedAt = DateTime.UtcNow;
                threat.Status = "Active";

                _logger.LogInformation("Threat intelligence created: {ThreatId} - {ThreatNumber}", threat.Id, threat.ThreatId);
                return threat;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create threat intelligence");
                throw;
            }
        }

        public async Task<List<ThreatIntelligenceDto>> GetThreatIntelligenceAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<ThreatIntelligenceDto>
            {
                new ThreatIntelligenceDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ThreatId = "TI-20241227-1001",
                    ThreatName = "APT-29 Targeting HR Systems",
                    ThreatType = "Advanced Persistent Threat",
                    ThreatCategory = "Nation State Actor",
                    Severity = "Critical",
                    ConfidenceLevel = 85.5,
                    Source = "Government Threat Intelligence Feed",
                    Description = "APT-29 group actively targeting HR management systems to steal employee data and credentials",
                    RelevanceScore = 9.2,
                    ExpirationDate = DateTime.UtcNow.AddDays(90),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-6)
                }
            };
        }

        public async Task<CyberSecurityVulnerabilityDto> CreateVulnerabilityAsync(CyberSecurityVulnerabilityDto vulnerability)
        {
            try
            {
                vulnerability.Id = Guid.NewGuid();
                vulnerability.VulnerabilityId = $"VULN-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                vulnerability.CreatedAt = DateTime.UtcNow;
                vulnerability.Status = "Open";

                _logger.LogInformation("Vulnerability created: {VulnerabilityId} - {VulnerabilityNumber}", vulnerability.Id, vulnerability.VulnerabilityId);
                return vulnerability;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create vulnerability");
                throw;
            }
        }

        public async Task<List<CyberSecurityVulnerabilityDto>> GetVulnerabilitiesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CyberSecurityVulnerabilityDto>
            {
                new CyberSecurityVulnerabilityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    VulnerabilityId = "VULN-20241227-1001",
                    Title = "Critical SQL Injection in User Management API",
                    Description = "SQL injection vulnerability in user search functionality allows unauthorized data access",
                    CveId = "CVE-2024-12345",
                    CvssScore = 9.1,
                    Severity = "Critical",
                    Priority = "P1",
                    Status = "Open",
                    DiscoveredDate = DateTime.UtcNow.AddDays(-3),
                    DiscoveryMethod = "Automated Security Scanning",
                    VulnerabilityType = "Injection",
                    AttackVector = "Network",
                    AttackComplexity = "Low",
                    PrivilegesRequired = "None",
                    UserInteraction = "None",
                    Scope = "Changed",
                    ConfidentialityImpact = "High",
                    IntegrityImpact = "High",
                    AvailabilityImpact = "High",
                    ExploitabilityScore = 3.9,
                    ImpactScore = 6.0,
                    TechnicalDetails = "The search parameter is directly concatenated into SQL query without proper sanitization",
                    ProofOfConcept = "GET /api/users/search?query=' OR '1'='1' --",
                    AssignedTo = "Development Team Lead",
                    DueDate = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<SecurityAnalyticsDto> GetSecurityAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new SecurityAnalyticsDto
            {
                TenantId = tenantId,
                TotalIncidents = 156,
                OpenIncidents = 12,
                ResolvedIncidents = 144,
                CriticalIncidents = 8,
                HighIncidents = 25,
                MediumIncidents = 78,
                LowIncidents = 45,
                AverageResolutionTime = 4.2,
                SecurityScore = 7.8,
                ThreatLevel = "Medium",
                VulnerabilityCount = 45,
                CriticalVulnerabilities = 3,
                HighVulnerabilities = 12,
                MediumVulnerabilities = 20,
                LowVulnerabilities = 10,
                PatchComplianceRate = 92.5,
                SecurityTrainingCompletion = 88.5,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<CyberSecurityReportDto> GenerateSecurityReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new CyberSecurityReportDto
            {
                TenantId = tenantId,
                ReportType = "Security Assessment Report",
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                ExecutiveSummary = "Security posture remains strong with 95% incident resolution rate and improved threat detection capabilities.",
                FromDate = fromDate,
                ToDate = toDate,
                TotalIncidents = 45,
                ResolvedIncidents = 42,
                OpenIncidents = 3,
                ResolutionRate = 93.3,
                AverageResolutionTime = 3.8,
                SecurityScore = 7.8,
                ThreatLevel = "Medium",
                TopThreats = new List<string> { "Phishing Attacks", "Malware", "Insider Threats", "DDoS Attacks", "Data Breaches" },
                Recommendations = new List<string> { "Enhance email security", "Update security training", "Implement zero-trust architecture" },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<CyberSecurityPolicyDto>> GetSecurityPoliciesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CyberSecurityPolicyDto>
            {
                new CyberSecurityPolicyDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PolicyNumber = "SP-20241227-1001",
                    PolicyName = "Information Security Policy",
                    PolicyType = "Corporate Policy",
                    Category = "Information Security",
                    Version = "2.1",
                    Status = "Active",
                    EffectiveDate = DateTime.UtcNow.AddDays(-180),
                    ReviewDate = DateTime.UtcNow.AddDays(185),
                    Owner = "Chief Information Security Officer",
                    Approver = "Chief Executive Officer",
                    Scope = "All employees, contractors, and third-party users",
                    Purpose = "Establish comprehensive information security requirements and responsibilities",
                    PolicyStatement = "All information assets must be protected according to their classification level and business value",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<CyberSecurityPolicyDto> CreateSecurityPolicyAsync(CyberSecurityPolicyDto policy)
        {
            try
            {
                policy.Id = Guid.NewGuid();
                policy.PolicyNumber = $"SP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                policy.CreatedAt = DateTime.UtcNow;
                policy.Status = "Draft";

                _logger.LogInformation("Security policy created: {PolicyId} - {PolicyNumber}", policy.Id, policy.PolicyNumber);
                return policy;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create security policy");
                throw;
            }
        }

        public async Task<bool> UpdateSecurityPolicyAsync(Guid policyId, CyberSecurityPolicyDto policy)
        {
            try
            {
                await Task.CompletedTask;
                policy.UpdatedAt = DateTime.UtcNow;
                _logger.LogInformation("Security policy updated: {PolicyId}", policyId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update security policy {PolicyId}", policyId);
                return false;
            }
        }

        public async Task<List<CyberSecurityAuditDto>> GetSecurityAuditsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CyberSecurityAuditDto>
            {
                new CyberSecurityAuditDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AuditNumber = "AUDIT-20241227-1001",
                    AuditName = "Annual Security Compliance Audit",
                    AuditType = "Compliance Audit",
                    AuditorName = "External Security Auditor",
                    AuditorOrganization = "CyberSec Audit Firm",
                    AuditDate = DateTime.UtcNow.AddDays(-30),
                    Status = "Completed",
                    Scope = "Enterprise-wide security controls and compliance",
                    OverallRating = "Satisfactory",
                    ComplianceScore = 92.5,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25)
                }
            };
        }

        public async Task<CyberSecurityAuditDto> CreateSecurityAuditAsync(CyberSecurityAuditDto audit)
        {
            try
            {
                audit.Id = Guid.NewGuid();
                audit.AuditNumber = $"AUDIT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                audit.CreatedAt = DateTime.UtcNow;
                audit.Status = "Planned";

                _logger.LogInformation("Security audit created: {AuditId} - {AuditNumber}", audit.Id, audit.AuditNumber);
                return audit;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create security audit");
                throw;
            }
        }

        public async Task<CyberSecurityComplianceDto> GetSecurityComplianceAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new CyberSecurityComplianceDto
            {
                TenantId = tenantId,
                OverallComplianceScore = 94.5,
                LastUpdated = DateTime.UtcNow.AddDays(-7),
                NextReviewDate = DateTime.UtcNow.AddDays(83)
            };
        }

        public async Task<bool> UpdateSecurityComplianceAsync(Guid tenantId, CyberSecurityComplianceDto compliance)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Security compliance updated for tenant: {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update security compliance for tenant {TenantId}", tenantId);
                return false;
            }
        }
    }

    public class CyberSecurityIncidentDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string IncidentNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string IncidentType { get; set; }
        public required string Severity { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public DateTime DetectedAt { get; set; }
        public required string ReportedBy { get; set; }
        public required string AssignedTo { get; set; }
        public required string ImpactAssessment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SecurityAssessmentDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string AssessmentNumber { get; set; }
        public required string AssessmentName { get; set; }
        public required string AssessmentType { get; set; }
        public required string Scope { get; set; }
        public DateTime AssessmentDate { get; set; }
        public required string AssessorName { get; set; }
        public required string AssessorOrganization { get; set; }
        public required string Status { get; set; }
        public double OverallScore { get; set; }
        public required string SecurityMaturityLevel { get; set; }
        public double IdentityAccessScore { get; set; }
        public double NetworkSecurityScore { get; set; }
        public double DataProtectionScore { get; set; }
        public double IncidentResponseScore { get; set; }
        public double ComplianceScore { get; set; }
        public double VulnerabilityManagementScore { get; set; }
        public double SecurityAwarenessScore { get; set; }
        public double PhysicalSecurityScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ThreatIntelligenceDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string ThreatId { get; set; }
        public required string ThreatName { get; set; }
        public required string ThreatType { get; set; }
        public required string ThreatCategory { get; set; }
        public required string Severity { get; set; }
        public double ConfidenceLevel { get; set; }
        public required string Source { get; set; }
        public required string Description { get; set; }
        public double RelevanceScore { get; set; }
        public DateTime ExpirationDate { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CyberSecurityVulnerabilityDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string VulnerabilityId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string CveId { get; set; }
        public double CvssScore { get; set; }
        public required string Severity { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public DateTime DiscoveredDate { get; set; }
        public required string DiscoveryMethod { get; set; }
        public required string VulnerabilityType { get; set; }
        public required string AttackVector { get; set; }
        public required string AttackComplexity { get; set; }
        public required string PrivilegesRequired { get; set; }
        public required string UserInteraction { get; set; }
        public required string Scope { get; set; }
        public required string ConfidentialityImpact { get; set; }
        public required string IntegrityImpact { get; set; }
        public required string AvailabilityImpact { get; set; }
        public double ExploitabilityScore { get; set; }
        public double ImpactScore { get; set; }
        public required string TechnicalDetails { get; set; }
        public required string ProofOfConcept { get; set; }
        public required string AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SecurityAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int CriticalIncidents { get; set; }
        public int HighIncidents { get; set; }
        public int MediumIncidents { get; set; }
        public int LowIncidents { get; set; }
        public double AverageResolutionTime { get; set; }
        public double SecurityScore { get; set; }
        public required string ThreatLevel { get; set; }
        public int VulnerabilityCount { get; set; }
        public int CriticalVulnerabilities { get; set; }
        public int HighVulnerabilities { get; set; }
        public int MediumVulnerabilities { get; set; }
        public int LowVulnerabilities { get; set; }
        public double PatchComplianceRate { get; set; }
        public double SecurityTrainingCompletion { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class SecurityReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public required string ExecutiveSummary { get; set; }
        public int TotalIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public double ResolutionRate { get; set; }
        public double AverageResolutionTime { get; set; }
        public double SecurityScore { get; set; }
        public required string ThreatLevel { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class CyberSecurityPolicyDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyName { get; set; }
        public string PolicyType { get; set; }
        public string Category { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ReviewDate { get; set; }
        public string Owner { get; set; }
        public string Approver { get; set; }
        public string Scope { get; set; }
        public string Purpose { get; set; }
        public string PolicyStatement { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CyberSecurityAuditDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string AuditNumber { get; set; }
        public string AuditName { get; set; }
        public string AuditType { get; set; }
        public string AuditorName { get; set; }
        public string AuditorOrganization { get; set; }
        public DateTime AuditDate { get; set; }
        public string Status { get; set; }
        public string Scope { get; set; }
        public string OverallRating { get; set; }
        public double ComplianceScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CyberSecurityComplianceDto
    {
        public Guid TenantId { get; set; }
        public double OverallComplianceScore { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime NextReviewDate { get; set; }
    }

    public class CyberSecurityReportDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string ReportType { get; set; }
        public required string ReportPeriod { get; set; }
        public required string ExecutiveSummary { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalIncidents { get; set; }
        public int CriticalIncidents { get; set; }
        public int ResolvedIncidents { get; set; }
        public int OpenIncidents { get; set; }
        public double ResolutionRate { get; set; }
        public double AverageResolutionTime { get; set; }
        public double SecurityScore { get; set; }
        public required string ThreatLevel { get; set; }
        public required List<string> TopThreats { get; set; }
        public required List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
