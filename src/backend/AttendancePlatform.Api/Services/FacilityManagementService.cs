using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IFacilityManagementService
    {
        Task<FacilityDto> CreateFacilityAsync(FacilityDto facility);
        Task<List<FacilityDto>> GetFacilitiesAsync(Guid tenantId);
        Task<FacilityDto> UpdateFacilityAsync(Guid facilityId, FacilityDto facility);
        Task<bool> DeleteFacilityAsync(Guid facilityId);
        Task<FacilityBookingDto> CreateBookingAsync(FacilityBookingDto booking);
        Task<List<FacilityBookingDto>> GetFacilityBookingsAsync(Guid facilityId);
        Task<bool> CancelBookingAsync(Guid bookingId);
        Task<FacilityMaintenanceDto> CreateMaintenanceRequestAsync(FacilityMaintenanceDto maintenance);
        Task<List<FacilityMaintenanceDto>> GetMaintenanceRequestsAsync(Guid facilityId);
        Task<bool> UpdateMaintenanceStatusAsync(Guid maintenanceId, string status);
        Task<FacilityUtilizationDto> GetFacilityUtilizationAsync(Guid facilityId, DateTime fromDate, DateTime toDate);
        Task<List<FacilityDto>> GetAvailableFacilitiesAsync(DateTime startTime, DateTime endTime);
        Task<FacilityReportDto> GenerateFacilityReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<FacilityAnalyticsDto> GetFacilityAnalyticsAsync(Guid tenantId);
        Task<FacilityResourceDto> CreateFacilityResourceAsync(FacilityResourceDto resource);
        Task<List<FacilityResourceDto>> GetFacilityResourcesAsync(Guid facilityId);
        Task<FacilityAccessDto> CreateAccessRequestAsync(FacilityAccessDto access);
        Task<List<FacilityAccessDto>> GetAccessRequestsAsync(Guid facilityId);
        Task<bool> GrantFacilityAccessAsync(Guid accessId, Guid approverId);
        Task<FacilitySecurityDto> GetFacilitySecurityStatusAsync(Guid facilityId);
    }

    public class FacilityManagementService : IFacilityManagementService
    {
        private readonly ILogger<FacilityManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public FacilityManagementService(ILogger<FacilityManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<FacilityDto> CreateFacilityAsync(FacilityDto facility)
        {
            try
            {
                facility.Id = Guid.NewGuid();
                facility.FacilityCode = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                facility.CreatedAt = DateTime.UtcNow;
                facility.Status = "Active";
                facility.IsAvailable = true;

                _logger.LogInformation("Facility created: {FacilityId} - {FacilityCode}", facility.Id, facility.FacilityCode);
                return facility;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create facility");
                throw;
            }
        }

        public async Task<List<FacilityDto>> GetFacilitiesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<FacilityDto>
            {
                new FacilityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    FacilityCode = "FAC-20241227-1001",
                    Name = "Main Conference Center",
                    Description = "Large conference center with multiple meeting rooms and auditorium",
                    FacilityType = "Conference Center",
                    Location = "Building A, Floor 1",
                    Capacity = 500,
                    Area = 2500.0,
                    Status = "Active",
                    IsAvailable = true,
                    Amenities = new List<string> { "WiFi", "Projector", "Sound System", "Air Conditioning", "Parking" },
                    OperatingHours = "08:00 - 22:00",
                    ContactPerson = "Facility Manager",
                    ContactPhone = "+1234567890",
                    BookingRate = 250.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new FacilityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    FacilityCode = "FAC-20241227-1002",
                    Name = "Executive Training Room",
                    Description = "Premium training room with advanced AV equipment",
                    FacilityType = "Training Room",
                    Location = "Building B, Floor 3",
                    Capacity = 25,
                    Area = 400.0,
                    Status = "Active",
                    IsAvailable = true,
                    Amenities = new List<string> { "Interactive Whiteboard", "Video Conferencing", "WiFi", "Coffee Station" },
                    OperatingHours = "07:00 - 20:00",
                    ContactPerson = "Training Coordinator",
                    ContactPhone = "+1234567891",
                    BookingRate = 150.00m,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                }
            };
        }

        public async Task<FacilityDto> UpdateFacilityAsync(Guid facilityId, FacilityDto facility)
        {
            try
            {
                await Task.CompletedTask;
                facility.Id = facilityId;
                facility.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Facility updated: {FacilityId}", facilityId);
                return facility;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update facility {FacilityId}", facilityId);
                throw;
            }
        }

        public async Task<bool> DeleteFacilityAsync(Guid facilityId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Facility deleted: {FacilityId}", facilityId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete facility {FacilityId}", facilityId);
                return false;
            }
        }

        public async Task<FacilityBookingDto> CreateBookingAsync(FacilityBookingDto booking)
        {
            try
            {
                booking.Id = Guid.NewGuid();
                booking.BookingNumber = $"BK-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                booking.BookingDate = DateTime.UtcNow;
                booking.Status = "Confirmed";

                _logger.LogInformation("Facility booking created: {BookingId} - {BookingNumber}", booking.Id, booking.BookingNumber);
                return booking;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create facility booking");
                throw;
            }
        }

        public async Task<List<FacilityBookingDto>> GetFacilityBookingsAsync(Guid facilityId)
        {
            await Task.CompletedTask;
            return new List<FacilityBookingDto>
            {
                new FacilityBookingDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    BookingNumber = "BK-20241227-1001",
                    BookedBy = Guid.NewGuid(),
                    BookerName = "John Smith",
                    BookerEmail = "john.smith@company.com",
                    Purpose = "Quarterly Team Meeting",
                    StartDateTime = DateTime.UtcNow.AddDays(2).AddHours(9),
                    EndDateTime = DateTime.UtcNow.AddDays(2).AddHours(17),
                    AttendeeCount = 25,
                    Status = "Confirmed",
                    BookingDate = DateTime.UtcNow.AddDays(-3),
                    TotalCost = 1250.00m,
                    SpecialRequirements = "Catering for 25 people, projector setup"
                },
                new FacilityBookingDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    BookingNumber = "BK-20241227-1002",
                    BookedBy = Guid.NewGuid(),
                    BookerName = "Sarah Johnson",
                    BookerEmail = "sarah.johnson@company.com",
                    Purpose = "Client Presentation",
                    StartDateTime = DateTime.UtcNow.AddDays(5).AddHours(14),
                    EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(16),
                    AttendeeCount = 8,
                    Status = "Pending",
                    BookingDate = DateTime.UtcNow.AddDays(-1),
                    TotalCost = 300.00m,
                    SpecialRequirements = "Video conferencing setup"
                }
            };
        }

        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Facility booking cancelled: {BookingId}", bookingId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel facility booking {BookingId}", bookingId);
                return false;
            }
        }

        public async Task<FacilityMaintenanceDto> CreateMaintenanceRequestAsync(FacilityMaintenanceDto maintenance)
        {
            try
            {
                maintenance.Id = Guid.NewGuid();
                maintenance.RequestNumber = $"MR-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                maintenance.RequestDate = DateTime.UtcNow;
                maintenance.Status = "Open";

                _logger.LogInformation("Facility maintenance request created: {MaintenanceId} - {RequestNumber}", maintenance.Id, maintenance.RequestNumber);
                return maintenance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create facility maintenance request");
                throw;
            }
        }

        public async Task<List<FacilityMaintenanceDto>> GetMaintenanceRequestsAsync(Guid facilityId)
        {
            await Task.CompletedTask;
            return new List<FacilityMaintenanceDto>
            {
                new FacilityMaintenanceDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    RequestNumber = "MR-20241227-1001",
                    RequestType = "Repair",
                    Priority = "High",
                    Description = "Air conditioning unit not working properly in main conference room",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Facility Manager",
                    Status = "In Progress",
                    RequestDate = DateTime.UtcNow.AddDays(-2),
                    ScheduledDate = DateTime.UtcNow.AddDays(1),
                    EstimatedCost = 500.00m,
                    AssignedTechnician = "HVAC Specialist"
                },
                new FacilityMaintenanceDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    RequestNumber = "MR-20241227-1002",
                    RequestType = "Preventive",
                    Priority = "Medium",
                    Description = "Monthly cleaning and inspection of projector equipment",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "IT Support",
                    Status = "Scheduled",
                    RequestDate = DateTime.UtcNow.AddDays(-5),
                    ScheduledDate = DateTime.UtcNow.AddDays(3),
                    EstimatedCost = 150.00m,
                    AssignedTechnician = "AV Technician"
                }
            };
        }

        public async Task<bool> UpdateMaintenanceStatusAsync(Guid maintenanceId, string status)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Facility maintenance status updated: {MaintenanceId} - {Status}", maintenanceId, status);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update maintenance status for {MaintenanceId}", maintenanceId);
                return false;
            }
        }

        public async Task<FacilityUtilizationDto> GetFacilityUtilizationAsync(Guid facilityId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new FacilityUtilizationDto
            {
                FacilityId = facilityId,
                Period = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalAvailableHours = 480,
                TotalBookedHours = 285,
                UtilizationRate = 59.4,
                TotalBookings = 18,
                AverageBookingDuration = 15.8,
                PeakUsageHours = new Dictionary<string, double>
                {
                    { "09:00-10:00", 85.7 },
                    { "10:00-11:00", 92.3 },
                    { "14:00-15:00", 78.9 },
                    { "15:00-16:00", 81.2 }
                },
                BookingsByPurpose = new Dictionary<string, int>
                {
                    { "Team Meetings", 8 },
                    { "Training Sessions", 5 },
                    { "Client Presentations", 3 },
                    { "Interviews", 2 }
                },
                Revenue = 4275.00m,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<FacilityDto>> GetAvailableFacilitiesAsync(DateTime startTime, DateTime endTime)
        {
            await Task.CompletedTask;
            return new List<FacilityDto>
            {
                new FacilityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = Guid.NewGuid(),
                    Name = "Small Meeting Room A",
                    FacilityCode = "MRA-001",
                    Description = "Small meeting room with modern amenities for team meetings",
                    FacilityType = "Meeting Room",
                    Capacity = 8,
                    Area = 25.5,
                    Location = "Building A, Floor 2",
                    Status = "Available",
                    IsAvailable = true,
                    Amenities = new List<string> { "Projector", "Whiteboard", "Video Conference" },
                    OperatingHours = "8:00 AM - 6:00 PM",
                    ContactPerson = "Facility Manager",
                    ContactPhone = "+1-555-0123",
                    BookingRate = 75.00m,
                    CreatedAt = DateTime.UtcNow
                },
                new FacilityDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = Guid.NewGuid(),
                    Name = "Training Room C",
                    FacilityCode = "TRC-001",
                    Description = "Large training room with modern presentation equipment",
                    FacilityType = "Training Room",
                    Capacity = 20,
                    Area = 45.0,
                    Location = "Building B, Floor 1",
                    Status = "Available",
                    IsAvailable = true,
                    Amenities = new List<string> { "Projector", "WiFi", "Flipchart", "Coffee Station" },
                    OperatingHours = "8:00 AM - 8:00 PM",
                    ContactPerson = "Training Coordinator",
                    ContactPhone = "+1-555-0456",
                    BookingRate = 125.00m,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public async Task<FacilityReportDto> GenerateFacilityReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new FacilityReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalFacilities = 25,
                ActiveFacilities = 23,
                InactiveFacilities = 2,
                TotalBookings = 185,
                TotalRevenue = 28750.00m,
                AverageUtilizationRate = 67.8,
                TopPerformingFacilities = new List<string>
                {
                    "Main Conference Center",
                    "Executive Training Room",
                    "Board Room",
                    "Innovation Lab"
                },
                FacilityTypeBreakdown = new Dictionary<string, FacilityTypeStatsDto>
                {
                    { "Conference Rooms", new FacilityTypeStatsDto { Count = 8, Bookings = 85, Revenue = 12750.00m, UtilizationRate = 72.5, Status = "Active", Description = "Conference room facilities" } },
                    { "Training Rooms", new FacilityTypeStatsDto { Count = 6, Bookings = 65, Revenue = 9875.00m, UtilizationRate = 68.3, Status = "Active", Description = "Training room facilities" } },
                    { "Meeting Rooms", new FacilityTypeStatsDto { Count = 10, Bookings = 35, Revenue = 6125.00m, UtilizationRate = 45.2, Status = "Active", Description = "Meeting room facilities" } },
                    { "Other", new FacilityTypeStatsDto { Count = 1, Bookings = 0, Revenue = 0.00m, UtilizationRate = 0.0, Status = "Inactive", Description = "Other facility types" } }
                },
                MaintenanceRequests = 15,
                CompletedMaintenance = 12,
                PendingMaintenance = 3,
                MaintenanceCosts = 4250.00m,
                CustomerSatisfactionScore = 4.3,
                Recommendations = new List<string>
                {
                    "Consider adding more small meeting rooms due to high demand",
                    "Upgrade AV equipment in older training rooms",
                    "Implement automated booking system for better efficiency",
                    "Schedule preventive maintenance during low-usage periods"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<FacilityAnalyticsDto> GetFacilityAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new FacilityAnalyticsDto
            {
                TenantId = tenantId,
                TotalFacilities = 25,
                TotalCapacity = 1250,
                AverageUtilizationRate = 67.8,
                TotalRevenue = 125000.00m,
                BookingTrends = new Dictionary<string, int>
                {
                    { "Jan", 145 }, { "Feb", 158 }, { "Mar", 172 }, { "Apr", 165 },
                    { "May", 189 }, { "Jun", 195 }, { "Jul", 142 }, { "Aug", 178 },
                    { "Sep", 205 }, { "Oct", 198 }, { "Nov", 185 }, { "Dec", 168 }
                },
                UtilizationByFacilityType = new Dictionary<string, double>
                {
                    { "Conference Rooms", 72.5 },
                    { "Training Rooms", 68.3 },
                    { "Meeting Rooms", 45.2 },
                    { "Auditoriums", 85.7 },
                    { "Labs", 52.8 }
                },
                PeakUsageTimes = new Dictionary<string, double>
                {
                    { "08:00-09:00", 45.2 },
                    { "09:00-10:00", 78.5 },
                    { "10:00-11:00", 85.7 },
                    { "11:00-12:00", 72.3 },
                    { "14:00-15:00", 68.9 },
                    { "15:00-16:00", 75.4 },
                    { "16:00-17:00", 52.1 }
                },
                MaintenanceMetrics = new Dictionary<string, int>
                {
                    { "Completed Requests", 125 },
                    { "Pending Requests", 18 },
                    { "Overdue Requests", 3 },
                    { "Preventive Maintenance", 85 }
                },
                CustomerSatisfactionTrends = new Dictionary<string, double>
                {
                    { "Q1", 4.2 }, { "Q2", 4.3 }, { "Q3", 4.1 }, { "Q4", 4.4 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<FacilityResourceDto> CreateFacilityResourceAsync(FacilityResourceDto resource)
        {
            try
            {
                resource.Id = Guid.NewGuid();
                resource.CreatedAt = DateTime.UtcNow;
                resource.Status = "Available";

                _logger.LogInformation("Facility resource created: {ResourceId} for facility {FacilityId}", resource.Id, resource.FacilityId);
                return resource;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create facility resource");
                throw;
            }
        }

        public async Task<List<FacilityResourceDto>> GetFacilityResourcesAsync(Guid facilityId)
        {
            await Task.CompletedTask;
            return new List<FacilityResourceDto>
            {
                new FacilityResourceDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    ResourceType = "AV Equipment",
                    ResourceName = "4K Projector",
                    Description = "High-definition projector with wireless connectivity",
                    Quantity = 2,
                    Status = "Available",
                    Location = "Storage Room A",
                    SerialNumber = "PROJ-2024-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-365),
                    WarrantyExpiry = DateTime.UtcNow.AddDays(365),
                    CreatedAt = DateTime.UtcNow.AddDays(-365)
                },
                new FacilityResourceDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    ResourceType = "Furniture",
                    ResourceName = "Conference Table",
                    Description = "Large oval conference table for 12 people",
                    Quantity = 1,
                    Status = "In Use",
                    Location = "Conference Room",
                    SerialNumber = "TABLE-2024-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-180),
                    WarrantyExpiry = DateTime.UtcNow.AddDays(1820),
                    CreatedAt = DateTime.UtcNow.AddDays(-180)
                }
            };
        }

        public async Task<FacilityAccessDto> CreateAccessRequestAsync(FacilityAccessDto access)
        {
            try
            {
                access.Id = Guid.NewGuid();
                access.RequestNumber = $"AR-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                access.RequestDate = DateTime.UtcNow;
                access.Status = "Pending";

                _logger.LogInformation("Facility access request created: {AccessId} - {RequestNumber}", access.Id, access.RequestNumber);
                return access;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create facility access request");
                throw;
            }
        }

        public async Task<List<FacilityAccessDto>> GetAccessRequestsAsync(Guid facilityId)
        {
            await Task.CompletedTask;
            return new List<FacilityAccessDto>
            {
                new FacilityAccessDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    RequestNumber = "AR-20241227-1001",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "John Smith",
                    RequesterEmail = "john.smith@company.com",
                    AccessType = "Temporary",
                    Purpose = "Client meeting preparation",
                    RequestedStartDate = DateTime.UtcNow.AddDays(1),
                    RequestedEndDate = DateTime.UtcNow.AddDays(1).AddHours(4),
                    Status = "Pending",
                    RequestDate = DateTime.UtcNow.AddHours(-2),
                    Justification = "Need access to set up presentation materials before client arrival"
                },
                new FacilityAccessDto
                {
                    Id = Guid.NewGuid(),
                    FacilityId = facilityId,
                    RequestNumber = "AR-20241227-1002",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Sarah Johnson",
                    RequesterEmail = "sarah.johnson@company.com",
                    AccessType = "Extended",
                    Purpose = "Training program coordination",
                    RequestedStartDate = DateTime.UtcNow.AddDays(7),
                    RequestedEndDate = DateTime.UtcNow.AddDays(14),
                    Status = "Approved",
                    RequestDate = DateTime.UtcNow.AddDays(-3),
                    ApprovedBy = Guid.NewGuid(),
                    ApprovalDate = DateTime.UtcNow.AddDays(-1),
                    Justification = "Coordinating week-long leadership training program"
                }
            };
        }

        public async Task<bool> GrantFacilityAccessAsync(Guid accessId, Guid approverId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Facility access granted: {AccessId} by {ApproverId}", accessId, approverId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to grant facility access for {AccessId}", accessId);
                return false;
            }
        }

        public async Task<FacilitySecurityDto> GetFacilitySecurityStatusAsync(Guid facilityId)
        {
            await Task.CompletedTask;
            return new FacilitySecurityDto
            {
                FacilityId = facilityId,
                SecurityLevel = "High",
                AccessControlStatus = "Active",
                CameraSystemStatus = "Operational",
                AlarmSystemStatus = "Armed",
                LastSecurityCheck = DateTime.UtcNow.AddHours(-2),
                ActiveAccessCards = 25,
                SecurityIncidents = new List<SecurityManagementIncidentDto>
                {
                    new SecurityManagementIncidentDto
                    {
                        Id = Guid.NewGuid(),
                        TenantId = Guid.NewGuid(),
                        IncidentNumber = "INC-20241227-001",
                        Title = "Unauthorized Access Attempt",
                        Description = "Failed card swipe attempt at main entrance",
                        Severity = "Medium",
                        Status = "Resolved",
                        Category = "Access Control",
                        ReportedBy = "Security System",
                        AssignedTo = "Security Team",
                        OccurredAt = DateTime.UtcNow.AddHours(-6),
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                        Resolution = "Investigated and confirmed as employee with expired access card. Card reactivated."
                    }
                },
                SecurityMetrics = new Dictionary<string, object>
                {
                    { "Daily Access Attempts", 145 },
                    { "Successful Access", 142 },
                    { "Failed Access", 3 },
                    { "Security Alerts", 1 }
                },
                LastUpdated = DateTime.UtcNow
            };
        }
    }

    public class FacilityDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string FacilityCode { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string FacilityType { get; set; }
        public required string Location { get; set; }
        public int Capacity { get; set; }
        public double Area { get; set; }
        public required string Status { get; set; }
        public bool IsAvailable { get; set; }
        public required List<string> Amenities { get; set; }
        public required string OperatingHours { get; set; }
        public required string ContactPerson { get; set; }
        public required string ContactPhone { get; set; }
        public decimal BookingRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FacilityBookingDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public required string BookingNumber { get; set; }
        public Guid BookedBy { get; set; }
        public required string BookerName { get; set; }
        public required string BookerEmail { get; set; }
        public required string Purpose { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int AttendeeCount { get; set; }
        public required string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal TotalCost { get; set; }
        public required string SpecialRequirements { get; set; }
    }

    public class FacilityMaintenanceDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public required string RequestNumber { get; set; }
        public required string RequestType { get; set; }
        public required string Priority { get; set; }
        public required string Description { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequesterName { get; set; }
        public required string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public decimal EstimatedCost { get; set; }
        public required string AssignedTechnician { get; set; }
    }

    public class FacilityUtilizationDto
    {
        public Guid FacilityId { get; set; }
        public required string Period { get; set; }
        public int TotalAvailableHours { get; set; }
        public int TotalBookedHours { get; set; }
        public double UtilizationRate { get; set; }
        public int TotalBookings { get; set; }
        public double AverageBookingDuration { get; set; }
        public Dictionary<string, double> PeakUsageHours { get; set; }
        public Dictionary<string, int> BookingsByPurpose { get; set; }
        public decimal Revenue { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class FacilityReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public int TotalFacilities { get; set; }
        public int ActiveFacilities { get; set; }
        public int InactiveFacilities { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageUtilizationRate { get; set; }
        public List<string> TopPerformingFacilities { get; set; }
        public Dictionary<string, FacilityTypeStatsDto> FacilityTypeBreakdown { get; set; }
        public int MaintenanceRequests { get; set; }
        public int CompletedMaintenance { get; set; }
        public int PendingMaintenance { get; set; }
        public decimal MaintenanceCosts { get; set; }
        public double CustomerSatisfactionScore { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class FacilityTypeStatsDto
    {
        public int Count { get; set; }
        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public double UtilizationRate { get; set; }
        public required string Status { get; set; }
        public required string Description { get; set; }
    }

    public class FacilityAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalFacilities { get; set; }
        public int TotalCapacity { get; set; }
        public double AverageUtilizationRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public Dictionary<string, int> BookingTrends { get; set; }
        public Dictionary<string, double> UtilizationByFacilityType { get; set; }
        public Dictionary<string, double> PeakUsageTimes { get; set; }
        public Dictionary<string, int> MaintenanceMetrics { get; set; }
        public Dictionary<string, double> CustomerSatisfactionTrends { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class FacilityResourceDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public required string ResourceType { get; set; }
        public required string ResourceName { get; set; }
        public required string Description { get; set; }
        public int Quantity { get; set; }
        public required string Status { get; set; }
        public required string Location { get; set; }
        public required string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime WarrantyExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FacilityAccessDto
    {
        public Guid Id { get; set; }
        public Guid FacilityId { get; set; }
        public required string RequestNumber { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequesterName { get; set; }
        public required string RequesterEmail { get; set; }
        public required string AccessType { get; set; }
        public required string Purpose { get; set; }
        public DateTime RequestedStartDate { get; set; }
        public DateTime RequestedEndDate { get; set; }
        public required string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public required string Justification { get; set; }
    }

    public class FacilitySecurityDto
    {
        public Guid FacilityId { get; set; }
        public required string SecurityLevel { get; set; }
        public required string AccessControlStatus { get; set; }
        public required string CameraSystemStatus { get; set; }
        public required string AlarmSystemStatus { get; set; }
        public DateTime LastSecurityCheck { get; set; }
        public int ActiveAccessCards { get; set; }
        public List<SecurityManagementIncidentDto> SecurityIncidents { get; set; }
        public Dictionary<string, object> SecurityMetrics { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class FacilitySecurityIncidentDto
    {
        public Guid IncidentId { get; set; }
        public required string IncidentType { get; set; }
        public required string Severity { get; set; }
        public required string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Status { get; set; }
    }
}
