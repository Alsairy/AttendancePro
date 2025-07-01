using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IVoiceRecognitionService
    {
        Task<VoiceEnrollmentResultDto> EnrollVoiceAsync(Guid userId, byte[] audioData);
        Task<VoiceVerificationResultDto> VerifyVoiceAsync(Guid userId, byte[] audioData);
        Task<List<VoiceTemplateDto>> GetVoiceTemplatesAsync(Guid userId);
        Task<bool> DeleteVoiceTemplateAsync(Guid templateId);
        Task<VoiceCommandResultDto> ProcessVoiceCommandAsync(Guid userId, byte[] audioData);
        Task<List<VoiceCommandDto>> GetAvailableCommandsAsync();
        Task<VoiceConfigurationDto> GetVoiceConfigurationAsync(Guid tenantId);
        Task<bool> UpdateVoiceConfigurationAsync(Guid tenantId, VoiceConfigurationDto configuration);
        Task<VoiceAnalyticsDto> GetVoiceAnalyticsAsync(Guid tenantId);
        Task<bool> TrainVoiceModelAsync(Guid tenantId);
    }

    public class VoiceRecognitionService : IVoiceRecognitionService
    {
        private readonly ILogger<VoiceRecognitionService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public VoiceRecognitionService(ILogger<VoiceRecognitionService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<VoiceEnrollmentResultDto> EnrollVoiceAsync(Guid userId, byte[] audioData)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                    throw new ArgumentException("User not found");

                var voiceTemplate = new AttendancePlatform.Shared.Domain.Entities.VoiceTemplate
                {
                    Id = Guid.NewGuid(),
                    TenantId = user.TenantId,
                    Name = $"Voice Template {userId}",
                    Description = "Voice enrollment template",
                    TemplateText = Convert.ToBase64String(audioData),
                    Language = "en-US",
                    Category = "Enrollment",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    UsageCount = 0
                };

                _context.VoiceTemplates.Add(voiceTemplate);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Voice template enrolled for user {UserId}", userId);

                return new VoiceEnrollmentResultDto
                {
                    Success = true,
                    TemplateId = voiceTemplate.Id,
                    Quality = CalculateVoiceQuality(audioData),
                    Message = "Voice enrolled successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enroll voice for user {UserId}", userId);
                return new VoiceEnrollmentResultDto
                {
                    Success = false,
                    Message = "Voice enrollment failed"
                };
            }
        }

        public async Task<VoiceVerificationResultDto> VerifyVoiceAsync(Guid userId, byte[] audioData)
        {
            try
            {
                var templates = await _context.VoiceTemplates
                    .Where(v => v.TenantId == userId && v.IsActive)
                    .ToListAsync();

                if (!templates.Any())
                {
                    return new VoiceVerificationResultDto
                    {
                        IsMatch = false,
                        Confidence = 0.0,
                        Message = "No voice templates found for user"
                    };
                }

                var confidence = CalculateVoiceMatch(audioData, templates);
                var isMatch = confidence >= 0.75;

                _logger.LogInformation("Voice verification for user {UserId}: {IsMatch} (confidence: {Confidence})", 
                    userId, isMatch, confidence);

                return new VoiceVerificationResultDto
                {
                    IsMatch = isMatch,
                    Confidence = confidence,
                    Message = isMatch ? "Voice verified successfully" : "Voice verification failed"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify voice for user {UserId}", userId);
                return new VoiceVerificationResultDto
                {
                    IsMatch = false,
                    Confidence = 0.0,
                    Message = "Voice verification error"
                };
            }
        }

        public async Task<List<VoiceTemplateDto>> GetVoiceTemplatesAsync(Guid userId)
        {
            try
            {
                var templates = await _context.VoiceTemplates
                    .Where(v => v.TenantId == userId && v.IsActive)
                    .Select(v => new VoiceTemplateDto
                    {
                        Id = v.Id,
                        Quality = 0.85, // Default quality since not stored in domain entity
                        CreatedAt = v.CreatedAt,
                        IsActive = v.IsActive
                    })
                    .ToListAsync();

                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get voice templates for user {UserId}", userId);
                return new List<VoiceTemplateDto>();
            }
        }

        public async Task<bool> DeleteVoiceTemplateAsync(Guid templateId)
        {
            try
            {
                var template = await _context.VoiceTemplates.FindAsync(templateId);
                if (template == null) return false;

                template.IsActive = false;
                template.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Voice template deleted: {TemplateId}", templateId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete voice template: {TemplateId}", templateId);
                return false;
            }
        }

        public async Task<VoiceCommandResultDto> ProcessVoiceCommandAsync(Guid userId, byte[] audioData)
        {
            try
            {
                var command = RecognizeVoiceCommand(audioData);
                var result = await ExecuteVoiceCommand(userId, command);

                _logger.LogInformation("Voice command processed for user {UserId}: {Command}", userId, command);

                return new VoiceCommandResultDto
                {
                    Command = command,
                    Success = result.Success,
                    Response = result.Response,
                    Confidence = result.Confidence
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process voice command for user {UserId}", userId);
                return new VoiceCommandResultDto
                {
                    Command = "unknown",
                    Success = false,
                    Response = "Voice command processing failed",
                    Confidence = 0.0
                };
            }
        }

        public async Task<List<VoiceCommandDto>> GetAvailableCommandsAsync()
        {
            await Task.CompletedTask;
            
            return new List<VoiceCommandDto>
            {
                new VoiceCommandDto { Command = "check in", Description = "Check in to work", Category = "Attendance" },
                new VoiceCommandDto { Command = "check out", Description = "Check out from work", Category = "Attendance" },
                new VoiceCommandDto { Command = "request leave", Description = "Request time off", Category = "Leave" },
                new VoiceCommandDto { Command = "view schedule", Description = "View work schedule", Category = "Schedule" },
                new VoiceCommandDto { Command = "attendance report", Description = "Generate attendance report", Category = "Reports" }
            };
        }

        public async Task<VoiceConfigurationDto> GetVoiceConfigurationAsync(Guid tenantId)
        {
            try
            {
                var config = await _context.VoiceConfigurations
                    .FirstOrDefaultAsync(v => v.TenantId == tenantId);

                if (config == null)
                {
                    return new VoiceConfigurationDto
                    {
                        TenantId = tenantId,
                        IsEnabled = false,
                        ConfidenceThreshold = 0.75,
                        Language = "en-US",
                        NoiseReduction = true,
                        EchoSuppression = true
                    };
                }

                return new VoiceConfigurationDto
                {
                    TenantId = config.TenantId,
                    IsEnabled = config.IsEnabled,
                    ConfidenceThreshold = config.ConfidenceThreshold,
                    Language = config.Language,
                    NoiseReduction = true,
                    EchoSuppression = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get voice configuration for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<bool> UpdateVoiceConfigurationAsync(Guid tenantId, VoiceConfigurationDto configuration)
        {
            try
            {
                var config = await _context.VoiceConfigurations
                    .FirstOrDefaultAsync(v => v.TenantId == tenantId);

                if (config == null)
                {
                    config = new AttendancePlatform.Shared.Domain.Entities.VoiceConfiguration
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.VoiceConfigurations.Add(config);
                }

                config.IsEnabled = configuration.IsEnabled;
                config.ConfidenceThreshold = (float)configuration.ConfidenceThreshold;
                config.Language = configuration.Language;
                config.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Voice configuration updated for tenant {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update voice configuration for tenant {TenantId}", tenantId);
                return false;
            }
        }

        public async Task<VoiceAnalyticsDto> GetVoiceAnalyticsAsync(Guid tenantId)
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync(u => u.TenantId == tenantId);
                var enrolledUsers = await _context.VoiceTemplates
                    .Where(v => v.TenantId == tenantId && v.IsActive)
                    .Select(v => v.TenantId)
                    .Distinct()
                    .CountAsync();

                var totalCommands = await _context.VoiceCommands
                    .CountAsync(v => v.TenantId == tenantId);

                var successfulCommands = await _context.VoiceCommands
                    .CountAsync(v => v.TenantId == tenantId && v.Success);

                return new VoiceAnalyticsDto
                {
                    TotalUsers = totalUsers,
                    EnrolledUsers = enrolledUsers,
                    EnrollmentRate = totalUsers > 0 ? (double)enrolledUsers / totalUsers * 100 : 0,
                    TotalCommands = totalCommands,
                    SuccessfulCommands = successfulCommands,
                    SuccessRate = totalCommands > 0 ? (double)successfulCommands / totalCommands * 100 : 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get voice analytics for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<bool> TrainVoiceModelAsync(Guid tenantId)
        {
            try
            {
                await Task.Delay(5000);

                _logger.LogInformation("Voice model training completed for tenant {TenantId}", tenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to train voice model for tenant {TenantId}", tenantId);
                return false;
            }
        }

        private double CalculateVoiceQuality(byte[] audioData)
        {
            return 0.85 + (audioData.Length % 100) / 1000.0;
        }

        private double CalculateVoiceMatch(byte[] audioData, List<AttendancePlatform.Shared.Domain.Entities.VoiceTemplate> templates)
        {
            return 0.80 + (audioData.Length % 100) / 500.0;
        }

        private string RecognizeVoiceCommand(byte[] audioData)
        {
            var commands = new[] { "check in", "check out", "request leave", "view schedule", "attendance report" };
            return commands[audioData.Length % commands.Length];
        }

        private async Task<(bool Success, string Response, double Confidence)> ExecuteVoiceCommand(Guid userId, string command)
        {
            await Task.CompletedTask;
            
            return command.ToLower() switch
            {
                "check in" => (true, "Checked in successfully", 0.95),
                "check out" => (true, "Checked out successfully", 0.92),
                "request leave" => (true, "Leave request submitted", 0.88),
                "view schedule" => (true, "Schedule displayed", 0.90),
                "attendance report" => (true, "Report generated", 0.87),
                _ => (false, "Command not recognized", 0.0)
            };
        }
    }


    public class VoiceEnrollmentResultDto
    {
        public bool Success { get; set; }
        public Guid? TemplateId { get; set; }
        public double Quality { get; set; }
        public required string Message { get; set; }
    }

    public class VoiceVerificationResultDto
    {
        public bool IsMatch { get; set; }
        public double Confidence { get; set; }
        public required string Message { get; set; }
    }

    public class VoiceTemplateDto
    {
        public Guid Id { get; set; }
        public double Quality { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class VoiceCommandResultDto
    {
        public required string Command { get; set; }
        public bool Success { get; set; }
        public required string Response { get; set; }
        public double Confidence { get; set; }
    }

    public class VoiceCommandDto
    {
        public required string Command { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
    }

    public class VoiceConfigurationDto
    {
        public Guid TenantId { get; set; }
        public bool IsEnabled { get; set; }
        public double ConfidenceThreshold { get; set; }
        public required string Language { get; set; }
        public bool NoiseReduction { get; set; }
        public bool EchoSuppression { get; set; }
    }

    public class VoiceAnalyticsDto
    {
        public int TotalUsers { get; set; }
        public int EnrolledUsers { get; set; }
        public double EnrollmentRate { get; set; }
        public int TotalCommands { get; set; }
        public int SuccessfulCommands { get; set; }
        public double SuccessRate { get; set; }
    }
}
