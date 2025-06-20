using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.FaceRecognition.Api.Services
{
    public interface IFaceRecognitionService
    {
        Task<ApiResponse<FaceEnrollmentDto>> EnrollFaceAsync(FaceEnrollmentRequest request, Guid userId);
        Task<ApiResponse<FaceVerificationDto>> VerifyFaceAsync(FaceVerificationRequest request, Guid userId);
        Task<ApiResponse<LivenessDetectionDto>> DetectLivenessAsync(LivenessDetectionRequest request);
        Task<ApiResponse<IEnumerable<BiometricTemplateDto>>> GetUserFaceTemplatesAsync(Guid userId);
        Task<ApiResponse<bool>> DeleteFaceTemplateAsync(Guid templateId, Guid userId);
        Task<ApiResponse<FaceMatchDto>> FindFaceMatchAsync(FaceMatchRequest request);
        Task<ApiResponse<bool>> UpdateFaceTemplateAsync(Guid templateId, UpdateFaceTemplateRequest request, Guid userId);
    }

    public class FaceRecognitionService : IFaceRecognitionService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<FaceRecognitionService> _logger;
        private readonly ITenantContext _tenantContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public FaceRecognitionService(
            AttendancePlatformDbContext context,
            ILogger<FaceRecognitionService> logger,
            ITenantContext tenantContext,
            IDateTimeProvider dateTimeProvider,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
            _dateTimeProvider = dateTimeProvider;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<ApiResponse<FaceEnrollmentDto>> EnrollFaceAsync(FaceEnrollmentRequest request, Guid userId)
        {
            try
            {
                // Validate image data
                if (string.IsNullOrEmpty(request.ImageBase64))
                {
                    return ApiResponse<FaceEnrollmentDto>.ErrorResult("Image data is required");
                }

                // Process and validate image
                var processedImage = await ProcessImageAsync(request.ImageBase64);
                if (processedImage == null)
                {
                    return ApiResponse<FaceEnrollmentDto>.ErrorResult("Invalid image format or corrupted image");
                }

                // Perform liveness detection if required
                if (request.RequireLivenessCheck)
                {
                    var livenessResult = await PerformLivenessDetectionAsync(processedImage.ImageData);
                    if (!livenessResult.IsLive)
                    {
                        return ApiResponse<FaceEnrollmentDto>.ErrorResult("Liveness detection failed. Please ensure you are a real person.");
                    }
                }

                // Extract face features/template
                var faceTemplate = await ExtractFaceTemplateAsync(processedImage.ImageData);
                if (faceTemplate == null)
                {
                    return ApiResponse<FaceEnrollmentDto>.ErrorResult("No face detected in the image or face quality is too low");
                }

                // Check for duplicate enrollment
                var existingTemplates = await _context.BiometricTemplates
                    .Where(bt => bt.UserId == userId && bt.Type == BiometricType.Face && !bt.IsDeleted)
                    .ToListAsync();

                foreach (var template in existingTemplates)
                {
                    var similarity = await CompareFaceTemplatesAsync(faceTemplate.TemplateData, template.TemplateData);
                    if (similarity > 0.85) // High similarity threshold
                    {
                        return ApiResponse<FaceEnrollmentDto>.ErrorResult("This face is already enrolled for the user");
                    }
                }

                // Save face template
                var biometricTemplate = new BiometricTemplate
                {
                    UserId = userId,
                    Type = BiometricType.Face,
                    TemplateData = faceTemplate.TemplateData,
                    Quality = faceTemplate.Quality,
                    ImageUrl = await SaveImageAsync(processedImage.ImageData, userId),
                    EnrollmentDate = _dateTimeProvider.UtcNow,
                    IsActive = true,
                    DeviceId = request.DeviceId,
                    DeviceType = request.DeviceType,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.BiometricTemplates.Add(biometricTemplate);
                await _context.SaveChangesAsync();

                var dto = new FaceEnrollmentDto
                {
                    TemplateId = biometricTemplate.Id,
                    Quality = biometricTemplate.Quality,
                    EnrollmentDate = biometricTemplate.EnrollmentDate,
                    IsActive = biometricTemplate.IsActive,
                    Message = "Face enrolled successfully"
                };

                return ApiResponse<FaceEnrollmentDto>.SuccessResult(dto, "Face enrollment completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during face enrollment for user: {UserId}", userId);
                return ApiResponse<FaceEnrollmentDto>.ErrorResult("An error occurred during face enrollment");
            }
        }

        public async Task<ApiResponse<FaceVerificationDto>> VerifyFaceAsync(FaceVerificationRequest request, Guid userId)
        {
            try
            {
                // Validate image data
                if (string.IsNullOrEmpty(request.ImageBase64))
                {
                    return ApiResponse<FaceVerificationDto>.ErrorResult("Image data is required");
                }

                // Process image
                var processedImage = await ProcessImageAsync(request.ImageBase64);
                if (processedImage == null)
                {
                    return ApiResponse<FaceVerificationDto>.ErrorResult("Invalid image format or corrupted image");
                }

                // Perform liveness detection if required
                if (request.RequireLivenessCheck)
                {
                    var livenessResult = await PerformLivenessDetectionAsync(processedImage.ImageData);
                    if (!livenessResult.IsLive)
                    {
                        return ApiResponse<FaceVerificationDto>.ErrorResult("Liveness detection failed");
                    }
                }

                // Extract face template from verification image
                var verificationTemplate = await ExtractFaceTemplateAsync(processedImage.ImageData);
                if (verificationTemplate == null)
                {
                    return ApiResponse<FaceVerificationDto>.ErrorResult("No face detected in the verification image");
                }

                // Get user's enrolled face templates
                var enrolledTemplates = await _context.BiometricTemplates
                    .Where(bt => bt.UserId == userId && bt.Type == BiometricType.Face && bt.IsActive && !bt.IsDeleted)
                    .ToListAsync();

                if (!enrolledTemplates.Any())
                {
                    return ApiResponse<FaceVerificationDto>.ErrorResult("No enrolled face templates found for user");
                }

                // Compare with enrolled templates
                double bestMatchScore = 0;
                Guid? bestMatchTemplateId = null;

                foreach (var template in enrolledTemplates)
                {
                    var similarity = await CompareFaceTemplatesAsync(verificationTemplate.TemplateData, template.TemplateData);
                    if (similarity > bestMatchScore)
                    {
                        bestMatchScore = similarity;
                        bestMatchTemplateId = template.Id;
                    }
                }

                // Determine verification result
                var threshold = _configuration.GetValue<double>("FaceRecognition:VerificationThreshold", 0.75);
                var isVerified = bestMatchScore >= threshold;

                // Log verification attempt
                var verificationLog = new BiometricVerificationLog
                {
                    UserId = userId,
                    Type = BiometricType.Face,
                    TemplateId = bestMatchTemplateId,
                    VerificationTime = _dateTimeProvider.UtcNow,
                    IsSuccessful = isVerified,
                    ConfidenceScore = bestMatchScore,
                    DeviceId = request.DeviceId,
                    DeviceType = request.DeviceType,
                    TenantId = _tenantContext.TenantId ?? throw new InvalidOperationException("Tenant context not set")
                };

                _context.BiometricVerificationLogs.Add(verificationLog);
                await _context.SaveChangesAsync();

                var dto = new FaceVerificationDto
                {
                    IsVerified = isVerified,
                    ConfidenceScore = bestMatchScore,
                    MatchedTemplateId = bestMatchTemplateId,
                    VerificationTime = verificationLog.VerificationTime,
                    Message = isVerified ? "Face verification successful" : "Face verification failed"
                };

                return ApiResponse<FaceVerificationDto>.SuccessResult(dto, dto.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during face verification for user: {UserId}", userId);
                return ApiResponse<FaceVerificationDto>.ErrorResult("An error occurred during face verification");
            }
        }

        public async Task<ApiResponse<LivenessDetectionDto>> DetectLivenessAsync(LivenessDetectionRequest request)
        {
            try
            {
                // Process image
                var processedImage = await ProcessImageAsync(request.ImageBase64);
                if (processedImage == null)
                {
                    return ApiResponse<LivenessDetectionDto>.ErrorResult("Invalid image format");
                }

                // Perform liveness detection
                var livenessResult = await PerformLivenessDetectionAsync(processedImage.ImageData);

                var dto = new LivenessDetectionDto
                {
                    IsLive = livenessResult.IsLive,
                    ConfidenceScore = livenessResult.Confidence,
                    DetectionTime = _dateTimeProvider.UtcNow,
                    Checks = livenessResult.Checks,
                    Message = livenessResult.IsLive ? "Liveness detected" : "Liveness not detected"
                };

                return ApiResponse<LivenessDetectionDto>.SuccessResult(dto, dto.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during liveness detection");
                return ApiResponse<LivenessDetectionDto>.ErrorResult("An error occurred during liveness detection");
            }
        }

        public async Task<ApiResponse<IEnumerable<BiometricTemplateDto>>> GetUserFaceTemplatesAsync(Guid userId)
        {
            try
            {
                var templates = await _context.BiometricTemplates
                    .Where(bt => bt.UserId == userId && bt.Type == BiometricType.Face && !bt.IsDeleted)
                    .OrderByDescending(bt => bt.EnrollmentDate)
                    .ToListAsync();

                var dtos = templates.Select(t => new BiometricTemplateDto
                {
                    Id = t.Id,
                    Type = t.Type.ToString(),
                    Quality = t.Quality,
                    EnrollmentDate = t.EnrollmentDate,
                    IsActive = t.IsActive,
                    DeviceId = t.DeviceId,
                    DeviceType = t.DeviceType
                });

                return ApiResponse<IEnumerable<BiometricTemplateDto>>.SuccessResult(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting face templates for user: {UserId}", userId);
                return ApiResponse<IEnumerable<BiometricTemplateDto>>.ErrorResult("An error occurred while retrieving face templates");
            }
        }

        public async Task<ApiResponse<bool>> DeleteFaceTemplateAsync(Guid templateId, Guid userId)
        {
            try
            {
                var template = await _context.BiometricTemplates
                    .FirstOrDefaultAsync(bt => bt.Id == templateId && bt.UserId == userId && bt.Type == BiometricType.Face);

                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResult("Face template not found");
                }

                template.IsDeleted = true;
                template.UpdatedAt = _dateTimeProvider.UtcNow;
                
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Face template deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting face template: {TemplateId}", templateId);
                return ApiResponse<bool>.ErrorResult("An error occurred while deleting the face template");
            }
        }

        public async Task<ApiResponse<FaceMatchDto>> FindFaceMatchAsync(FaceMatchRequest request)
        {
            try
            {
                // Process image
                var processedImage = await ProcessImageAsync(request.ImageBase64);
                if (processedImage == null)
                {
                    return ApiResponse<FaceMatchDto>.ErrorResult("Invalid image format");
                }

                // Extract face template
                var searchTemplate = await ExtractFaceTemplateAsync(processedImage.ImageData);
                if (searchTemplate == null)
                {
                    return ApiResponse<FaceMatchDto>.ErrorResult("No face detected in the search image");
                }

                // Get all active face templates in the tenant
                var allTemplates = await _context.BiometricTemplates
                    .Include(bt => bt.User)
                    .Where(bt => bt.Type == BiometricType.Face && bt.IsActive && !bt.IsDeleted)
                    .ToListAsync();

                // Find best matches
                var matches = new List<FaceMatchResult>();
                var threshold = _configuration.GetValue<double>("FaceRecognition:SearchThreshold", 0.70);

                foreach (var template in allTemplates)
                {
                    var similarity = await CompareFaceTemplatesAsync(searchTemplate.TemplateData, template.TemplateData);
                    if (similarity >= threshold)
                    {
                        matches.Add(new FaceMatchResult
                        {
                            UserId = template.UserId,
                            UserName = $"{template.User?.FirstName} {template.User?.LastName}",
                            EmployeeId = template.User?.EmployeeId,
                            TemplateId = template.Id,
                            ConfidenceScore = similarity
                        });
                    }
                }

                // Sort by confidence score
                matches = matches.OrderByDescending(m => m.ConfidenceScore).ToList();

                var dto = new FaceMatchDto
                {
                    HasMatches = matches.Any(),
                    MatchCount = matches.Count,
                    Matches = matches,
                    SearchTime = _dateTimeProvider.UtcNow
                };

                return ApiResponse<FaceMatchDto>.SuccessResult(dto, $"Found {matches.Count} face matches");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during face search");
                return ApiResponse<FaceMatchDto>.ErrorResult("An error occurred during face search");
            }
        }

        public async Task<ApiResponse<bool>> UpdateFaceTemplateAsync(Guid templateId, UpdateFaceTemplateRequest request, Guid userId)
        {
            try
            {
                var template = await _context.BiometricTemplates
                    .FirstOrDefaultAsync(bt => bt.Id == templateId && bt.UserId == userId && bt.Type == BiometricType.Face);

                if (template == null)
                {
                    return ApiResponse<bool>.ErrorResult("Face template not found");
                }

                template.IsActive = request.IsActive;
                template.UpdatedAt = _dateTimeProvider.UtcNow;
                
                await _context.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Face template updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating face template: {TemplateId}", templateId);
                return ApiResponse<bool>.ErrorResult("An error occurred while updating the face template");
            }
        }

        private async Task<ProcessedImageResult?> ProcessImageAsync(string imageBase64)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(imageBase64);
                
                using var image = Image.Load(imageBytes);
                
                // Resize image if too large (max 1024x1024)
                if (image.Width > 1024 || image.Height > 1024)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(1024, 1024),
                        Mode = ResizeMode.Max
                    }));
                }

                // Convert to JPEG format
                using var outputStream = new MemoryStream();
                await image.SaveAsJpegAsync(outputStream, new JpegEncoder { Quality = 85 });
                
                return new ProcessedImageResult
                {
                    ImageData = outputStream.ToArray(),
                    Width = image.Width,
                    Height = image.Height
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image");
                return null;
            }
        }

        private async Task<FaceTemplateResult?> ExtractFaceTemplateAsync(byte[] imageData)
        {
            // This is a placeholder implementation
            // In a real implementation, you would use a face recognition library
            // such as Microsoft Cognitive Services, AWS Rekognition, or an on-device ML model
            
            await Task.Delay(100); // Simulate processing time
            
            // Generate a mock face template (in reality, this would be actual face features)
            var random = new Random();
            var template = new byte[512]; // Typical face template size
            random.NextBytes(template);
            
            return new FaceTemplateResult
            {
                TemplateData = Convert.ToBase64String(template),
                Quality = random.NextDouble() * 0.3 + 0.7 // Quality between 0.7 and 1.0
            };
        }

        private async Task<double> CompareFaceTemplatesAsync(string template1, string template2)
        {
            // This is a placeholder implementation
            // In a real implementation, you would compare actual face templates
            
            await Task.Delay(50); // Simulate processing time
            
            // Mock comparison - in reality, this would use cosine similarity or other metrics
            var random = new Random();
            return random.NextDouble(); // Return random similarity score
        }

        private async Task<LivenessResult> PerformLivenessDetectionAsync(byte[] imageData)
        {
            // This is a placeholder implementation
            // In a real implementation, you would use liveness detection algorithms
            
            await Task.Delay(200); // Simulate processing time
            
            var random = new Random();
            var isLive = random.NextDouble() > 0.1; // 90% chance of being live
            
            return new LivenessResult
            {
                IsLive = isLive,
                Confidence = random.NextDouble() * 0.3 + 0.7,
                Checks = new Dictionary<string, bool>
                {
                    ["BlinkDetection"] = random.NextDouble() > 0.2,
                    ["FaceMovement"] = random.NextDouble() > 0.2,
                    ["TextureAnalysis"] = random.NextDouble() > 0.1,
                    ["DepthAnalysis"] = random.NextDouble() > 0.15
                }
            };
        }

        private async Task<string> SaveImageAsync(byte[] imageData, Guid userId)
        {
            // This is a placeholder implementation
            // In a real implementation, you would save to cloud storage (Azure Blob, AWS S3, etc.)
            
            await Task.Delay(100); // Simulate upload time
            
            var fileName = $"face_{userId}_{Guid.NewGuid()}.jpg";
            return $"/images/faces/{fileName}";
        }
    }

    // Helper classes
    public class ProcessedImageResult
    {
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class FaceTemplateResult
    {
        public string TemplateData { get; set; } = string.Empty;
        public double Quality { get; set; }
    }

    public class LivenessResult
    {
        public bool IsLive { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, bool> Checks { get; set; } = new();
    }

    public class FaceMatchResult
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? EmployeeId { get; set; }
        public Guid TemplateId { get; set; }
        public double ConfidenceScore { get; set; }
    }
}

