using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Application.Services;

namespace AttendancePlatform.Api.Services
{
    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<FaceRecognitionService> _logger;
        private readonly ITenantContext _tenantContext;

        public FaceRecognitionService(
            AttendancePlatformDbContext context,
            ILogger<FaceRecognitionService> logger,
            ITenantContext tenantContext)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public async Task<ApiResponse<Application.DTOs.FaceEnrollmentDto>> EnrollFaceAsync(Guid userId, byte[] imageData, string deviceId = "", string deviceType = "")
        {
            try
            {
                var template = ExtractFaceTemplate(imageData);
                if (template == null)
                {
                    return ApiResponse<Application.DTOs.FaceEnrollmentDto>.ErrorResult("No face detected in the image");
                }

                var biometricTemplate = new BiometricTemplate
                {
                    UserId = userId.ToString(),
                    TemplateData = template,
                    BiometricType = "Face",
                    QualityScore = CalculateQualityScore(imageData),
                    EnrolledAt = DateTime.UtcNow,
                    DeviceId = deviceId,
                    IsActive = true,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.BiometricTemplates.Add(biometricTemplate);
                await _context.SaveChangesAsync();

                var result = new Application.DTOs.FaceEnrollmentDto
                {
                    Id = biometricTemplate.Id,
                    UserId = userId,
                    QualityScore = biometricTemplate.QualityScore,
                    EnrollmentDate = biometricTemplate.EnrolledAt,
                    IsSuccessful = true
                };

                return ApiResponse<Application.DTOs.FaceEnrollmentDto>.SuccessResult(result, "Face enrolled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling face for user: {UserId}", userId);
                return ApiResponse<Application.DTOs.FaceEnrollmentDto>.ErrorResult("An error occurred during face enrollment");
            }
        }

        public async Task<ApiResponse<Application.DTOs.FaceVerificationDto>> VerifyFaceAsync(Guid userId, byte[] imageData, string deviceId = "", string deviceType = "")
        {
            try
            {
                var template = ExtractFaceTemplate(imageData);
                if (template == null)
                {
                    return ApiResponse<Application.DTOs.FaceVerificationDto>.ErrorResult("No face detected in the image");
                }

                var userTemplates = await _context.BiometricTemplates
                    .Where(bt => bt.UserId == userId.ToString() && bt.BiometricType == "Face" && bt.IsActive)
                    .ToListAsync();

                if (!userTemplates.Any())
                {
                    return ApiResponse<Application.DTOs.FaceVerificationDto>.ErrorResult("No enrolled face templates found for user");
                }

                var bestMatch = 0.0;
                BiometricTemplate? matchedTemplate = null;

                foreach (var userTemplate in userTemplates)
                {
                    var storedTemplate = userTemplate.TemplateData;
                    var similarity = CompareFaceTemplates(template, storedTemplate);
                    
                    if (similarity > bestMatch)
                    {
                        bestMatch = similarity;
                        matchedTemplate = userTemplate;
                    }
                }

                var threshold = 0.75;
                var isVerified = bestMatch >= threshold;

                var verificationLog = new BiometricVerificationLog
                {
                    UserId = userId,
                    TemplateId = matchedTemplate?.Id,
                    VerificationTime = DateTime.UtcNow,
                    IsSuccessful = isVerified,
                    ConfidenceScore = bestMatch,
                    DeviceId = deviceId,
                    DeviceType = deviceType,
                    FailureReason = isVerified ? null : "Confidence score below threshold",
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.BiometricVerificationLogs.Add(verificationLog);
                await _context.SaveChangesAsync();

                var result = new Application.DTOs.FaceVerificationDto
                {
                    UserId = userId,
                    IsMatch = isVerified,
                    ConfidenceScore = bestMatch,
                    VerificationDate = verificationLog.VerificationTime
                };

                return ApiResponse<Application.DTOs.FaceVerificationDto>.SuccessResult(result, 
                    isVerified ? "Face verification successful" : "Face verification failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying face for user: {UserId}", userId);
                return ApiResponse<Application.DTOs.FaceVerificationDto>.ErrorResult("An error occurred during face verification");
            }
        }

        public async Task<ApiResponse<IEnumerable<Application.DTOs.BiometricTemplateDto>>> GetUserTemplatesAsync(Guid userId)
        {
            try
            {
                var templates = await _context.BiometricTemplates
                    .Where(bt => bt.UserId == userId.ToString() && bt.BiometricType == "Face" && bt.IsActive)
                    .ToListAsync();

                var dtos = templates.Select(t => new Application.DTOs.BiometricTemplateDto
                {
                    Id = t.Id,
                    UserId = Guid.Parse(t.UserId),
                    TemplateType = t.BiometricType,
                    QualityScore = t.QualityScore,
                    EnrollmentDate = t.EnrolledAt,
                    IsActive = t.IsActive
                }).ToList();

                return ApiResponse<IEnumerable<Application.DTOs.BiometricTemplateDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting templates for user: {UserId}", userId);
                return ApiResponse<IEnumerable<Application.DTOs.BiometricTemplateDto>>.ErrorResult("An error occurred while retrieving templates");
            }
        }

        public async Task<ApiResponse<bool>> DeleteTemplateAsync(Guid userId, Guid templateId)
        {
            try
            {
                var template = await _context.BiometricTemplates
                    .FirstOrDefaultAsync(bt => bt.Id == templateId && bt.UserId == userId.ToString());

                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResult("Template not found");
                }

                template.IsActive = false;
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Template deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template: {TemplateId}", templateId);
                return ApiResponse<bool>.ErrorResult("An error occurred while deleting template");
            }
        }

        public async Task<ApiResponse<Application.DTOs.FaceIdentificationDto>> IdentifyFaceAsync(byte[] imageData, string deviceId = "", string deviceType = "")
        {
            try
            {
                var template = ExtractFaceTemplate(imageData);
                if (template == null)
                {
                    return ApiResponse<Application.DTOs.FaceIdentificationDto>.ErrorResult("No face detected in the image");
                }

                var allTemplates = await _context.BiometricTemplates
                    .Where(bt => bt.BiometricType == "Face" && bt.IsActive)
                    .ToListAsync();

                var bestMatch = 0.0;
                BiometricTemplate? matchedTemplate = null;

                foreach (var userTemplate in allTemplates)
                {
                    var storedTemplate = userTemplate.TemplateData;
                    var similarity = CompareFaceTemplates(template, storedTemplate);
                    
                    if (similarity > bestMatch)
                    {
                        bestMatch = similarity;
                        matchedTemplate = userTemplate;
                    }
                }

                var threshold = 0.80;
                var isIdentified = bestMatch >= threshold;

                var result = new Application.DTOs.FaceIdentificationDto
                {
                    Id = Guid.NewGuid(),
                    UserId = isIdentified && matchedTemplate != null ? Guid.Parse(matchedTemplate.UserId) : null,
                    ConfidenceScore = bestMatch,
                    IdentificationDate = DateTime.UtcNow,
                    IsSuccessful = isIdentified
                };

                return ApiResponse<Application.DTOs.FaceIdentificationDto>.SuccessResult(result,
                    isIdentified ? "Face identified successfully" : "Face not identified");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error identifying face");
                return ApiResponse<Application.DTOs.FaceIdentificationDto>.ErrorResult("An error occurred during face identification");
            }
        }

        public async Task<ApiResponse<bool>> UpdateTemplateAsync(Guid userId, Guid templateId, byte[] newImageData)
        {
            try
            {
                var template = await _context.BiometricTemplates
                    .FirstOrDefaultAsync(bt => bt.Id == templateId && bt.UserId == userId.ToString());

                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResult("Template not found");
                }

                var newTemplate = ExtractFaceTemplate(newImageData);
                if (newTemplate == null)
                {
                    return ApiResponse<bool>.ErrorResult("No face detected in the new image");
                }

                template.TemplateData = newTemplate;
                template.QualityScore = CalculateQualityScore(newImageData);
                template.EnrolledAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Template updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template: {TemplateId}", templateId);
                return ApiResponse<bool>.ErrorResult("An error occurred while updating template");
            }
        }

        private byte[]? ExtractFaceTemplate(byte[] imageData)
        {
            try
            {
                return imageData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting face template");
                return null;
            }
        }

        private double CalculateQualityScore(byte[] imageData)
        {
            try
            {
                return 0.85;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating quality score");
                return 0.0;
            }
        }

        private double CompareFaceTemplates(byte[] template1, byte[] template2)
        {
            try
            {
                if (template1.Length != template2.Length)
                    return 0.0;

                var similarity = 0.0;
                for (int i = 0; i < template1.Length; i++)
                {
                    similarity += Math.Abs(template1[i] - template2[i]);
                }

                return Math.Max(0.0, 1.0 - (similarity / (template1.Length * 255.0)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing face templates");
                return 0.0;
            }
        }
    }
}
