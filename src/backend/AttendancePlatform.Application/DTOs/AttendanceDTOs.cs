using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Application.DTOs
{
    public class CheckInRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Method { get; set; }
        public string? DeviceId { get; set; }
        public string? Notes { get; set; }
        public string? PhotoBase64 { get; set; }
        public Guid? GeofenceId { get; set; }
        public string? BeaconId { get; set; }
        public string? BeaconName { get; set; }
        public string? DeviceType { get; set; }
        public bool IsOffline { get; set; } = false;
        public DateTime? OfflineTimestamp { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? BiometricData { get; set; }
    }

    public class CheckOutRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? Method { get; set; }
        public string? DeviceId { get; set; }
        public string? Notes { get; set; }
        public string? PhotoBase64 { get; set; }
        public Guid? GeofenceId { get; set; }
        public string? BeaconId { get; set; }
        public string? BeaconName { get; set; }
        public string? DeviceType { get; set; }
        public bool IsOffline { get; set; } = false;
        public DateTime? OfflineTimestamp { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? BiometricData { get; set; }
    }

    public class AttendanceRecordDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? CheckInLatitude { get; set; }
        public double? CheckInLongitude { get; set; }
        public double? CheckOutLatitude { get; set; }
        public double? CheckOutLongitude { get; set; }
        public string? Notes { get; set; }
        public Guid? GeofenceId { get; set; }
        public string? BeaconName { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
        public TimeSpan? WorkDuration { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public DateTime Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public bool IsWithinGeofence { get; set; }
        public string? BeaconId { get; set; }
        public bool IsBiometricVerified { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsOfflineRecord { get; set; }
        public bool IsApproved { get; set; } = true;
    }

    public class AttendanceReportRequest
    {
        public Guid? UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Department { get; set; }
        public string? Status { get; set; }
    }

    public class AttendanceReportDto
    {
        public List<AttendanceRecordDto> Records { get; set; } = new();
        public AttendanceStatisticsDto Statistics { get; set; } = new();
    }

    public class AttendanceStatisticsDto
    {
        public int TotalDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int EarlyLeaveDays { get; set; }
        public TimeSpan TotalWorkTime { get; set; }
        public TimeSpan AverageWorkTime { get; set; }
    }
}
