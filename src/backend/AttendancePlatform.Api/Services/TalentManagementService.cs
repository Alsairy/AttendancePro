using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface ITalentManagementService
    {
        Task<TalentProfileDto> CreateTalentProfileAsync(TalentProfileDto profile);
        Task<List<TalentProfileDto>> GetTalentProfilesAsync(Guid tenantId);
        Task<TalentProfileDto> UpdateTalentProfileAsync(Guid profileId, TalentProfileDto profile);
        Task<TalentManagementSkillAssessmentDto> CreateSkillAssessmentAsync(TalentManagementSkillAssessmentDto assessment);
        Task<List<TalentManagementSkillAssessmentDto>> GetSkillAssessmentsAsync(Guid employeeId);
        Task<CareerPathDto> CreateCareerPathAsync(CareerPathDto careerPath);
        Task<List<CareerPathDto>> GetCareerPathsAsync(Guid tenantId);
        Task<SuccessionPlanDto> CreateSuccessionPlanAsync(SuccessionPlanDto plan);
        Task<List<SuccessionPlanDto>> GetSuccessionPlansAsync(Guid tenantId);
        Task<TalentPoolDto> CreateTalentPoolAsync(TalentPoolDto pool);
        Task<List<TalentPoolDto>> GetTalentPoolsAsync(Guid tenantId);
        Task<MentorshipProgramDto> CreateMentorshipProgramAsync(MentorshipProgramDto program);
        Task<List<MentorshipProgramDto>> GetMentorshipProgramsAsync(Guid tenantId);
        Task<TalentAnalyticsDto> GetTalentAnalyticsAsync(Guid tenantId);
        Task<TalentReportDto> GenerateTalentReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<List<TalentProfileDto>> SearchTalentBySkillsAsync(Guid tenantId, List<string> skills);
        Task<TalentRetentionDto> GetTalentRetentionAnalysisAsync(Guid tenantId);
        Task<bool> AssignMentorAsync(Guid menteeId, Guid mentorId);
        Task<List<TalentGapDto>> IdentifyTalentGapsAsync(Guid tenantId);
    }

    public class TalentManagementService : ITalentManagementService
    {
        private readonly ILogger<TalentManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public TalentManagementService(ILogger<TalentManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<TalentProfileDto> CreateTalentProfileAsync(TalentProfileDto profile)
        {
            try
            {
                profile.Id = Guid.NewGuid();
                profile.ProfileNumber = $"TP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                profile.CreatedAt = DateTime.UtcNow;
                profile.Status = "Active";
                profile.TalentScore = CalculateTalentScore(profile);

                _logger.LogInformation("Talent profile created: {ProfileId} - {ProfileNumber}", profile.Id, profile.ProfileNumber);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create talent profile");
                throw;
            }
        }

        public async Task<List<TalentProfileDto>> GetTalentProfilesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<TalentProfileDto>
            {
                new TalentProfileDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ProfileNumber = "TP-20241227-1001",
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "Sarah Johnson",
                    Position = "Senior Software Engineer",
                    Department = "Engineering",
                    HireDate = DateTime.UtcNow.AddDays(-1095),
                    TalentScore = 8.5,
                    PotentialRating = "High",
                    PerformanceRating = "Exceeds Expectations",
                    Skills = new List<string> { "C#", ".NET", "React", "Azure", "Leadership" },
                    Certifications = new List<string> { "Microsoft Azure Solutions Architect", "Scrum Master" },
                    CareerAspiration = "Engineering Manager",
                    StrengthAreas = new List<string> { "Technical Leadership", "Problem Solving", "Team Collaboration" },
                    DevelopmentAreas = new List<string> { "Public Speaking", "Strategic Planning" },
                    ReadinessForPromotion = "Ready",
                    LastAssessmentDate = DateTime.UtcNow.AddDays(-90),
                    NextAssessmentDate = DateTime.UtcNow.AddDays(90),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-365),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<TalentProfileDto> UpdateTalentProfileAsync(Guid profileId, TalentProfileDto profile)
        {
            try
            {
                await Task.CompletedTask;
                profile.Id = profileId;
                profile.UpdatedAt = DateTime.UtcNow;
                profile.TalentScore = CalculateTalentScore(profile);

                _logger.LogInformation("Talent profile updated: {ProfileId}", profileId);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update talent profile {ProfileId}", profileId);
                throw;
            }
        }

        public async Task<TalentManagementSkillAssessmentDto> CreateSkillAssessmentAsync(TalentManagementSkillAssessmentDto assessment)
        {
            try
            {
                assessment.Id = Guid.NewGuid();
                assessment.AssessmentNumber = $"SA-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                assessment.CreatedAt = DateTime.UtcNow;
                assessment.Status = "Completed";
                assessment.OverallScore = CalculateOverallSkillScore(assessment.SkillScores);

                _logger.LogInformation("Skill assessment created: {AssessmentId} - {AssessmentNumber}", assessment.Id, assessment.AssessmentNumber);
                return assessment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create skill assessment");
                throw;
            }
        }

        public async Task<List<TalentManagementSkillAssessmentDto>> GetSkillAssessmentsAsync(Guid employeeId)
        {
            await Task.CompletedTask;
            return new List<TalentManagementSkillAssessmentDto>
            {
                new TalentManagementSkillAssessmentDto
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    AssessmentNumber = "SA-20241227-1001",
                    AssessmentType = "Annual Review",
                    AssessmentDate = DateTime.UtcNow.AddDays(-30),
                    AssessorId = Guid.NewGuid(),
                    AssessorName = "Manager Smith",
                    SkillScores = new Dictionary<string, double>
                    {
                        { "Technical Skills", 8.5 },
                        { "Communication", 7.8 },
                        { "Leadership", 8.0 },
                        { "Problem Solving", 9.0 },
                        { "Teamwork", 8.2 }
                    },
                    OverallScore = 8.3,
                    Strengths = new List<string> { "Excellent problem-solving abilities", "Strong technical expertise" },
                    AreasForImprovement = new List<string> { "Public speaking", "Delegation skills" },
                    DevelopmentRecommendations = new List<string> { "Leadership training", "Presentation skills workshop" },
                    Status = "Completed",
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<CareerPathDto> CreateCareerPathAsync(CareerPathDto careerPath)
        {
            try
            {
                careerPath.Id = Guid.NewGuid();
                careerPath.PathNumber = $"CP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                careerPath.CreatedAt = DateTime.UtcNow;
                careerPath.Status = "Active";

                _logger.LogInformation("Career path created: {PathId} - {PathNumber}", careerPath.Id, careerPath.PathNumber);
                return careerPath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create career path");
                throw;
            }
        }

        public async Task<List<CareerPathDto>> GetCareerPathsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CareerPathDto>
            {
                new CareerPathDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PathNumber = "CP-20241227-1001",
                    PathName = "Software Engineering Career Track",
                    Department = "Engineering",
                    StartingPosition = "Junior Software Engineer",
                    TargetPosition = "Engineering Manager",
                    EstimatedDuration = 60,
                    RequiredSkills = new List<string> { "Programming", "System Design", "Leadership", "Project Management" },
                    Milestones = new List<CareerMilestoneDto>
                    {
                        new CareerMilestoneDto { Position = "Software Engineer", Duration = 18, RequiredSkills = new List<string> { "Programming", "Testing" } },
                        new CareerMilestoneDto { Position = "Senior Software Engineer", Duration = 24, RequiredSkills = new List<string> { "Architecture", "Mentoring" } },
                        new CareerMilestoneDto { Position = "Lead Software Engineer", Duration = 18, RequiredSkills = new List<string> { "Leadership", "System Design" } }
                    },
                    Prerequisites = new List<string> { "Bachelor's degree in Computer Science or related field", "2+ years programming experience" },
                    DevelopmentPrograms = new List<string> { "Technical Leadership Program", "Management Fundamentals" },
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<SuccessionPlanDto> CreateSuccessionPlanAsync(SuccessionPlanDto plan)
        {
            try
            {
                plan.Id = Guid.NewGuid();
                plan.PlanNumber = $"SP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                plan.CreatedAt = DateTime.UtcNow;
                plan.Status = "Active";

                _logger.LogInformation("Succession plan created: {PlanId} - {PlanNumber}", plan.Id, plan.PlanNumber);
                return plan;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create succession plan");
                throw;
            }
        }

        public async Task<List<SuccessionPlanDto>> GetSuccessionPlansAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<SuccessionPlanDto>
            {
                new SuccessionPlanDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PlanNumber = "SP-20241227-1001",
                    KeyPosition = "Engineering Manager",
                    Department = "Engineering",
                    CurrentIncumbent = "John Manager",
                    CriticalityLevel = "High",
                    RiskLevel = "Medium",
                    Successors = new List<SuccessorCandidateDto>
                    {
                        new SuccessorCandidateDto { Name = "Sarah Johnson", Readiness = "Ready Now", ReadinessScore = 9.0 },
                        new SuccessorCandidateDto { Name = "Mike Developer", Readiness = "Ready in 6 months", ReadinessScore = 7.5 },
                        new SuccessorCandidateDto { Name = "Lisa Tech Lead", Readiness = "Ready in 12 months", ReadinessScore = 6.8 }
                    },
                    DevelopmentActions = new List<string>
                    {
                        "Leadership training for top candidates",
                        "Cross-functional project assignments",
                        "Mentoring program participation"
                    },
                    TimelineMonths = 12,
                    NextReviewDate = DateTime.UtcNow.AddDays(90),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };
        }

        public async Task<TalentPoolDto> CreateTalentPoolAsync(TalentPoolDto pool)
        {
            try
            {
                pool.Id = Guid.NewGuid();
                pool.PoolNumber = $"TL-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                pool.CreatedAt = DateTime.UtcNow;
                pool.Status = "Active";

                _logger.LogInformation("Talent pool created: {PoolId} - {PoolNumber}", pool.Id, pool.PoolNumber);
                return pool;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create talent pool");
                throw;
            }
        }

        public async Task<List<TalentPoolDto>> GetTalentPoolsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<TalentPoolDto>
            {
                new TalentPoolDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PoolNumber = "TL-20241227-1001",
                    PoolName = "High Potential Leaders",
                    Description = "Employees identified as high potential for leadership roles",
                    Category = "Leadership",
                    Criteria = new List<string> { "High performance rating", "Leadership potential", "Cultural fit" },
                    MemberCount = 15,
                    Members = new List<TalentPoolMemberDto>
                    {
                        new TalentPoolMemberDto { EmployeeName = "Sarah Johnson", Position = "Senior Software Engineer", TalentScore = 8.5 },
                        new TalentPoolMemberDto { EmployeeName = "Michael Chen", Position = "Marketing Specialist", TalentScore = 7.8 },
                        new TalentPoolMemberDto { EmployeeName = "Lisa Rodriguez", Position = "Product Manager", TalentScore = 8.2 }
                    },
                    DevelopmentPrograms = new List<string> { "Leadership Development Program", "Executive Coaching" },
                    LastReviewDate = DateTime.UtcNow.AddDays(-30),
                    NextReviewDate = DateTime.UtcNow.AddDays(90),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<MentorshipProgramDto> CreateMentorshipProgramAsync(MentorshipProgramDto program)
        {
            try
            {
                program.Id = Guid.NewGuid();
                program.ProgramNumber = $"MP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                program.CreatedAt = DateTime.UtcNow;
                program.Status = "Active";

                _logger.LogInformation("Mentorship program created: {ProgramId} - {ProgramNumber}", program.Id, program.ProgramNumber);
                return program;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create mentorship program");
                throw;
            }
        }

        public async Task<List<MentorshipProgramDto>> GetMentorshipProgramsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MentorshipProgramDto>
            {
                new MentorshipProgramDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ProgramNumber = "MP-20241227-1001",
                    ProgramName = "Leadership Mentorship Program",
                    Description = "Pairing high-potential employees with senior leaders",
                    ProgramType = "Leadership Development",
                    Duration = 12,
                    MaxParticipants = 20,
                    CurrentParticipants = 18,
                    Objectives = new List<string>
                    {
                        "Develop leadership skills",
                        "Build professional networks",
                        "Accelerate career growth"
                    },
                    MentorCriteria = new List<string> { "Senior leadership role", "5+ years experience", "Strong communication skills" },
                    MenteeCriteria = new List<string> { "High potential rating", "Career growth aspirations", "Commitment to program" },
                    MatchingCriteria = new List<string> { "Career goals alignment", "Personality compatibility", "Skill development needs" },
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    EndDate = DateTime.UtcNow.AddDays(270),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-120),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<TalentAnalyticsDto> GetTalentAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new TalentAnalyticsDto
            {
                TenantId = tenantId,
                TotalEmployees = 250,
                HighPotentialEmployees = 45,
                ReadyForPromotionCount = 28,
                AtRiskTalentCount = 12,
                AverageTalentScore = 7.2,
                TalentRetentionRate = 92.5,
                InternalPromotionRate = 68.3,
                SuccessionCoverage = 75.0,
                TalentByDepartment = new Dictionary<string, TalentDepartmentStatsDto>
                {
                    { "Engineering", new TalentDepartmentStatsDto { TotalCount = 85, HighPotential = 18, AvgScore = 7.8 } },
                    { "Sales", new TalentDepartmentStatsDto { TotalCount = 45, HighPotential = 12, AvgScore = 7.1 } },
                    { "Marketing", new TalentDepartmentStatsDto { TotalCount = 35, HighPotential = 8, AvgScore = 6.9 } },
                    { "Operations", new TalentDepartmentStatsDto { TotalCount = 55, HighPotential = 5, AvgScore = 6.8 } },
                    { "HR", new TalentDepartmentStatsDto { TotalCount = 15, HighPotential = 2, AvgScore = 7.3 } },
                    { "Finance", new TalentDepartmentStatsDto { TotalCount = 15, HighPotential = 0, AvgScore = 6.5 } }
                },
                SkillGaps = new Dictionary<string, int>
                {
                    { "Leadership", 15 },
                    { "Data Analysis", 12 },
                    { "Project Management", 10 },
                    { "Digital Marketing", 8 },
                    { "Cloud Technologies", 18 }
                },
                TalentMobility = new Dictionary<string, double>
                {
                    { "Internal Transfers", 15.2 },
                    { "Promotions", 8.5 },
                    { "Cross-Department Moves", 5.8 },
                    { "Role Changes", 12.3 }
                },
                DevelopmentPrograms = new Dictionary<string, int>
                {
                    { "Leadership Development", 35 },
                    { "Technical Training", 65 },
                    { "Mentorship Programs", 43 },
                    { "Certification Programs", 28 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<TalentReportDto> GenerateTalentReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new TalentReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalAssessments = 125,
                NewTalentIdentified = 18,
                PromotionsExecuted = 12,
                TalentLost = 5,
                AverageTalentScore = 7.4,
                TalentRetentionRate = 94.2,
                TopPerformers = new List<string>
                {
                    "Sarah Johnson - Engineering (Score: 9.2)",
                    "Michael Chen - Marketing (Score: 8.8)",
                    "Lisa Rodriguez - Product (Score: 8.6)",
                    "David Kim - Sales (Score: 8.4)",
                    "Anna Smith - Operations (Score: 8.2)"
                },
                SkillDevelopmentProgress = new Dictionary<string, TalentSkillProgressDto>
                {
                    { "Leadership", new TalentSkillProgressDto { ParticipantsCount = 35, AverageImprovement = 1.2, CompletionRate = 85.7 } },
                    { "Technical Skills", new TalentSkillProgressDto { ParticipantsCount = 65, AverageImprovement = 0.8, CompletionRate = 92.3 } },
                    { "Communication", new TalentSkillProgressDto { ParticipantsCount = 28, AverageImprovement = 1.0, CompletionRate = 89.3 } }
                },
                SuccessionPlanProgress = new Dictionary<string, string>
                {
                    { "Engineering Manager", "2 candidates ready, 1 developing" },
                    { "Sales Director", "1 candidate ready, 2 developing" },
                    { "Marketing Manager", "3 candidates ready" },
                    { "Operations Lead", "1 candidate developing" }
                },
                TalentRisks = new List<string>
                {
                    "High performer in Sales considering external opportunity",
                    "Key technical lead approaching retirement",
                    "Limited succession depth in Operations"
                },
                Recommendations = new List<string>
                {
                    "Accelerate development programs for high-potential candidates",
                    "Implement retention strategies for at-risk talent",
                    "Expand cross-functional development opportunities",
                    "Increase focus on succession planning for critical roles"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<TalentProfileDto>> SearchTalentBySkillsAsync(Guid tenantId, List<string> skills)
        {
            await Task.CompletedTask;
            return new List<TalentProfileDto>
            {
                new TalentProfileDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ProfileNumber = "TP-20241227-1003",
                    EmployeeName = "Alex Developer",
                    Position = "Full Stack Developer",
                    Department = "Engineering",
                    TalentScore = 8.0,
                    Skills = skills.Concat(new List<string> { "JavaScript", "Python", "AWS" }).ToList(),
                    MatchingSkills = skills,
                    SkillMatchPercentage = 85.0,
                    Status = "Active"
                }
            };
        }

        public async Task<TalentRetentionDto> GetTalentRetentionAnalysisAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new TalentRetentionDto
            {
                TenantId = tenantId,
                OverallRetentionRate = 92.5,
                HighPerformerRetentionRate = 95.8,
                NewHireRetentionRate = 88.2,
                RetentionByDepartment = new Dictionary<string, double>
                {
                    { "Engineering", 94.1 },
                    { "Sales", 89.3 },
                    { "Marketing", 91.7 },
                    { "Operations", 93.2 },
                    { "HR", 96.0 },
                    { "Finance", 90.5 }
                },
                RetentionByTenure = new Dictionary<string, double>
                {
                    { "0-1 years", 88.2 },
                    { "1-3 years", 91.5 },
                    { "3-5 years", 94.8 },
                    { "5+ years", 96.2 }
                },
                AtRiskEmployees = new List<AtRiskEmployeeDto>
                {
                    new AtRiskEmployeeDto { Name = "John Developer", Department = "Engineering", RiskScore = 7.5, RiskFactors = new List<string> { "Low engagement", "Limited growth opportunities" } },
                    new AtRiskEmployeeDto { Name = "Mary Sales", Department = "Sales", RiskScore = 6.8, RiskFactors = new List<string> { "Compensation concerns", "Work-life balance" } }
                },
                RetentionStrategies = new List<string>
                {
                    "Career development programs",
                    "Competitive compensation reviews",
                    "Flexible work arrangements",
                    "Recognition and rewards programs"
                },
                TurnoverCost = 125000.00m,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> AssignMentorAsync(Guid menteeId, Guid mentorId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Mentor assigned: Mentor {MentorId} to Mentee {MenteeId}", mentorId, menteeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign mentor {MentorId} to mentee {MenteeId}", mentorId, menteeId);
                return false;
            }
        }

        public async Task<List<TalentGapDto>> IdentifyTalentGapsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<TalentGapDto>
            {
                new TalentGapDto
                {
                    Department = "Engineering",
                    Position = "Senior DevOps Engineer",
                    CurrentCount = 2,
                    RequiredCount = 4,
                    GapCount = 2,
                    CriticalityLevel = "High",
                    RequiredSkills = new List<string> { "Kubernetes", "AWS", "CI/CD", "Infrastructure as Code" },
                    TimeToFill = 90,
                    RecommendedActions = new List<string> { "External recruitment", "Internal training program" }
                }
            };
        }

        private double CalculateTalentScore(TalentProfileDto profile)
        {
            double baseScore = 5.0;
            
            if (profile.PerformanceRating == "Exceeds Expectations") baseScore += 2.0;
            else if (profile.PerformanceRating == "Meets Expectations") baseScore += 1.0;
            
            if (profile.PotentialRating == "High") baseScore += 1.5;
            else if (profile.PotentialRating == "Medium") baseScore += 0.5;
            
            if (profile.Skills?.Count > 5) baseScore += 0.5;
            if (profile.Certifications?.Count > 0) baseScore += 0.3;
            
            return Math.Min(baseScore, 10.0);
        }

        private double CalculateOverallSkillScore(Dictionary<string, double> skillScores)
        {
            if (skillScores == null || !skillScores.Any()) return 0.0;
            return skillScores.Values.Average();
        }
    }

    public class TalentProfileDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ProfileNumber { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public double TalentScore { get; set; }
        public string PotentialRating { get; set; }
        public string PerformanceRating { get; set; }
        public List<string> Skills { get; set; }
        public List<string> Certifications { get; set; }
        public string CareerAspiration { get; set; }
        public List<string> StrengthAreas { get; set; }
        public List<string> DevelopmentAreas { get; set; }
        public string ReadinessForPromotion { get; set; }
        public DateTime? LastAssessmentDate { get; set; }
        public DateTime? NextAssessmentDate { get; set; }
        public List<string> MatchingSkills { get; set; }
        public double SkillMatchPercentage { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TalentManagementSkillAssessmentDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string AssessmentNumber { get; set; }
        public string AssessmentType { get; set; }
        public DateTime AssessmentDate { get; set; }
        public Guid AssessorId { get; set; }
        public string AssessorName { get; set; }
        public Dictionary<string, double> SkillScores { get; set; }
        public double OverallScore { get; set; }
        public List<string> Strengths { get; set; }
        public List<string> AreasForImprovement { get; set; }
        public List<string> DevelopmentRecommendations { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CareerPathDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string PathNumber { get; set; }
        public string PathName { get; set; }
        public string Department { get; set; }
        public string StartingPosition { get; set; }
        public string TargetPosition { get; set; }
        public int EstimatedDuration { get; set; }
        public List<string> RequiredSkills { get; set; }
        public List<CareerMilestoneDto> Milestones { get; set; }
        public List<string> Prerequisites { get; set; }
        public List<string> DevelopmentPrograms { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CareerMilestoneDto
    {
        public string Position { get; set; }
        public int Duration { get; set; }
        public List<string> RequiredSkills { get; set; }
    }

    public class SuccessionPlanDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string PlanNumber { get; set; }
        public string KeyPosition { get; set; }
        public string Department { get; set; }
        public string CurrentIncumbent { get; set; }
        public string CriticalityLevel { get; set; }
        public string RiskLevel { get; set; }
        public List<SuccessorCandidateDto> Successors { get; set; }
        public List<string> DevelopmentActions { get; set; }
        public int TimelineMonths { get; set; }
        public DateTime NextReviewDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SuccessorCandidateDto
    {
        public string Name { get; set; }
        public string Readiness { get; set; }
        public double ReadinessScore { get; set; }
    }

    public class TalentPoolDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string PoolNumber { get; set; }
        public string PoolName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<string> Criteria { get; set; }
        public int MemberCount { get; set; }
        public List<TalentPoolMemberDto> Members { get; set; }
        public List<string> DevelopmentPrograms { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TalentPoolMemberDto
    {
        public string EmployeeName { get; set; }
        public string Position { get; set; }
        public double TalentScore { get; set; }
    }

    public class MentorshipProgramDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ProgramNumber { get; set; }
        public string ProgramName { get; set; }
        public string Description { get; set; }
        public string ProgramType { get; set; }
        public int Duration { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants { get; set; }
        public List<string> Objectives { get; set; }
        public List<string> MentorCriteria { get; set; }
        public List<string> MenteeCriteria { get; set; }
        public List<string> MatchingCriteria { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TalentAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalEmployees { get; set; }
        public int HighPotentialEmployees { get; set; }
        public int ReadyForPromotionCount { get; set; }
        public int AtRiskTalentCount { get; set; }
        public double AverageTalentScore { get; set; }
        public double TalentRetentionRate { get; set; }
        public double InternalPromotionRate { get; set; }
        public double SuccessionCoverage { get; set; }
        public Dictionary<string, TalentDepartmentStatsDto> TalentByDepartment { get; set; }
        public Dictionary<string, int> SkillGaps { get; set; }
        public Dictionary<string, double> TalentMobility { get; set; }
        public Dictionary<string, int> DevelopmentPrograms { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class TalentDepartmentStatsDto
    {
        public int TotalCount { get; set; }
        public int HighPotential { get; set; }
        public double AvgScore { get; set; }
    }

    public class TalentReportDto
    {
        public Guid TenantId { get; set; }
        public string ReportPeriod { get; set; }
        public int TotalAssessments { get; set; }
        public int NewTalentIdentified { get; set; }
        public int PromotionsExecuted { get; set; }
        public int TalentLost { get; set; }
        public double AverageTalentScore { get; set; }
        public double TalentRetentionRate { get; set; }
        public List<string> TopPerformers { get; set; }
        public Dictionary<string, TalentSkillProgressDto> SkillDevelopmentProgress { get; set; }
        public Dictionary<string, string> SuccessionPlanProgress { get; set; }
        public List<string> TalentRisks { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class TalentSkillProgressDto
    {
        public int ParticipantsCount { get; set; }
        public double AverageImprovement { get; set; }
        public double CompletionRate { get; set; }
    }

    public class TalentRetentionDto
    {
        public Guid TenantId { get; set; }
        public double OverallRetentionRate { get; set; }
        public double HighPerformerRetentionRate { get; set; }
        public double NewHireRetentionRate { get; set; }
        public Dictionary<string, double> RetentionByDepartment { get; set; }
        public Dictionary<string, double> RetentionByTenure { get; set; }
        public List<AtRiskEmployeeDto> AtRiskEmployees { get; set; }
        public List<string> RetentionStrategies { get; set; }
        public decimal TurnoverCost { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class AtRiskEmployeeDto
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public double RiskScore { get; set; }
        public List<string> RiskFactors { get; set; }
    }

    public class TalentGapDto
    {
        public string Department { get; set; }
        public string Position { get; set; }
        public int CurrentCount { get; set; }
        public int RequiredCount { get; set; }
        public int GapCount { get; set; }
        public string CriticalityLevel { get; set; }
        public List<string> RequiredSkills { get; set; }
        public int TimeToFill { get; set; }
        public List<string> RecommendedActions { get; set; }
    }
}
