using System.ComponentModel.DataAnnotations;

namespace AttendancePlatform.Application.DTOs
{
    public class BiometricTemplateDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TemplateType { get; set; } = string.Empty;
        public string TemplateData { get; set; } = string.Empty;
        public double QualityScore { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? VerificationDate { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class FaceIdentificationDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public double ConfidenceScore { get; set; }
        public string ImageData { get; set; } = string.Empty;
        public DateTime IdentificationDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class FaceEnrollmentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string TemplateData { get; set; } = string.Empty;
        public double QualityScore { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class EnrollFaceRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public string ImageData { get; set; } = string.Empty;
        
        public string? TemplateType { get; set; }
    }

    public class IdentifyFaceRequest
    {
        [Required]
        public string ImageData { get; set; } = string.Empty;
        
        public double MinConfidenceScore { get; set; } = 0.7;
    }

    public class VerifyFaceRequest
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public string ImageData { get; set; } = string.Empty;
    }

    public class FaceVerificationDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public bool IsMatch { get; set; }
        public double ConfidenceScore { get; set; }
        public DateTime VerificationDate { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
