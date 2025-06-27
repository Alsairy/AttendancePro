using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.VoiceRecognition.Api.Services
{
    public interface IVoiceRecognitionService
    {
        Task<VoiceRecognitionResultDto> ProcessVoiceCommandAsync(VoiceCommandDto voiceCommand);
        Task<VoiceTranscriptionDto> TranscribeAudioAsync(AudioDataDto audioData);
        Task<VoiceBiometricResultDto> VerifyVoiceBiometricAsync(VoiceBiometricDto voiceBiometric);
        Task<bool> EnrollVoiceBiometricAsync(VoiceEnrollmentDto voiceEnrollment);
        Task<VoiceProfileDto> CreateVoiceProfileAsync(CreateVoiceProfileDto createVoiceProfile);
        Task<VoiceProfileDto> UpdateVoiceProfileAsync(Guid profileId, UpdateVoiceProfileDto updateVoiceProfile);
        Task<bool> DeleteVoiceProfileAsync(Guid profileId);
        Task<VoiceProfileDto> GetVoiceProfileAsync(Guid profileId);
        Task<IEnumerable<VoiceProfileDto>> GetUserVoiceProfilesAsync(Guid userId);
        Task<VoiceCommandConfigDto> GetVoiceCommandConfigAsync(Guid tenantId);
        Task<VoiceCommandConfigDto> UpdateVoiceCommandConfigAsync(Guid tenantId, UpdateVoiceCommandConfigDto config);
        Task<IEnumerable<SupportedLanguageDto>> GetSupportedLanguagesAsync();
        Task<VoiceAnalyticsDto> GetVoiceAnalyticsAsync(Guid tenantId, DateTime startDate, DateTime endDate);
        Task<bool> TrainVoiceModelAsync(Guid userId, IEnumerable<VoiceTrainingDataDto> trainingData);
        Task<VoiceQualityAssessmentDto> AssessVoiceQualityAsync(AudioDataDto audioData);
        Task<bool> TestVoiceRecognitionAsync(Guid userId, AudioDataDto testAudio);
        Task<VoiceSecurityResultDto> ValidateVoiceSecurityAsync(VoiceSecurityDto voiceSecurity);
        Task<IEnumerable<VoiceCommandDto>> GetAvailableCommandsAsync(Guid tenantId);
        Task<VoiceCommandDto> CreateCustomCommandAsync(CreateCustomCommandDto createCustomCommand);
        Task<bool> DeleteCustomCommandAsync(Guid commandId);
    }
}
