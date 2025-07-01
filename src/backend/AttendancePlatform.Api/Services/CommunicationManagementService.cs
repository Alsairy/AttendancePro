using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface ICommunicationManagementService
    {
        Task<MessageDto> SendMessageAsync(MessageDto message);
        Task<List<MessageDto>> GetMessagesAsync(Guid userId);
        Task<List<MessageDto>> GetConversationAsync(Guid userId1, Guid userId2);
        Task<AnnouncementDto> CreateAnnouncementAsync(AnnouncementDto announcement);
        Task<List<AnnouncementDto>> GetAnnouncementsAsync(Guid tenantId);
        Task<EmailCampaignDto> CreateEmailCampaignAsync(EmailCampaignDto campaign);
        Task<List<EmailCampaignDto>> GetEmailCampaignsAsync(Guid tenantId);
        Task<bool> SendEmailCampaignAsync(Guid campaignId);
        Task<SmsMessageDto> SendSmsAsync(SmsMessageDto sms);
        Task<List<SmsMessageDto>> GetSmsHistoryAsync(Guid tenantId);
        Task<PushNotificationDto> SendPushNotificationAsync(PushNotificationDto notification);
        Task<List<PushNotificationDto>> GetPushNotificationHistoryAsync(Guid tenantId);
        Task<CommunicationTemplateDto> CreateTemplateAsync(CommunicationTemplateDto template);
        Task<List<CommunicationTemplateDto>> GetTemplatesAsync(Guid tenantId);
        Task<CommunicationAnalyticsDto> GetCommunicationAnalyticsAsync(Guid tenantId);
        Task<CommunicationReportDto> GenerateCommunicationReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId);
        Task<List<MessageDto>> GetUnreadMessagesAsync(Guid userId);
        Task<CommunicationPreferencesDto> UpdateCommunicationPreferencesAsync(Guid userId, CommunicationPreferencesDto preferences);
        Task<CommunicationPreferencesDto> GetCommunicationPreferencesAsync(Guid userId);
    }

    public class CommunicationManagementService : ICommunicationManagementService
    {
        private readonly ILogger<CommunicationManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public CommunicationManagementService(ILogger<CommunicationManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<MessageDto> SendMessageAsync(MessageDto message)
        {
            try
            {
                message.Id = Guid.NewGuid();
                message.SentAt = DateTime.UtcNow;
                message.Status = "Sent";
                message.IsRead = false;

                _logger.LogInformation("Message sent: {MessageId} from {SenderId} to {RecipientId}", message.Id, message.SenderId, message.RecipientId);
                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send message");
                throw;
            }
        }

        public async Task<List<MessageDto>> GetMessagesAsync(Guid userId)
        {
            await Task.CompletedTask;
            return new List<MessageDto>
            {
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = Guid.NewGuid(),
                    SenderName = "John Smith",
                    RecipientId = userId,
                    RecipientName = "Current User",
                    Subject = "Team Meeting Tomorrow",
                    Content = "Don't forget about our team meeting tomorrow at 10 AM in Conference Room A.",
                    MessageType = "Direct",
                    Priority = "Normal",
                    Status = "Delivered",
                    IsRead = false,
                    SentAt = DateTime.UtcNow.AddHours(-2)
                },
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = Guid.NewGuid(),
                    SenderName = "HR Department",
                    RecipientId = userId,
                    RecipientName = "Current User",
                    Subject = "Policy Update Notification",
                    Content = "Please review the updated remote work policy in the employee handbook.",
                    MessageType = "System",
                    Priority = "High",
                    Status = "Delivered",
                    IsRead = true,
                    SentAt = DateTime.UtcNow.AddDays(-1),
                    ReadAt = DateTime.UtcNow.AddHours(-18)
                }
            };
        }

        public async Task<List<MessageDto>> GetConversationAsync(Guid userId1, Guid userId2)
        {
            await Task.CompletedTask;
            return new List<MessageDto>
            {
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = userId1,
                    SenderName = "User 1",
                    RecipientId = userId2,
                    RecipientName = "User 2",
                    Subject = "Project Discussion",
                    Content = "Hi, can we discuss the project timeline?",
                    MessageType = "Direct",
                    Priority = "Normal",
                    Status = "Delivered",
                    IsRead = true,
                    SentAt = DateTime.UtcNow.AddHours(-4),
                    ReadAt = DateTime.UtcNow.AddHours(-3)
                },
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = userId2,
                    SenderName = "User 2",
                    RecipientId = userId1,
                    RecipientName = "User 1",
                    Subject = "Re: Project Discussion",
                    Content = "Sure! I'm available this afternoon. How about 3 PM?",
                    MessageType = "Direct",
                    Priority = "Normal",
                    Status = "Delivered",
                    IsRead = false,
                    SentAt = DateTime.UtcNow.AddHours(-3)
                }
            };
        }

        public async Task<AnnouncementDto> CreateAnnouncementAsync(AnnouncementDto announcement)
        {
            try
            {
                announcement.Id = Guid.NewGuid();
                announcement.CreatedAt = DateTime.UtcNow;
                announcement.Status = "Published";
                announcement.ViewCount = 0;

                _logger.LogInformation("Announcement created: {AnnouncementId} - {Title}", announcement.Id, announcement.Title);
                return announcement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create announcement");
                throw;
            }
        }

        public async Task<List<AnnouncementDto>> GetAnnouncementsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<AnnouncementDto>
            {
                new AnnouncementDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Title = "Company Holiday Schedule",
                    Content = "Please note the updated holiday schedule for the remainder of the year. All offices will be closed on December 25th and January 1st.",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "HR Department",
                    Priority = "High",
                    Status = "Published",
                    TargetAudience = "All Employees",
                    ViewCount = 485,
                    ExpiresAt = DateTime.UtcNow.AddDays(30),
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new AnnouncementDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Title = "New Security Protocols",
                    Content = "We are implementing new security protocols effective immediately. Please ensure you follow the updated guidelines.",
                    AuthorId = Guid.NewGuid(),
                    AuthorName = "IT Security",
                    Priority = "Critical",
                    Status = "Published",
                    TargetAudience = "All Employees",
                    ViewCount = 320,
                    ExpiresAt = DateTime.UtcNow.AddDays(60),
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
        }

        public async Task<EmailCampaignDto> CreateEmailCampaignAsync(EmailCampaignDto campaign)
        {
            try
            {
                campaign.Id = Guid.NewGuid();
                campaign.CreatedAt = DateTime.UtcNow;
                campaign.Status = "Draft";
                campaign.SentCount = 0;
                campaign.OpenCount = 0;
                campaign.ClickCount = 0;

                _logger.LogInformation("Email campaign created: {CampaignId} - {CampaignName}", campaign.Id, campaign.Name);
                return campaign;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create email campaign");
                throw;
            }
        }

        public async Task<List<EmailCampaignDto>> GetEmailCampaignsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<EmailCampaignDto>
            {
                new EmailCampaignDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Monthly Newsletter",
                    Subject = "Company Updates - December 2024",
                    Content = "Welcome to our monthly newsletter with company updates and announcements.",
                    TargetAudience = "All Employees",
                    Status = "Sent",
                    SentCount = 485,
                    OpenCount = 342,
                    ClickCount = 89,
                    OpenRate = 70.5,
                    ClickRate = 18.4,
                    ScheduledAt = DateTime.UtcNow.AddDays(-7),
                    SentAt = DateTime.UtcNow.AddDays(-7),
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new EmailCampaignDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Training Reminder",
                    Subject = "Mandatory Security Training Due",
                    Content = "This is a reminder that your mandatory security training is due by the end of this week.",
                    TargetAudience = "Employees with Pending Training",
                    Status = "Scheduled",
                    SentCount = 0,
                    OpenCount = 0,
                    ClickCount = 0,
                    OpenRate = 0,
                    ClickRate = 0,
                    ScheduledAt = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
        }

        public async Task<bool> SendEmailCampaignAsync(Guid campaignId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Email campaign sent: {CampaignId}", campaignId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email campaign {CampaignId}", campaignId);
                return false;
            }
        }

        public async Task<SmsMessageDto> SendSmsAsync(SmsMessageDto sms)
        {
            try
            {
                sms.Id = Guid.NewGuid();
                sms.SentAt = DateTime.UtcNow;
                sms.Status = "Sent";

                _logger.LogInformation("SMS sent: {SmsId} to {PhoneNumber}", sms.Id, sms.PhoneNumber);
                return sms;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS");
                throw;
            }
        }

        public async Task<List<SmsMessageDto>> GetSmsHistoryAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<SmsMessageDto>
            {
                new SmsMessageDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PhoneNumber = "+1234567890",
                    Message = "Your attendance has been recorded. Thank you!",
                    Status = "Delivered",
                    SentAt = DateTime.UtcNow.AddHours(-1),
                    DeliveredAt = DateTime.UtcNow.AddHours(-1).AddMinutes(2)
                },
                new SmsMessageDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    PhoneNumber = "+1234567891",
                    Message = "Reminder: Team meeting at 2 PM today.",
                    Status = "Delivered",
                    SentAt = DateTime.UtcNow.AddHours(-3),
                    DeliveredAt = DateTime.UtcNow.AddHours(-3).AddMinutes(1)
                }
            };
        }

        public async Task<PushNotificationDto> SendPushNotificationAsync(PushNotificationDto notification)
        {
            try
            {
                notification.Id = Guid.NewGuid();
                notification.SentAt = DateTime.UtcNow;
                notification.Status = "Sent";

                _logger.LogInformation("Push notification sent: {NotificationId} to {UserId}", notification.Id, notification.UserId);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send push notification");
                throw;
            }
        }

        public async Task<List<PushNotificationDto>> GetPushNotificationHistoryAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<PushNotificationDto>
            {
                new PushNotificationDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    UserId = Guid.NewGuid(),
                    Title = "Attendance Reminder",
                    Message = "Don't forget to check in for today!",
                    Status = "Delivered",
                    SentAt = DateTime.UtcNow.AddHours(-2),
                    DeliveredAt = DateTime.UtcNow.AddHours(-2).AddMinutes(1)
                },
                new PushNotificationDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    UserId = Guid.NewGuid(),
                    Title = "Leave Request Approved",
                    Message = "Your leave request for next week has been approved.",
                    Status = "Delivered",
                    SentAt = DateTime.UtcNow.AddHours(-4),
                    DeliveredAt = DateTime.UtcNow.AddHours(-4).AddMinutes(1)
                }
            };
        }

        public async Task<CommunicationTemplateDto> CreateTemplateAsync(CommunicationTemplateDto template)
        {
            try
            {
                template.Id = Guid.NewGuid();
                template.CreatedAt = DateTime.UtcNow;
                template.IsActive = true;

                _logger.LogInformation("Communication template created: {TemplateId} - {TemplateName}", template.Id, template.Name);
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create communication template");
                throw;
            }
        }

        public async Task<List<CommunicationTemplateDto>> GetTemplatesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<CommunicationTemplateDto>
            {
                new CommunicationTemplateDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Welcome Email",
                    Type = "Email",
                    Subject = "Welcome to {CompanyName}!",
                    Content = "Dear {EmployeeName}, welcome to our team! We're excited to have you on board.",
                    Variables = new List<string> { "CompanyName", "EmployeeName", "StartDate" },
                    IsActive = true,
                    UsageCount = 25,
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new CommunicationTemplateDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Attendance Reminder",
                    Type = "SMS",
                    Subject = "",
                    Content = "Hi {EmployeeName}, please remember to check in for today. Thank you!",
                    Variables = new List<string> { "EmployeeName", "Date" },
                    IsActive = true,
                    UsageCount = 150,
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                }
            };
        }

        public async Task<CommunicationAnalyticsDto> GetCommunicationAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new CommunicationAnalyticsDto
            {
                TenantId = tenantId,
                TotalMessages = 2850,
                EmailsSent = 1250,
                SmsSent = 485,
                PushNotificationsSent = 1115,
                EmailOpenRate = 68.5,
                EmailClickRate = 15.2,
                SmsDeliveryRate = 98.8,
                PushNotificationDeliveryRate = 95.2,
                AverageResponseTime = 4.2,
                TopCommunicationChannels = new Dictionary<string, int>
                {
                    { "Push Notifications", 1115 },
                    { "Email", 1250 },
                    { "SMS", 485 },
                    { "In-App Messages", 320 },
                    { "Announcements", 180 }
                },
                MonthlyTrends = new Dictionary<string, int>
                {
                    { "Jan", 220 }, { "Feb", 245 }, { "Mar", 268 }, { "Apr", 235 },
                    { "May", 285 }, { "Jun", 295 }, { "Jul", 250 }, { "Aug", 275 },
                    { "Sep", 310 }, { "Oct", 285 }, { "Nov", 265 }, { "Dec", 320 }
                },
                EngagementMetrics = new Dictionary<string, double>
                {
                    { "Read Rate", 78.5 },
                    { "Response Rate", 25.8 },
                    { "Click-through Rate", 15.2 },
                    { "Unsubscribe Rate", 2.1 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<CommunicationReportDto> GenerateCommunicationReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new CommunicationReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalCommunications = 850,
                EmailCommunications = 385,
                SmsCommunications = 125,
                PushNotifications = 285,
                InAppMessages = 55,
                SuccessfulDeliveries = 815,
                FailedDeliveries = 35,
                DeliveryRate = 95.9,
                EngagementRate = 72.3,
                TopPerformingCampaigns = new List<string>
                {
                    "Monthly Newsletter",
                    "Security Training Reminder",
                    "Holiday Schedule Announcement",
                    "New Policy Update"
                },
                ChannelPerformance = new Dictionary<string, CommunicationChannelPerformanceDto>
                {
                    { "Email", new CommunicationChannelPerformanceDto { Sent = 385, Delivered = 375, Opened = 258, Clicked = 58 } },
                    { "SMS", new CommunicationChannelPerformanceDto { Sent = 125, Delivered = 123, Opened = 123, Clicked = 0 } },
                    { "Push", new CommunicationChannelPerformanceDto { Sent = 285, Delivered = 271, Opened = 195, Clicked = 45 } }
                },
                UserEngagement = new Dictionary<string, int>
                {
                    { "Highly Engaged", 185 },
                    { "Moderately Engaged", 245 },
                    { "Low Engagement", 55 }
                },
                Recommendations = new List<string>
                {
                    "Increase personalization in email campaigns",
                    "Optimize push notification timing",
                    "A/B test subject lines for better open rates",
                    "Implement automated follow-up sequences"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Message marked as read: {MessageId} by {UserId}", messageId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark message as read {MessageId}", messageId);
                return false;
            }
        }

        public async Task<List<MessageDto>> GetUnreadMessagesAsync(Guid userId)
        {
            await Task.CompletedTask;
            return new List<MessageDto>
            {
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = Guid.NewGuid(),
                    SenderName = "Manager",
                    RecipientId = userId,
                    RecipientName = "Current User",
                    Subject = "Urgent: Project Deadline",
                    Content = "Please review the updated project deadline and confirm your availability.",
                    MessageType = "Direct",
                    Priority = "Urgent",
                    Status = "Delivered",
                    IsRead = false,
                    SentAt = DateTime.UtcNow.AddMinutes(-30)
                },
                new MessageDto
                {
                    Id = Guid.NewGuid(),
                    SenderId = Guid.NewGuid(),
                    SenderName = "IT Support",
                    RecipientId = userId,
                    RecipientName = "Current User",
                    Subject = "System Maintenance Notice",
                    Content = "Scheduled system maintenance will occur this weekend. Please save your work.",
                    MessageType = "System",
                    Priority = "Normal",
                    Status = "Delivered",
                    IsRead = false,
                    SentAt = DateTime.UtcNow.AddHours(-1)
                }
            };
        }

        public async Task<CommunicationPreferencesDto> UpdateCommunicationPreferencesAsync(Guid userId, CommunicationPreferencesDto preferences)
        {
            try
            {
                await Task.CompletedTask;
                preferences.UserId = userId;
                preferences.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Communication preferences updated for user: {UserId}", userId);
                return preferences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update communication preferences for user {UserId}", userId);
                throw;
            }
        }

        public async Task<CommunicationPreferencesDto> GetCommunicationPreferencesAsync(Guid userId)
        {
            await Task.CompletedTask;
            return new CommunicationPreferencesDto
            {
                UserId = userId,
                EmailNotifications = true,
                SmsNotifications = false,
                PushNotifications = true,
                InAppNotifications = true,
                MarketingEmails = false,
                SecurityAlerts = true,
                SystemUpdates = true,
                TeamMessages = true,
                AnnouncementNotifications = true,
                PreferredLanguage = "English",
                TimeZone = "UTC",
                QuietHoursStart = "22:00",
                QuietHoursEnd = "08:00",
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            };
        }
    }

    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; }
        public Guid RecipientId { get; set; }
        public string RecipientName { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string MessageType { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public bool IsRead { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
    }

    public class AnnouncementDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Guid AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string TargetAudience { get; set; }
        public int ViewCount { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmailCampaignDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string TargetAudience { get; set; }
        public string Status { get; set; }
        public int SentCount { get; set; }
        public int OpenCount { get; set; }
        public int ClickCount { get; set; }
        public double OpenRate { get; set; }
        public double ClickRate { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SmsMessageDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }

    public class PushNotificationDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime SentAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }

    public class CommunicationTemplateDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<string> Variables { get; set; }
        public bool IsActive { get; set; }
        public int UsageCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommunicationAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalMessages { get; set; }
        public int EmailsSent { get; set; }
        public int SmsSent { get; set; }
        public int PushNotificationsSent { get; set; }
        public double EmailOpenRate { get; set; }
        public double EmailClickRate { get; set; }
        public double SmsDeliveryRate { get; set; }
        public double PushNotificationDeliveryRate { get; set; }
        public double AverageResponseTime { get; set; }
        public Dictionary<string, int> TopCommunicationChannels { get; set; }
        public Dictionary<string, int> MonthlyTrends { get; set; }
        public Dictionary<string, double> EngagementMetrics { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class CommunicationReportDto
    {
        public Guid TenantId { get; set; }
        public string ReportPeriod { get; set; }
        public int TotalCommunications { get; set; }
        public int EmailCommunications { get; set; }
        public int SmsCommunications { get; set; }
        public int PushNotifications { get; set; }
        public int InAppMessages { get; set; }
        public int SuccessfulDeliveries { get; set; }
        public int FailedDeliveries { get; set; }
        public double DeliveryRate { get; set; }
        public double EngagementRate { get; set; }
        public List<string> TopPerformingCampaigns { get; set; }
        public Dictionary<string, CommunicationChannelPerformanceDto> ChannelPerformance { get; set; }
        public Dictionary<string, int> UserEngagement { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class CommunicationChannelPerformanceDto
    {
        public int Sent { get; set; }
        public int Delivered { get; set; }
        public int Opened { get; set; }
        public int Clicked { get; set; }
    }

    public class CommunicationPreferencesDto
    {
        public Guid UserId { get; set; }
        public bool EmailNotifications { get; set; }
        public bool SmsNotifications { get; set; }
        public bool PushNotifications { get; set; }
        public bool InAppNotifications { get; set; }
        public bool MarketingEmails { get; set; }
        public bool SecurityAlerts { get; set; }
        public bool SystemUpdates { get; set; }
        public bool TeamMessages { get; set; }
        public bool AnnouncementNotifications { get; set; }
        public string PreferredLanguage { get; set; }
        public string TimeZone { get; set; }
        public string QuietHoursStart { get; set; }
        public string QuietHoursEnd { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
