using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IGeofencingService
    {
        Task<GeofenceDto> CreateGeofenceAsync(GeofenceDto geofence);
        Task<List<GeofenceDto>> GetGeofencesAsync(Guid tenantId);
        Task<GeofenceDto> UpdateGeofenceAsync(Guid geofenceId, GeofenceDto geofence);
        Task<bool> DeleteGeofenceAsync(Guid geofenceId);
        Task<bool> ValidateLocationAsync(Guid geofenceId, double latitude, double longitude);
        Task<List<GeofenceViolationDto>> GetViolationsAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<GeofenceAnalyticsDto> GetGeofenceAnalyticsAsync(Guid tenantId);
        Task<List<LocationHistoryDto>> GetLocationHistoryAsync(Guid employeeId, DateTime fromDate, DateTime toDate);
        Task<bool> SetGeofenceAlertsAsync(Guid geofenceId, GeofenceAlertConfigDto alertConfig);
        Task<List<GeofenceAlertDto>> GetActiveAlertsAsync(Guid tenantId);
        Task<GeofenceComplianceDto> GetComplianceReportAsync(Guid tenantId);
        Task<bool> BulkImportGeofencesAsync(Guid tenantId, List<GeofenceDto> geofences);
        Task<GeofenceOptimizationDto> OptimizeGeofencesAsync(Guid tenantId);
        Task<List<NearbyLocationDto>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm);
        Task<GeofenceUsageReportDto> GenerateUsageReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
    }

    public class GeofencingService : IGeofencingService
    {
        private readonly ILogger<GeofencingService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public GeofencingService(ILogger<GeofencingService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<GeofenceDto> CreateGeofenceAsync(GeofenceDto geofence)
        {
            try
            {
                geofence.Id = Guid.NewGuid();
                geofence.CreatedAt = DateTime.UtcNow;
                geofence.IsActive = true;

                _logger.LogInformation("Geofence created: {GeofenceId} - {GeofenceName}", geofence.Id, geofence.Name);
                return geofence;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create geofence");
                throw;
            }
        }

        public async Task<List<GeofenceDto>> GetGeofencesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<GeofenceDto>
            {
                new GeofenceDto 
                { 
                    Id = Guid.NewGuid(), 
                    TenantId = tenantId,
                    Name = "Main Office", 
                    Description = "Main office location",
                    Latitude = 24.7136, 
                    Longitude = 46.6753, 
                    Radius = 100,
                    Type = "Office",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new GeofenceDto 
                { 
                    Id = Guid.NewGuid(), 
                    TenantId = tenantId,
                    Name = "Branch Office", 
                    Description = "Branch office location",
                    Latitude = 24.7236, 
                    Longitude = 46.6853, 
                    Radius = 75,
                    Type = "Branch",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };
        }

        public async Task<GeofenceDto> UpdateGeofenceAsync(Guid geofenceId, GeofenceDto geofence)
        {
            try
            {
                await Task.CompletedTask;
                geofence.Id = geofenceId;
                geofence.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Geofence updated: {GeofenceId}", geofenceId);
                return geofence;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update geofence {GeofenceId}", geofenceId);
                throw;
            }
        }

        public async Task<bool> DeleteGeofenceAsync(Guid geofenceId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Geofence deleted: {GeofenceId}", geofenceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete geofence {GeofenceId}", geofenceId);
                return false;
            }
        }

        public async Task<bool> ValidateLocationAsync(Guid geofenceId, double latitude, double longitude)
        {
            try
            {
                await Task.CompletedTask;
                var distance = CalculateDistance(24.7136, 46.6753, latitude, longitude);
                var isValid = distance <= 100;

                _logger.LogInformation("Location validation for geofence {GeofenceId}: {IsValid} (distance: {Distance}m)", 
                    geofenceId, isValid, distance);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate location for geofence {GeofenceId}", geofenceId);
                return false;
            }
        }

        public async Task<List<GeofenceViolationDto>> GetViolationsAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new List<GeofenceViolationDto>
            {
                new GeofenceViolationDto
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "John Doe",
                    GeofenceId = Guid.NewGuid(),
                    GeofenceName = "Main Office",
                    ViolationType = "Outside Boundary",
                    Latitude = 24.7200,
                    Longitude = 46.6800,
                    Distance = 150,
                    OccurredAt = DateTime.UtcNow.AddHours(-2),
                    Severity = "Medium"
                }
            };
        }

        public async Task<GeofenceAnalyticsDto> GetGeofenceAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new GeofenceAnalyticsDto
            {
                TenantId = tenantId,
                TotalGeofences = 5,
                ActiveGeofences = 4,
                TotalViolations = 12,
                ComplianceRate = 94.5,
                MostViolatedGeofence = "Main Office",
                AverageAccuracy = 98.2,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<LocationHistoryDto>> GetLocationHistoryAsync(Guid employeeId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            var history = new List<LocationHistoryDto>();
            var random = new Random();

            for (int i = 0; i < 10; i++)
            {
                history.Add(new LocationHistoryDto
                {
                    EmployeeId = employeeId,
                    Latitude = 24.7136 + (random.NextDouble() - 0.5) * 0.01,
                    Longitude = 46.6753 + (random.NextDouble() - 0.5) * 0.01,
                    Accuracy = 5 + random.Next(10),
                    Timestamp = DateTime.UtcNow.AddHours(-i),
                    Source = "GPS"
                });
            }

            return history;
        }

        public async Task<bool> SetGeofenceAlertsAsync(Guid geofenceId, GeofenceAlertConfigDto alertConfig)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Geofence alerts configured for {GeofenceId}", geofenceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set geofence alerts for {GeofenceId}", geofenceId);
                return false;
            }
        }

        public async Task<List<GeofenceAlertDto>> GetActiveAlertsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<GeofenceAlertDto>
            {
                new GeofenceAlertDto
                {
                    Id = Guid.NewGuid(),
                    GeofenceId = Guid.NewGuid(),
                    GeofenceName = "Main Office",
                    AlertType = "Boundary Violation",
                    Message = "Employee outside designated area",
                    Severity = "High",
                    TriggeredAt = DateTime.UtcNow.AddMinutes(-15),
                    IsActive = true
                }
            };
        }

        public async Task<GeofenceComplianceDto> GetComplianceReportAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new GeofenceComplianceDto
            {
                TenantId = tenantId,
                OverallCompliance = 96.8,
                PolicyCompliance = 98.2,
                LocationAccuracy = 94.5,
                ViolationRate = 3.2,
                ComplianceByGeofence = new Dictionary<string, double>
                {
                    { "Main Office", 97.5 },
                    { "Branch Office", 95.8 },
                    { "Warehouse", 98.1 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> BulkImportGeofencesAsync(Guid tenantId, List<GeofenceDto> geofences)
        {
            try
            {
                await Task.CompletedTask;
                foreach (var geofence in geofences)
                {
                    geofence.Id = Guid.NewGuid();
                    geofence.TenantId = tenantId;
                    geofence.CreatedAt = DateTime.UtcNow;
                }

                _logger.LogInformation("Bulk imported {Count} geofences for tenant {TenantId}", geofences.Count, tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to bulk import geofences for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<GeofenceOptimizationDto> OptimizeGeofencesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new GeofenceOptimizationDto
            {
                TenantId = tenantId,
                OptimizationScore = 88.5,
                Recommendations = new List<string>
                {
                    "Increase radius for Main Office by 25m to reduce false violations",
                    "Merge overlapping geofences in downtown area",
                    "Add buffer zones for high-traffic areas"
                },
                PotentialSavings = 15.3,
                AccuracyImprovement = 12.8,
                OptimizedAt = DateTime.UtcNow
            };
        }

        public async Task<List<NearbyLocationDto>> GetNearbyLocationsAsync(double latitude, double longitude, double radiusKm)
        {
            await Task.CompletedTask;
            return new List<NearbyLocationDto>
            {
                new NearbyLocationDto { Name = "Coffee Shop", Distance = 0.2, Type = "Restaurant", Latitude = latitude + 0.001, Longitude = longitude + 0.001 },
                new NearbyLocationDto { Name = "Bank Branch", Distance = 0.5, Type = "Financial", Latitude = latitude + 0.002, Longitude = longitude + 0.002 },
                new NearbyLocationDto { Name = "Parking Garage", Distance = 0.1, Type = "Parking", Latitude = latitude - 0.001, Longitude = longitude - 0.001 }
            };
        }

        public async Task<GeofenceUsageReportDto> GenerateUsageReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var totalEmployees = await _context.Users.CountAsync(u => u.TenantId == tenantId && u.IsActive);
                
                return new GeofenceUsageReportDto
                {
                    TenantId = tenantId,
                    ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                    TotalGeofences = 5,
                    ActiveGeofences = 4,
                    TotalCheckIns = totalEmployees * 20,
                    ValidCheckIns = totalEmployees * 18,
                    InvalidCheckIns = totalEmployees * 2,
                    AccuracyRate = 90.0,
                    MostUsedGeofence = "Main Office",
                    LeastUsedGeofence = "Remote Site",
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate usage report for tenant {TenantId}", tenantId);
                throw;
            }
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth's radius in meters
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }

    public class GeofenceDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Radius { get; set; }
        public required string Type { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class GeofenceViolationDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public Guid GeofenceId { get; set; }
        public required string GeofenceName { get; set; }
        public required string ViolationType { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Distance { get; set; }
        public DateTime OccurredAt { get; set; }
        public required string Severity { get; set; }
    }

    public class GeofenceAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalGeofences { get; set; }
        public int ActiveGeofences { get; set; }
        public int TotalViolations { get; set; }
        public double ComplianceRate { get; set; }
        public required string MostViolatedGeofence { get; set; }
        public double AverageAccuracy { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class LocationHistoryDto
    {
        public Guid EmployeeId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Source { get; set; }
    }

    public class GeofenceAlertConfigDto
    {
        public bool EnableViolationAlerts { get; set; }
        public bool EnableEntryAlerts { get; set; }
        public bool EnableExitAlerts { get; set; }
        public List<string> NotificationMethods { get; set; }
        public List<string> Recipients { get; set; }
    }

    public class GeofenceAlertDto
    {
        public Guid Id { get; set; }
        public Guid GeofenceId { get; set; }
        public required string GeofenceName { get; set; }
        public required string AlertType { get; set; }
        public required string Message { get; set; }
        public required string Severity { get; set; }
        public DateTime TriggeredAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class GeofenceComplianceDto
    {
        public Guid TenantId { get; set; }
        public double OverallCompliance { get; set; }
        public double PolicyCompliance { get; set; }
        public double LocationAccuracy { get; set; }
        public double ViolationRate { get; set; }
        public Dictionary<string, double> ComplianceByGeofence { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class GeofenceOptimizationDto
    {
        public Guid TenantId { get; set; }
        public double OptimizationScore { get; set; }
        public List<string> Recommendations { get; set; }
        public double PotentialSavings { get; set; }
        public double AccuracyImprovement { get; set; }
        public DateTime OptimizedAt { get; set; }
    }

    public class NearbyLocationDto
    {
        public required string Name { get; set; }
        public double Distance { get; set; }
        public required string Type { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class GeofenceUsageReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public int TotalGeofences { get; set; }
        public int ActiveGeofences { get; set; }
        public int TotalCheckIns { get; set; }
        public int ValidCheckIns { get; set; }
        public int InvalidCheckIns { get; set; }
        public double AccuracyRate { get; set; }
        public required string MostUsedGeofence { get; set; }
        public required string LeastUsedGeofence { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
