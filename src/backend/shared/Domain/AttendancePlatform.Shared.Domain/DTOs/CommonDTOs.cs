using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Shared.Domain.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public static ApiResponse<T> SuccessResult(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
        
        public static ApiResponse<T> ErrorResult(string message, IEnumerable<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
    
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
    
    public class PagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
    
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        public bool RememberMe { get; set; } = false;
        
        public string? TwoFactorCode { get; set; }
    }
    
    public class LoginResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = null!;
        public bool RequiresTwoFactor { get; set; } = false;
    }
    
    public class UserDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? EmployeeId { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string FullName => $"{FirstName} {LastName}";
    }
    
    public class AttendanceRecordDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public bool IsWithinGeofence { get; set; }
        public string? BeaconId { get; set; }
        public bool IsBiometricVerified { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsOfflineRecord { get; set; }
        public bool IsApproved { get; set; }
        public string? Notes { get; set; }
    }
    
    public class CheckInRequest
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? LocationName { get; set; }
        public string? Address { get; set; }
        public string? BeaconId { get; set; }
        public string? BiometricData { get; set; }
        public string? PhotoBase64 { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
        public string? Notes { get; set; }
        public bool IsOffline { get; set; } = false;
        public DateTime? OfflineTimestamp { get; set; }
    }
    
    public class LeaveRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string LeaveTypeName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalDays { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovalNotes { get; set; }
        public bool IsEmergency { get; set; }
        public IEnumerable<string>? AttachmentUrls { get; set; }
    }
    
    public class CreateLeaveRequestDto
    {
        [Required]
        public Guid LeaveTypeId { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [MaxLength(1000)]
        public string? Reason { get; set; }
        
        public bool IsEmergency { get; set; } = false;
        
        public string? ContactDuringLeave { get; set; }
        
        public IEnumerable<string>? AttachmentUrls { get; set; }
    }
    
    public class PermissionRequestDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestedAt { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public bool IsEmergency { get; set; }
    }
    
    public class CreatePermissionRequestDto
    {
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;
        
        public bool IsEmergency { get; set; } = false;
    }

    public class BiometricTemplateDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public double Quality { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public string? DeviceId { get; set; }
        public string? DeviceType { get; set; }
    }

    public class FaceEnrollmentDto
    {
        public Guid TemplateId { get; set; }
        public double Quality { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class FaceVerificationDto
    {
        public bool IsVerified { get; set; }
        public double Confidence { get; set; }
        public Guid TemplateId { get; set; }
        public DateTime VerificationTime { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class FaceIdentificationDto
    {
        public bool IsIdentified { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public double Confidence { get; set; }
        public Guid? TemplateId { get; set; }
        public DateTime IdentificationTime { get; set; }
        public List<object> AllMatches { get; set; } = new();
    }
}

