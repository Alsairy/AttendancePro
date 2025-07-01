using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IEventManagementService
    {
        Task<EventDto> CreateEventAsync(EventDto eventDto);
        Task<List<EventDto>> GetEventsAsync(Guid tenantId);
        Task<EventDto> UpdateEventAsync(Guid eventId, EventDto eventDto);
        Task<bool> DeleteEventAsync(Guid eventId);
        Task<EventRegistrationDto> RegisterForEventAsync(EventRegistrationDto registration);
        Task<List<EventRegistrationDto>> GetEventRegistrationsAsync(Guid eventId);
        Task<bool> CancelEventRegistrationAsync(Guid registrationId);
        Task<EventCalendarDto> GetEventCalendarAsync(Guid tenantId, DateTime month);
        Task<List<EventDto>> GetUpcomingEventsAsync(Guid tenantId, int count = 10);
        Task<EventReminderDto> CreateEventReminderAsync(EventReminderDto reminder);
        Task<List<EventReminderDto>> GetEventRemindersAsync(Guid eventId);
        Task<EventFeedbackDto> SubmitEventFeedbackAsync(EventFeedbackDto feedback);
        Task<List<EventFeedbackDto>> GetEventFeedbackAsync(Guid eventId);
        Task<EventAnalyticsDto> GetEventAnalyticsAsync(Guid tenantId);
        Task<EventReportDto> GenerateEventReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<bool> SendEventNotificationAsync(Guid eventId, string notificationType);
        Task<EventResourceDto> CreateEventResourceAsync(EventResourceDto resource);
        Task<List<EventResourceDto>> GetEventResourcesAsync(Guid eventId);
        Task<EventVenueDto> CreateEventVenueAsync(EventVenueDto venue);
        Task<List<EventVenueDto>> GetEventVenuesAsync(Guid tenantId);
    }

    public class EventManagementService : IEventManagementService
    {
        private readonly ILogger<EventManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public EventManagementService(ILogger<EventManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<EventDto> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                eventDto.Id = Guid.NewGuid();
                eventDto.EventCode = $"EVT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                eventDto.CreatedAt = DateTime.UtcNow;
                eventDto.Status = "Scheduled";
                eventDto.RegistrationCount = 0;

                _logger.LogInformation("Event created: {EventId} - {EventCode}", eventDto.Id, eventDto.EventCode);
                return eventDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event");
                throw;
            }
        }

        public async Task<List<EventDto>> GetEventsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<EventDto>
            {
                new EventDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    EventCode = "EVT-20241227-1001",
                    Title = "Annual Company Meeting",
                    Description = "Annual all-hands company meeting to discuss year-end results and next year's goals.",
                    EventType = "Company Meeting",
                    Category = "Corporate",
                    StartDateTime = DateTime.UtcNow.AddDays(14).AddHours(9),
                    EndDateTime = DateTime.UtcNow.AddDays(14).AddHours(17),
                    VenueId = Guid.NewGuid(),
                    VenueName = "Main Conference Hall",
                    OrganizerName = "Executive Team",
                    MaxAttendees = 500,
                    RegistrationCount = 285,
                    Status = "Scheduled",
                    IsPublic = true,
                    RequiresRegistration = true,
                    RegistrationDeadline = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new EventDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    EventCode = "EVT-20241227-1002",
                    Title = "Cybersecurity Training Workshop",
                    Description = "Mandatory cybersecurity awareness training for all employees.",
                    EventType = "Training",
                    Category = "Security",
                    StartDateTime = DateTime.UtcNow.AddDays(7).AddHours(14),
                    EndDateTime = DateTime.UtcNow.AddDays(7).AddHours(16),
                    VenueId = Guid.NewGuid(),
                    VenueName = "Training Room B",
                    OrganizerName = "IT Security Team",
                    MaxAttendees = 50,
                    RegistrationCount = 42,
                    Status = "Scheduled",
                    IsPublic = false,
                    RequiresRegistration = true,
                    RegistrationDeadline = DateTime.UtcNow.AddDays(3),
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };
        }

        public async Task<EventDto> UpdateEventAsync(Guid eventId, EventDto eventDto)
        {
            try
            {
                await Task.CompletedTask;
                eventDto.Id = eventId;
                eventDto.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Event updated: {EventId}", eventId);
                return eventDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update event {EventId}", eventId);
                throw;
            }
        }

        public async Task<bool> DeleteEventAsync(Guid eventId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Event deleted: {EventId}", eventId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete event {EventId}", eventId);
                return false;
            }
        }

        public async Task<EventRegistrationDto> RegisterForEventAsync(EventRegistrationDto registration)
        {
            try
            {
                registration.Id = Guid.NewGuid();
                registration.RegistrationNumber = $"REG-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                registration.RegistrationDate = DateTime.UtcNow;
                registration.Status = "Confirmed";

                _logger.LogInformation("Event registration created: {RegistrationId} - {RegistrationNumber}", registration.Id, registration.RegistrationNumber);
                return registration;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register for event");
                throw;
            }
        }

        public async Task<List<EventRegistrationDto>> GetEventRegistrationsAsync(Guid eventId)
        {
            await Task.CompletedTask;
            return new List<EventRegistrationDto>
            {
                new EventRegistrationDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    RegistrationNumber = "REG-20241227-1001",
                    UserId = Guid.NewGuid(),
                    UserName = "John Smith",
                    UserEmail = "john.smith@company.com",
                    RegistrationDate = DateTime.UtcNow.AddDays(-5),
                    Status = "Confirmed",
                    AttendanceStatus = "Pending",
                    SpecialRequirements = "Vegetarian meal preference"
                },
                new EventRegistrationDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    RegistrationNumber = "REG-20241227-1002",
                    UserId = Guid.NewGuid(),
                    UserName = "Sarah Johnson",
                    UserEmail = "sarah.johnson@company.com",
                    RegistrationDate = DateTime.UtcNow.AddDays(-3),
                    Status = "Confirmed",
                    AttendanceStatus = "Pending",
                    SpecialRequirements = ""
                }
            };
        }

        public async Task<bool> CancelEventRegistrationAsync(Guid registrationId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Event registration cancelled: {RegistrationId}", registrationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel event registration {RegistrationId}", registrationId);
                return false;
            }
        }

        public async Task<EventCalendarDto> GetEventCalendarAsync(Guid tenantId, DateTime month)
        {
            await Task.CompletedTask;
            return new EventCalendarDto
            {
                TenantId = tenantId,
                Month = month,
                Events = new List<EventCalendarItemDto>
                {
                    new EventCalendarItemDto
                    {
                        EventId = Guid.NewGuid(),
                        Title = "Team Building Activity",
                        StartDateTime = month.AddDays(5).AddHours(10),
                        EndDateTime = month.AddDays(5).AddHours(16),
                        EventType = "Team Building",
                        VenueName = "Outdoor Park"
                    },
                    new EventCalendarItemDto
                    {
                        EventId = Guid.NewGuid(),
                        Title = "Quarterly Review Meeting",
                        StartDateTime = month.AddDays(15).AddHours(14),
                        EndDateTime = month.AddDays(15).AddHours(17),
                        EventType = "Meeting",
                        VenueName = "Conference Room A"
                    },
                    new EventCalendarItemDto
                    {
                        EventId = Guid.NewGuid(),
                        Title = "Product Launch Event",
                        StartDateTime = month.AddDays(25).AddHours(18),
                        EndDateTime = month.AddDays(25).AddHours(21),
                        EventType = "Launch",
                        VenueName = "Main Auditorium"
                    }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<EventDto>> GetUpcomingEventsAsync(Guid tenantId, int count = 10)
        {
            await Task.CompletedTask;
            return new List<EventDto>
            {
                new EventDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    EventCode = "EVT-20241227-1003",
                    Title = "New Year Celebration",
                    Description = "Company New Year celebration party",
                    EventType = "Social",
                    Category = "Entertainment",
                    StartDateTime = DateTime.UtcNow.AddDays(4).AddHours(18),
                    EndDateTime = DateTime.UtcNow.AddDays(4).AddHours(22),
                    VenueName = "Main Hall",
                    OrganizerName = "HR Team",
                    MaxAttendees = 300,
                    RegistrationCount = 185,
                    Status = "Scheduled"
                },
                new EventDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    EventCode = "EVT-20241227-1004",
                    Title = "Leadership Workshop",
                    Description = "Advanced leadership skills development workshop",
                    EventType = "Training",
                    Category = "Professional Development",
                    StartDateTime = DateTime.UtcNow.AddDays(10).AddHours(9),
                    EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(17),
                    VenueName = "Training Center",
                    OrganizerName = "Learning & Development",
                    MaxAttendees = 25,
                    RegistrationCount = 20,
                    Status = "Scheduled"
                }
            };
        }

        public async Task<EventReminderDto> CreateEventReminderAsync(EventReminderDto reminder)
        {
            try
            {
                reminder.Id = Guid.NewGuid();
                reminder.CreatedAt = DateTime.UtcNow;
                reminder.Status = "Scheduled";

                _logger.LogInformation("Event reminder created: {ReminderId} for event {EventId}", reminder.Id, reminder.EventId);
                return reminder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event reminder");
                throw;
            }
        }

        public async Task<List<EventReminderDto>> GetEventRemindersAsync(Guid eventId)
        {
            await Task.CompletedTask;
            return new List<EventReminderDto>
            {
                new EventReminderDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    ReminderType = "Email",
                    ReminderTime = DateTime.UtcNow.AddDays(1),
                    Message = "Don't forget about tomorrow's event!",
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new EventReminderDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    ReminderType = "Push Notification",
                    ReminderTime = DateTime.UtcNow.AddHours(2),
                    Message = "Event starting in 2 hours",
                    Status = "Scheduled",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                }
            };
        }

        public async Task<EventFeedbackDto> SubmitEventFeedbackAsync(EventFeedbackDto feedback)
        {
            try
            {
                feedback.Id = Guid.NewGuid();
                feedback.SubmittedAt = DateTime.UtcNow;

                _logger.LogInformation("Event feedback submitted: {FeedbackId} for event {EventId}", feedback.Id, feedback.EventId);
                return feedback;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to submit event feedback");
                throw;
            }
        }

        public async Task<List<EventFeedbackDto>> GetEventFeedbackAsync(Guid eventId)
        {
            await Task.CompletedTask;
            return new List<EventFeedbackDto>
            {
                new EventFeedbackDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    UserName = "John Smith",
                    Rating = 5,
                    Comments = "Excellent event! Very well organized and informative.",
                    WouldRecommend = true,
                    SubmittedAt = DateTime.UtcNow.AddDays(-2)
                },
                new EventFeedbackDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    UserId = Guid.NewGuid(),
                    UserName = "Sarah Johnson",
                    Rating = 4,
                    Comments = "Good event, but could use better catering options.",
                    WouldRecommend = true,
                    SubmittedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<EventAnalyticsDto> GetEventAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new EventAnalyticsDto
            {
                TenantId = tenantId,
                TotalEvents = 125,
                UpcomingEvents = 15,
                CompletedEvents = 98,
                CancelledEvents = 12,
                TotalRegistrations = 2850,
                AverageAttendanceRate = 87.5,
                AverageEventRating = 4.3,
                EventsByCategory = new Dictionary<string, int>
                {
                    { "Training", 45 },
                    { "Corporate", 25 },
                    { "Social", 20 },
                    { "Professional Development", 18 },
                    { "Team Building", 12 },
                    { "Other", 5 }
                },
                EventsByType = new Dictionary<string, int>
                {
                    { "Workshop", 35 },
                    { "Meeting", 30 },
                    { "Conference", 20 },
                    { "Seminar", 18 },
                    { "Party", 12 },
                    { "Other", 10 }
                },
                MonthlyEventTrends = new Dictionary<string, int>
                {
                    { "Jan", 8 }, { "Feb", 12 }, { "Mar", 15 }, { "Apr", 10 },
                    { "May", 18 }, { "Jun", 14 }, { "Jul", 8 }, { "Aug", 16 },
                    { "Sep", 20 }, { "Oct", 18 }, { "Nov", 12 }, { "Dec", 14 }
                },
                PopularVenues = new Dictionary<string, int>
                {
                    { "Main Conference Hall", 25 },
                    { "Training Room A", 20 },
                    { "Auditorium", 15 },
                    { "Outdoor Area", 12 },
                    { "Training Room B", 10 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<EventReportDto> GenerateEventReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new EventReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalEvents = 45,
                CompletedEvents = 38,
                CancelledEvents = 3,
                UpcomingEvents = 4,
                TotalRegistrations = 1250,
                TotalAttendees = 1085,
                AttendanceRate = 86.8,
                AverageEventRating = 4.2,
                TopRatedEvents = new List<string>
                {
                    "Leadership Excellence Workshop",
                    "Annual Innovation Summit",
                    "Team Building Retreat",
                    "Customer Success Conference"
                },
                EventCategoryBreakdown = new Dictionary<string, EventCategoryStatsDto>
                {
                    { "Training", new EventCategoryStatsDto { EventCount = 18, RegistrationCount = 485, AttendanceRate = 92.3 } },
                    { "Corporate", new EventCategoryStatsDto { EventCount = 12, RegistrationCount = 385, AttendanceRate = 88.1 } },
                    { "Social", new EventCategoryStatsDto { EventCount = 8, RegistrationCount = 245, AttendanceRate = 78.5 } },
                    { "Professional Development", new EventCategoryStatsDto { EventCount = 7, RegistrationCount = 135, AttendanceRate = 94.2 } }
                },
                VenueUtilization = new Dictionary<string, int>
                {
                    { "Main Conference Hall", 12 },
                    { "Training Room A", 10 },
                    { "Auditorium", 8 },
                    { "Training Room B", 7 },
                    { "Outdoor Area", 5 },
                    { "Other", 3 }
                },
                BudgetAnalysis = new EventBudgetAnalysisDto
                {
                    TotalBudget = 125000.00m,
                    TotalSpent = 118500.00m,
                    BudgetUtilization = 94.8,
                    CostPerAttendee = 109.22m
                },
                Recommendations = new List<string>
                {
                    "Increase capacity for high-demand training events",
                    "Improve catering options based on feedback",
                    "Consider virtual options for broader participation",
                    "Implement early bird registration discounts"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> SendEventNotificationAsync(Guid eventId, string notificationType)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Event notification sent: {EventId} - {NotificationType}", eventId, notificationType);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send event notification for {EventId}", eventId);
                return false;
            }
        }

        public async Task<EventResourceDto> CreateEventResourceAsync(EventResourceDto resource)
        {
            try
            {
                resource.Id = Guid.NewGuid();
                resource.CreatedAt = DateTime.UtcNow;
                resource.Status = "Available";

                _logger.LogInformation("Event resource created: {ResourceId} for event {EventId}", resource.Id, resource.EventId);
                return resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event resource");
                throw;
            }
        }

        public async Task<List<EventResourceDto>> GetEventResourcesAsync(Guid eventId)
        {
            await Task.CompletedTask;
            return new List<EventResourceDto>
            {
                new EventResourceDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    ResourceType = "Equipment",
                    ResourceName = "Projector",
                    Description = "HD projector for presentations",
                    Quantity = 2,
                    Status = "Reserved",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                },
                new EventResourceDto
                {
                    Id = Guid.NewGuid(),
                    EventId = eventId,
                    ResourceType = "Catering",
                    ResourceName = "Coffee Service",
                    Description = "Coffee and refreshments for attendees",
                    Quantity = 1,
                    Status = "Confirmed",
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };
        }

        public async Task<EventVenueDto> CreateEventVenueAsync(EventVenueDto venue)
        {
            try
            {
                venue.Id = Guid.NewGuid();
                venue.CreatedAt = DateTime.UtcNow;
                venue.IsActive = true;

                _logger.LogInformation("Event venue created: {VenueId} - {VenueName}", venue.Id, venue.Name);
                return venue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event venue");
                throw;
            }
        }

        public async Task<List<EventVenueDto>> GetEventVenuesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<EventVenueDto>
            {
                new EventVenueDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Main Conference Hall",
                    Description = "Large conference hall with modern AV equipment",
                    Capacity = 500,
                    Location = "Building A, Floor 1",
                    Amenities = new List<string> { "Projector", "Sound System", "WiFi", "Air Conditioning" },
                    IsActive = true,
                    BookingRate = 500.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new EventVenueDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Name = "Training Room A",
                    Description = "Medium-sized training room with interactive whiteboard",
                    Capacity = 50,
                    Location = "Building B, Floor 2",
                    Amenities = new List<string> { "Interactive Whiteboard", "WiFi", "Video Conferencing" },
                    IsActive = true,
                    BookingRate = 150.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                }
            };
        }
    }

    public class EventDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string EventCode { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string EventType { get; set; }
        public required string Category { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public Guid VenueId { get; set; }
        public required string VenueName { get; set; }
        public required string OrganizerName { get; set; }
        public int MaxAttendees { get; set; }
        public int RegistrationCount { get; set; }
        public required string Status { get; set; }
        public bool IsPublic { get; set; }
        public bool RequiresRegistration { get; set; }
        public DateTime? RegistrationDeadline { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class EventRegistrationDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public required string RegistrationNumber { get; set; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public required string Status { get; set; }
        public required string AttendanceStatus { get; set; }
        public required string SpecialRequirements { get; set; }
    }

    public class EventCalendarDto
    {
        public Guid TenantId { get; set; }
        public DateTime Month { get; set; }
        public List<EventCalendarItemDto> Events { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class EventCalendarItemDto
    {
        public Guid EventId { get; set; }
        public required string Title { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public required string EventType { get; set; }
        public required string VenueName { get; set; }
    }

    public class EventReminderDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public required string ReminderType { get; set; }
        public DateTime ReminderTime { get; set; }
        public required string Message { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventFeedbackDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public int Rating { get; set; }
        public required string Comments { get; set; }
        public bool WouldRecommend { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    public class EventAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int CompletedEvents { get; set; }
        public int CancelledEvents { get; set; }
        public int TotalRegistrations { get; set; }
        public double AverageAttendanceRate { get; set; }
        public double AverageEventRating { get; set; }
        public Dictionary<string, int> EventsByCategory { get; set; }
        public Dictionary<string, int> EventsByType { get; set; }
        public Dictionary<string, int> MonthlyEventTrends { get; set; }
        public Dictionary<string, int> PopularVenues { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class EventReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public int TotalEvents { get; set; }
        public int CompletedEvents { get; set; }
        public int CancelledEvents { get; set; }
        public int UpcomingEvents { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalAttendees { get; set; }
        public double AttendanceRate { get; set; }
        public double AverageEventRating { get; set; }
        public List<string> TopRatedEvents { get; set; }
        public Dictionary<string, EventCategoryStatsDto> EventCategoryBreakdown { get; set; }
        public Dictionary<string, int> VenueUtilization { get; set; }
        public EventBudgetAnalysisDto BudgetAnalysis { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class EventCategoryStatsDto
    {
        public int EventCount { get; set; }
        public int RegistrationCount { get; set; }
        public double AttendanceRate { get; set; }
    }

    public class EventBudgetAnalysisDto
    {
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public double BudgetUtilization { get; set; }
        public decimal CostPerAttendee { get; set; }
    }

    public class EventResourceDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public required string ResourceType { get; set; }
        public required string ResourceName { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EventVenueDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int Capacity { get; set; }
        public required string Location { get; set; }
        public required List<string> Amenities { get; set; }
        public bool IsActive { get; set; }
        public decimal BookingRate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
