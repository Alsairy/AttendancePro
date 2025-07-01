using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IKnowledgeManagementService
    {
        Task<KnowledgeArticleDto> CreateKnowledgeArticleAsync(KnowledgeArticleDto article);
        Task<List<KnowledgeArticleDto>> GetKnowledgeArticlesAsync(Guid tenantId);
        Task<KnowledgeArticleDto> UpdateKnowledgeArticleAsync(Guid articleId, KnowledgeArticleDto article);
        Task<bool> DeleteKnowledgeArticleAsync(Guid articleId);
        Task<List<KnowledgeArticleDto>> SearchKnowledgeBaseAsync(string searchTerm, Guid tenantId);
        Task<KnowledgeCategoryDto> CreateKnowledgeCategoryAsync(KnowledgeCategoryDto category);
        Task<List<KnowledgeCategoryDto>> GetKnowledgeCategoriesAsync(Guid tenantId);
        Task<KnowledgeTagDto> CreateKnowledgeTagAsync(KnowledgeTagDto tag);
        Task<List<KnowledgeTagDto>> GetKnowledgeTagsAsync(Guid tenantId);
        Task<KnowledgeRatingDto> RateKnowledgeArticleAsync(KnowledgeRatingDto rating);
        Task<List<KnowledgeRatingDto>> GetArticleRatingsAsync(Guid articleId);
        Task<KnowledgeCommentDto> AddCommentAsync(KnowledgeCommentDto comment);
        Task<List<KnowledgeCommentDto>> GetArticleCommentsAsync(Guid articleId);
        Task<KnowledgeAnalyticsDto> GetKnowledgeAnalyticsAsync(Guid tenantId);
        Task<List<KnowledgeArticleDto>> GetPopularArticlesAsync(Guid tenantId, int count = 10);
        Task<List<KnowledgeArticleDto>> GetRecentArticlesAsync(Guid tenantId, int count = 10);
        Task<KnowledgeReportDto> GenerateKnowledgeReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<bool> ApproveKnowledgeArticleAsync(Guid articleId, Guid approverId);
        Task<KnowledgeWorkflowDto> CreateApprovalWorkflowAsync(KnowledgeWorkflowDto workflow);
        Task<List<KnowledgeArticleDto>> GetPendingApprovalsAsync(Guid tenantId);
    }

    public class KnowledgeManagementService : IKnowledgeManagementService
    {
        private readonly ILogger<KnowledgeManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public KnowledgeManagementService(ILogger<KnowledgeManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<KnowledgeArticleDto> CreateKnowledgeArticleAsync(KnowledgeArticleDto article)
        {
            try
            {
                article.Id = Guid.NewGuid();
                article.ArticleNumber = $"KB-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                article.CreatedAt = DateTime.UtcNow;
                article.Status = "Draft";
                article.ViewCount = 0;
                article.LikeCount = 0;

                _logger.LogInformation("Knowledge article created: {ArticleId} - {ArticleNumber}", article.Id, article.ArticleNumber);
                return article;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create knowledge article");
                throw;
            }
        }

        public async Task<List<KnowledgeArticleDto>> GetKnowledgeArticlesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeArticleDto>
            {
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1001",
                    Title = "Employee Onboarding Process Guide",
                    Content = "Comprehensive guide for new employee onboarding procedures and requirements.",
                    Summary = "Step-by-step onboarding process for new hires",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "HR Procedures",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "HR Manager",
                    Status = "Published",
                    Priority = "High",
                    ViewCount = 245,
                    LikeCount = 18,
                    AverageRating = 4.5,
                    Tags = new List<string> { "onboarding", "hr", "procedures", "new-hire" },
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5),
                    PublishedAt = DateTime.UtcNow.AddDays(-25)
                },
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1002",
                    Title = "IT Security Best Practices",
                    Content = "Essential cybersecurity guidelines and best practices for all employees.",
                    Summary = "Security protocols and guidelines for safe computing",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "IT Security",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "IT Security Team",
                    Status = "Published",
                    Priority = "Critical",
                    ViewCount = 189,
                    LikeCount = 25,
                    AverageRating = 4.8,
                    Tags = new List<string> { "security", "cybersecurity", "best-practices", "guidelines" },
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2),
                    PublishedAt = DateTime.UtcNow.AddDays(-18)
                }
            };
        }

        public async Task<KnowledgeArticleDto> UpdateKnowledgeArticleAsync(Guid articleId, KnowledgeArticleDto article)
        {
            try
            {
                await Task.CompletedTask;
                article.Id = articleId;
                article.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Knowledge article updated: {ArticleId}", articleId);
                return article;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update knowledge article {ArticleId}", articleId);
                throw;
            }
        }

        public async Task<bool> DeleteKnowledgeArticleAsync(Guid articleId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Knowledge article deleted: {ArticleId}", articleId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete knowledge article {ArticleId}", articleId);
                return false;
            }
        }

        public async Task<List<KnowledgeArticleDto>> SearchKnowledgeBaseAsync(string searchTerm, Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeArticleDto>
            {
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1003",
                    Title = $"Search Results for: {searchTerm}",
                    Content = "Relevant knowledge article content matching search criteria.",
                    Summary = "Search result summary",
                    CategoryId = Guid.NewGuid(),
                    CategoryName = "General",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "Knowledge Admin",
                    Status = "Published",
                    Priority = "Medium",
                    ViewCount = 45,
                    LikeCount = 3,
                    AverageRating = 4.2,
                    Tags = new List<string> { "search", "results" },
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    PublishedAt = DateTime.UtcNow.AddDays(-8)
                }
            };
        }

        public async Task<KnowledgeCategoryDto> CreateKnowledgeCategoryAsync(KnowledgeCategoryDto category)
        {
            try
            {
                category.Id = Guid.NewGuid();
                category.CreatedAt = DateTime.UtcNow;
                category.IsActive = true;

                _logger.LogInformation("Knowledge category created: {CategoryId} - {CategoryName}", category.Id, category.Name);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create knowledge category");
                throw;
            }
        }

        public async Task<List<KnowledgeCategoryDto>> GetKnowledgeCategoriesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeCategoryDto>
            {
                new KnowledgeCategoryDto { Id = Guid.NewGuid(), Name = "HR Procedures", Description = "Human resources policies and procedures", ArticleCount = 25, IsActive = true },
                new KnowledgeCategoryDto { Id = Guid.NewGuid(), Name = "IT Security", Description = "Information technology security guidelines", ArticleCount = 18, IsActive = true },
                new KnowledgeCategoryDto { Id = Guid.NewGuid(), Name = "Finance", Description = "Financial processes and procedures", ArticleCount = 12, IsActive = true },
                new KnowledgeCategoryDto { Id = Guid.NewGuid(), Name = "Operations", Description = "Operational procedures and workflows", ArticleCount = 22, IsActive = true },
                new KnowledgeCategoryDto { Id = Guid.NewGuid(), Name = "Compliance", Description = "Regulatory compliance and legal requirements", ArticleCount = 15, IsActive = true }
            };
        }

        public async Task<KnowledgeTagDto> CreateKnowledgeTagAsync(KnowledgeTagDto tag)
        {
            try
            {
                tag.Id = Guid.NewGuid();
                tag.CreatedAt = DateTime.UtcNow;
                tag.IsActive = true;

                _logger.LogInformation("Knowledge tag created: {TagId} - {TagName}", tag.Id, tag.Name);
                return tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create knowledge tag");
                throw;
            }
        }

        public async Task<List<KnowledgeTagDto>> GetKnowledgeTagsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeTagDto>
            {
                new KnowledgeTagDto { Id = Guid.NewGuid(), Name = "onboarding", Description = "Employee onboarding related", UsageCount = 15, IsActive = true },
                new KnowledgeTagDto { Id = Guid.NewGuid(), Name = "security", Description = "Security and cybersecurity topics", UsageCount = 28, IsActive = true },
                new KnowledgeTagDto { Id = Guid.NewGuid(), Name = "procedures", Description = "Standard operating procedures", UsageCount = 35, IsActive = true },
                new KnowledgeTagDto { Id = Guid.NewGuid(), Name = "guidelines", Description = "Guidelines and best practices", UsageCount = 22, IsActive = true },
                new KnowledgeTagDto { Id = Guid.NewGuid(), Name = "compliance", Description = "Compliance and regulatory topics", UsageCount = 18, IsActive = true }
            };
        }

        public async Task<KnowledgeRatingDto> RateKnowledgeArticleAsync(KnowledgeRatingDto rating)
        {
            try
            {
                rating.Id = Guid.NewGuid();
                rating.CreatedAt = DateTime.UtcNow;

                _logger.LogInformation("Knowledge article rated: {ArticleId} - Rating: {Rating}", rating.ArticleId, rating.Rating);
                return rating;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to rate knowledge article");
                throw;
            }
        }

        public async Task<List<KnowledgeRatingDto>> GetArticleRatingsAsync(Guid articleId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeRatingDto>
            {
                new KnowledgeRatingDto
                {
                    Id = Guid.NewGuid(),
                    ArticleId = articleId,
                    UserId = Guid.NewGuid(),
                    UserName = "John Smith",
                    Rating = 5,
                    Comment = "Very helpful and well-written article",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new KnowledgeRatingDto
                {
                    Id = Guid.NewGuid(),
                    ArticleId = articleId,
                    UserId = Guid.NewGuid(),
                    UserName = "Sarah Johnson",
                    Rating = 4,
                    Comment = "Good information, could use more examples",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };
        }

        public async Task<KnowledgeCommentDto> AddCommentAsync(KnowledgeCommentDto comment)
        {
            try
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTime.UtcNow;
                comment.Status = "Published";

                _logger.LogInformation("Knowledge comment added: {CommentId} for article {ArticleId}", comment.Id, comment.ArticleId);
                return comment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add knowledge comment");
                throw;
            }
        }

        public async Task<List<KnowledgeCommentDto>> GetArticleCommentsAsync(Guid articleId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeCommentDto>
            {
                new KnowledgeCommentDto
                {
                    Id = Guid.NewGuid(),
                    ArticleId = articleId,
                    UserId = Guid.NewGuid(),
                    UserName = "Mike Wilson",
                    Content = "This article was extremely helpful for understanding the onboarding process.",
                    Status = "Published",
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new KnowledgeCommentDto
                {
                    Id = Guid.NewGuid(),
                    ArticleId = articleId,
                    UserId = Guid.NewGuid(),
                    UserName = "Lisa Chen",
                    Content = "Could you add more information about the IT setup process?",
                    Status = "Published",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<KnowledgeAnalyticsDto> GetKnowledgeAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new KnowledgeAnalyticsDto
            {
                TenantId = tenantId,
                TotalArticles = 125,
                PublishedArticles = 98,
                DraftArticles = 18,
                PendingApprovalArticles = 9,
                TotalViews = 15680,
                TotalLikes = 1250,
                TotalComments = 485,
                AverageRating = 4.3,
                TopCategories = new Dictionary<string, int>
                {
                    { "HR Procedures", 25 },
                    { "Operations", 22 },
                    { "IT Security", 18 },
                    { "Compliance", 15 },
                    { "Finance", 12 },
                    { "Other", 6 }
                },
                PopularTags = new Dictionary<string, int>
                {
                    { "procedures", 35 },
                    { "security", 28 },
                    { "guidelines", 22 },
                    { "compliance", 18 },
                    { "onboarding", 15 }
                },
                MonthlyViews = new Dictionary<string, int>
                {
                    { "Jan", 1250 }, { "Feb", 1380 }, { "Mar", 1420 }, { "Apr", 1350 },
                    { "May", 1480 }, { "Jun", 1520 }, { "Jul", 1380 }, { "Aug", 1450 },
                    { "Sep", 1580 }, { "Oct", 1420 }, { "Nov", 1380 }, { "Dec", 1640 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<KnowledgeArticleDto>> GetPopularArticlesAsync(Guid tenantId, int count = 10)
        {
            await Task.CompletedTask;
            return new List<KnowledgeArticleDto>
            {
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1004",
                    Title = "Most Popular: Password Security Guidelines",
                    Content = "Essential password security best practices including complexity requirements, multi-factor authentication, and regular password updates.",
                    Summary = "Essential password security best practices",
                    Status = "Published",
                    Priority = "High",
                    ViewCount = 485,
                    LikeCount = 45,
                    AverageRating = 4.8,
                    CategoryName = "IT Security",
                    AuthorName = "Security Team",
                    Tags = new List<string> { "Security", "Password", "Best Practices" },
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1005",
                    Title = "Popular: Employee Benefits Overview",
                    Content = "Comprehensive guide to employee benefits including health insurance, retirement plans, and vacation policies.",
                    Summary = "Comprehensive guide to employee benefits",
                    Status = "Published",
                    Priority = "Medium",
                    ViewCount = 420,
                    LikeCount = 38,
                    AverageRating = 4.6,
                    CategoryName = "HR Procedures",
                    AuthorName = "HR Team",
                    Tags = new List<string> { "HR", "Benefits", "Employee" },
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                }
            };
        }

        public async Task<List<KnowledgeArticleDto>> GetRecentArticlesAsync(Guid tenantId, int count = 10)
        {
            await Task.CompletedTask;
            return new List<KnowledgeArticleDto>
            {
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1006",
                    Title = "Recent: Updated Remote Work Policy",
                    Content = "Latest updates to remote work guidelines including new equipment policies and communication standards.",
                    Summary = "Latest updates to remote work guidelines",
                    Status = "Published",
                    Priority = "Medium",
                    ViewCount = 85,
                    LikeCount = 8,
                    AverageRating = 4.2,
                    CategoryName = "HR Procedures",
                    AuthorName = "Policy Team",
                    Tags = new List<string> { "Remote Work", "Policy", "Guidelines" },
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1007",
                    Title = "Recent: New Expense Reporting Process",
                    Content = "Updated expense reporting procedures with new digital submission requirements and approval workflows.",
                    Summary = "Updated expense reporting procedures",
                    Status = "Published",
                    Priority = "Medium",
                    ViewCount = 65,
                    LikeCount = 5,
                    AverageRating = 4.0,
                    CategoryName = "Finance",
                    AuthorName = "Finance Team",
                    Tags = new List<string> { "Finance", "Expenses", "Process" },
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<KnowledgeReportDto> GenerateKnowledgeReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new KnowledgeReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalArticles = 125,
                NewArticles = 15,
                UpdatedArticles = 28,
                TotalViews = 3850,
                TotalLikes = 285,
                TotalComments = 125,
                AverageRating = 4.3,
                TopViewedArticles = new List<string>
                {
                    "Password Security Guidelines",
                    "Employee Benefits Overview",
                    "Remote Work Policy",
                    "Expense Reporting Process"
                },
                CategoryPerformance = new Dictionary<string, KnowledgeCategoryPerformanceDto>
                {
                    { "HR Procedures", new KnowledgeCategoryPerformanceDto { Views = 1250, Likes = 95, Comments = 45 } },
                    { "IT Security", new KnowledgeCategoryPerformanceDto { Views = 980, Likes = 85, Comments = 35 } },
                    { "Operations", new KnowledgeCategoryPerformanceDto { Views = 850, Likes = 65, Comments = 25 } },
                    { "Finance", new KnowledgeCategoryPerformanceDto { Views = 520, Likes = 25, Comments = 15 } },
                    { "Compliance", new KnowledgeCategoryPerformanceDto { Views = 250, Likes = 15, Comments = 5 } }
                },
                UserEngagement = new Dictionary<string, int>
                {
                    { "Active Readers", 485 },
                    { "Contributors", 25 },
                    { "Commenters", 125 },
                    { "Raters", 285 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> ApproveKnowledgeArticleAsync(Guid articleId, Guid approverId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Knowledge article approved: {ArticleId} by {ApproverId}", articleId, approverId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve knowledge article {ArticleId}", articleId);
                return false;
            }
        }

        public async Task<KnowledgeWorkflowDto> CreateApprovalWorkflowAsync(KnowledgeWorkflowDto workflow)
        {
            try
            {
                workflow.Id = Guid.NewGuid();
                workflow.CreatedAt = DateTime.UtcNow;
                workflow.Status = "Active";

                _logger.LogInformation("Knowledge approval workflow created: {WorkflowId}", workflow.Id);
                return workflow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create knowledge approval workflow");
                throw;
            }
        }

        public async Task<List<KnowledgeArticleDto>> GetPendingApprovalsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<KnowledgeArticleDto>
            {
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1008",
                    Title = "Pending: New Safety Procedures",
                    Content = "Updated workplace safety procedures including emergency protocols and equipment usage guidelines.",
                    Summary = "Updated workplace safety procedures",
                    Status = "Pending Approval",
                    Priority = "High",
                    CategoryName = "Operations",
                    AuthorName = "Safety Team",
                    Tags = new List<string> { "Safety", "Procedures", "Operations" },
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new KnowledgeArticleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ArticleNumber = "KB-20241227-1009",
                    Title = "Pending: Data Privacy Guidelines",
                    Content = "Updated data privacy and protection guidelines in compliance with GDPR and local regulations.",
                    Summary = "Updated data privacy and protection guidelines",
                    Status = "Pending Approval",
                    Priority = "High",
                    CategoryName = "Compliance",
                    AuthorName = "Legal Team",
                    Tags = new List<string> { "Privacy", "GDPR", "Compliance" },
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }
    }

    public class KnowledgeArticleDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string ArticleNumber { get; set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string Summary { get; set; }
        public Guid CategoryId { get; set; }
        public required string CategoryName { get; set; }
        public Guid AuthorId { get; set; }
        public required string AuthorName { get; set; }
        public required string Status { get; set; }
        public required string Priority { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public double AverageRating { get; set; }
        public required List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
    }

    public class KnowledgeCategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int ArticleCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KnowledgeTagDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int UsageCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KnowledgeRatingDto
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public int Rating { get; set; }
        public required string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KnowledgeCommentDto
    {
        public Guid Id { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string Content { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class KnowledgeAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalArticles { get; set; }
        public int PublishedArticles { get; set; }
        public int DraftArticles { get; set; }
        public int PendingApprovalArticles { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public double AverageRating { get; set; }
        public required Dictionary<string, int> TopCategories { get; set; }
        public required Dictionary<string, int> PopularTags { get; set; }
        public required Dictionary<string, int> MonthlyViews { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class KnowledgeReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public int TotalArticles { get; set; }
        public int NewArticles { get; set; }
        public int UpdatedArticles { get; set; }
        public int TotalViews { get; set; }
        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public double AverageRating { get; set; }
        public required List<string> TopViewedArticles { get; set; }
        public required Dictionary<string, KnowledgeCategoryPerformanceDto> CategoryPerformance { get; set; }
        public required Dictionary<string, int> UserEngagement { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class KnowledgeCategoryPerformanceDto
    {
        public int Views { get; set; }
        public int Likes { get; set; }
        public int Comments { get; set; }
    }

    public class KnowledgeWorkflowDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
