using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.VoiceRecognition.Api.Services
{
    public interface IVoiceRecognitionService
    {
        Task<ApiResponse<VoiceEnrollmentDto>> EnrollVoiceAsync(Guid userId, byte[] audioData);
        Task<ApiResponse<VoiceVerificationDto>> VerifyVoiceAsync(Guid userId, byte[] audioData);
        Task<ApiResponse<bool>> DeleteVoiceTemplateAsync(Guid userId);
        Task<ApiResponse<VoiceTemplateDto>> GetVoiceTemplateAsync(Guid userId);
        Task<ApiResponse<List<VoiceEnrollmentDto>>> GetUserVoiceEnrollmentsAsync(Guid tenantId);
    }

    public class VoiceRecognitionService : IVoiceRecognitionService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<VoiceRecognitionService> _logger;

        public VoiceRecognitionService(
            AttendancePlatformDbContext context,
            ILogger<VoiceRecognitionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ApiResponse<VoiceEnrollmentDto>> EnrollVoiceAsync(Guid userId, byte[] audioData)
        {
            try
            {
                _logger.LogInformation("Starting voice enrollment for user {UserId}", userId);

                if (audioData == null || audioData.Length == 0)
                {
                    _logger.LogWarning("Empty audio data provided for user {UserId}", userId);
                    return ApiResponse<VoiceEnrollmentDto>.ErrorResult("Audio data is required");
                }

                if (audioData.Length < 1024)
                {
                    _logger.LogWarning("Audio data too short for user {UserId}: {Length} bytes", userId, audioData.Length);
                    return ApiResponse<VoiceEnrollmentDto>.ErrorResult("Audio data is too short for voice enrollment");
                }

                var voiceTemplate = await ExtractVoiceTemplateAsync(audioData);
                if (voiceTemplate == null || voiceTemplate.Length == 0)
                {
                    _logger.LogError("Could not extract voice template for user {UserId}", userId);
                    return ApiResponse<VoiceEnrollmentDto>.ErrorResult("Could not extract voice template from audio data");
                }

                var biometric = await _context.Biometrics
                    .FirstOrDefaultAsync(b => b.UserId == userId.ToString());

                if (biometric == null)
                {
                    biometric = new Biometrics
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.Biometrics.Add(biometric);
                }

                biometric.VoiceTemplate = Convert.ToBase64String(voiceTemplate);
                biometric.IsVoiceEnrolled = true;
                biometric.VoiceEnrolledAt = DateTime.UtcNow;
                biometric.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var result = new VoiceEnrollmentDto
                {
                    UserId = userId,
                    EnrollmentDate = biometric.VoiceEnrolledAt.Value,
                    IsActive = true,
                    TemplateSize = voiceTemplate.Length,
                    Message = "Voice enrolled successfully"
                };

                _logger.LogInformation("Voice enrollment completed successfully for user {UserId}", userId);
                return ApiResponse<VoiceEnrollmentDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling voice for user {UserId}", userId);
                return ApiResponse<VoiceEnrollmentDto>.ErrorResult("Voice enrollment failed due to internal error");
            }
        }

        public async Task<ApiResponse<VoiceVerificationDto>> VerifyVoiceAsync(Guid userId, byte[] audioData)
        {
            try
            {
                _logger.LogInformation("Starting voice verification for user {UserId}", userId);

                if (audioData == null || audioData.Length == 0)
                {
                    _logger.LogWarning("Empty audio data provided for verification for user {UserId}", userId);
                    return ApiResponse<VoiceVerificationDto>.ErrorResult("Audio data is required for verification");
                }

                var biometric = await _context.Biometrics
                    .FirstOrDefaultAsync(b => b.UserId == userId.ToString() && b.IsVoiceEnrolled == true);

                if (biometric == null || string.IsNullOrEmpty(biometric.VoiceTemplate))
                {
                    _logger.LogWarning("No voice template found for user {UserId}", userId);
                    return ApiResponse<VoiceVerificationDto>.ErrorResult("No voice template found for user");
                }

                var storedTemplate = Convert.FromBase64String(biometric.VoiceTemplate);
                var currentTemplate = await ExtractVoiceTemplateAsync(audioData);

                if (currentTemplate == null || currentTemplate.Length == 0)
                {
                    _logger.LogError("Could not extract voice template from verification audio for user {UserId}", userId);
                    return ApiResponse<VoiceVerificationDto>.ErrorResult("Could not process verification audio");
                }

                var similarity = CalculateVoiceSimilarity(storedTemplate, currentTemplate);
                var threshold = 0.75; // 75% similarity threshold
                var isMatch = similarity >= threshold;

                var result = new VoiceVerificationDto
                {
                    UserId = userId,
                    IsMatch = isMatch,
                    Confidence = similarity,
                    Threshold = threshold,
                    VerificationDate = DateTime.UtcNow,
                    Message = isMatch ? "Voice verification successful" : "Voice verification failed"
                };

                _logger.LogInformation("Voice verification completed for user {UserId}: Match={IsMatch}, Confidence={Confidence}", 
                    userId, isMatch, similarity);

                return ApiResponse<VoiceVerificationDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying voice for user {UserId}", userId);
                return ApiResponse<VoiceVerificationDto>.ErrorResult("Voice verification failed due to internal error");
            }
        }

        public async Task<ApiResponse<bool>> DeleteVoiceTemplateAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Deleting voice template for user {UserId}", userId);

                var biometric = await _context.Biometrics
                    .FirstOrDefaultAsync(b => b.UserId == userId.ToString());

                if (biometric == null)
                {
                    _logger.LogWarning("No biometric record found for user {UserId}", userId);
                    return ApiResponse<bool>.ErrorResult("No biometric record found for user");
                }

                biometric.VoiceTemplate = null;
                biometric.IsVoiceEnrolled = false;
                biometric.VoiceEnrolledAt = null;
                biometric.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Voice template deleted successfully for user {UserId}", userId);
                return ApiResponse<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting voice template for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResult("Failed to delete voice template");
            }
        }

        public async Task<ApiResponse<VoiceTemplateDto>> GetVoiceTemplateAsync(Guid userId)
        {
            try
            {
                _logger.LogInformation("Retrieving voice template info for user {UserId}", userId);

                var biometric = await _context.Biometrics
                    .FirstOrDefaultAsync(b => b.UserId == userId.ToString());

                if (biometric == null || !biometric.IsVoiceEnrolled)
                {
                    _logger.LogWarning("No voice template found for user {UserId}", userId);
                    return ApiResponse<VoiceTemplateDto>.ErrorResult("No voice template found for user");
                }

                var result = new VoiceTemplateDto
                {
                    UserId = userId,
                    IsEnrolled = biometric.IsVoiceEnrolled,
                    EnrollmentDate = biometric.VoiceEnrolledAt,
                    TemplateSize = !string.IsNullOrEmpty(biometric.VoiceTemplate) 
                        ? Convert.FromBase64String(biometric.VoiceTemplate).Length 
                        : 0,
                    LastUpdated = biometric.UpdatedAt
                };

                return ApiResponse<VoiceTemplateDto>.SuccessResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voice template for user {UserId}", userId);
                return ApiResponse<VoiceTemplateDto>.ErrorResult("Failed to retrieve voice template information");
            }
        }

        public async Task<ApiResponse<List<VoiceEnrollmentDto>>> GetUserVoiceEnrollmentsAsync(Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Retrieving voice enrollments for tenant {TenantId}", tenantId);

                var enrollments = await _context.Biometrics
                    .Where(b => b.IsVoiceEnrolled == true)
                    .Join(_context.Users, 
                        b => b.UserId, 
                        u => u.Id.ToString(), 
                        (b, u) => new { Biometric = b, User = u })
                    .Where(x => x.User.TenantId == tenantId)
                    .Select(x => new VoiceEnrollmentDto
                    {
                        UserId = Guid.Parse(x.Biometric.UserId),
                        EnrollmentDate = x.Biometric.VoiceEnrolledAt.Value,
                        IsActive = x.Biometric.IsActive,
                        TemplateSize = !string.IsNullOrEmpty(x.Biometric.VoiceTemplate) 
                            ? Convert.FromBase64String(x.Biometric.VoiceTemplate).Length 
                            : 0,
                        Message = "Active voice enrollment"
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} voice enrollments for tenant {TenantId}", enrollments.Count, tenantId);
                return ApiResponse<List<VoiceEnrollmentDto>>.SuccessResult(enrollments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving voice enrollments for tenant {TenantId}", tenantId);
                return ApiResponse<List<VoiceEnrollmentDto>>.ErrorResult("Failed to retrieve voice enrollments");
            }
        }

        private async Task<byte[]> ExtractVoiceTemplateAsync(byte[] audioData)
        {
            try
            {
                
                var features = new List<byte>();
                
                var sampleRate = 16000; // Assume 16kHz sample rate
                var frameSize = 1024;
                
                for (int i = 0; i < audioData.Length - frameSize; i += frameSize)
                {
                    var frame = audioData.Skip(i).Take(frameSize).ToArray();
                    
                    var energy = CalculateFrameEnergy(frame);
                    var zeroCrossings = CalculateZeroCrossings(frame);
                    var spectralCentroid = CalculateSpectralCentroid(frame);
                    
                    features.AddRange(BitConverter.GetBytes(energy));
                    features.AddRange(BitConverter.GetBytes(zeroCrossings));
                    features.AddRange(BitConverter.GetBytes(spectralCentroid));
                }
                
                var template = features.Take(2048).ToArray();
                
                _logger.LogDebug("Extracted voice template of size {Size} bytes", template.Length);
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting voice template");
                return null;
            }
        }

        private double CalculateVoiceSimilarity(byte[] template1, byte[] template2)
        {
            try
            {
                if (template1 == null || template2 == null || template1.Length == 0 || template2.Length == 0)
                    return 0.0;

                var minLength = Math.Min(template1.Length, template2.Length);
                
                double dotProduct = 0.0;
                double norm1 = 0.0;
                double norm2 = 0.0;
                
                for (int i = 0; i < minLength; i++)
                {
                    dotProduct += template1[i] * template2[i];
                    norm1 += template1[i] * template1[i];
                    norm2 += template2[i] * template2[i];
                }
                
                if (norm1 == 0.0 || norm2 == 0.0)
                    return 0.0;
                
                var similarity = dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
                
                return Math.Max(0.0, Math.Min(1.0, (similarity + 1.0) / 2.0));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating voice similarity");
                return 0.0;
            }
        }

        private float CalculateFrameEnergy(byte[] frame)
        {
            float energy = 0.0f;
            for (int i = 0; i < frame.Length; i++)
            {
                energy += frame[i] * frame[i];
            }
            return energy / frame.Length;
        }

        private int CalculateZeroCrossings(byte[] frame)
        {
            int crossings = 0;
            for (int i = 1; i < frame.Length; i++)
            {
                if ((frame[i] >= 128 && frame[i-1] < 128) || (frame[i] < 128 && frame[i-1] >= 128))
                {
                    crossings++;
                }
            }
            return crossings;
        }

        private float CalculateSpectralCentroid(byte[] frame)
        {
            float weightedSum = 0.0f;
            float magnitudeSum = 0.0f;
            
            for (int i = 0; i < frame.Length; i++)
            {
                float magnitude = Math.Abs(frame[i] - 128); // Center around 0
                weightedSum += i * magnitude;
                magnitudeSum += magnitude;
            }
            
            return magnitudeSum > 0 ? weightedSum / magnitudeSum : 0.0f;
        }
    }

    public class VoiceEnrollmentDto
    {
        public Guid UserId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
        public int TemplateSize { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class VoiceVerificationDto
    {
        public Guid UserId { get; set; }
        public bool IsMatch { get; set; }
        public double Confidence { get; set; }
        public double Threshold { get; set; }
        public DateTime VerificationDate { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class VoiceTemplateDto
    {
        public Guid UserId { get; set; }
        public bool IsEnrolled { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public int TemplateSize { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
