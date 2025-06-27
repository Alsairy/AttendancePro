namespace AttendancePlatform.Shared.Domain.DTOs
{
    public class VoiceCommandDto
    {
        public Guid Id { get; set; }
        public string Command { get; set; }
        public string Language { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class VoiceRecognitionResultDto
    {
        public bool Success { get; set; }
        public string TranscribedText { get; set; }
        public string RecognizedCommand { get; set; }
        public Dictionary<string, object> ExtractedParameters { get; set; }
        public float Confidence { get; set; }
        public string Language { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class AudioDataDto
    {
        public byte[] AudioData { get; set; }
        public string Format { get; set; }
        public int SampleRate { get; set; }
        public int Channels { get; set; }
        public TimeSpan Duration { get; set; }
        public string Language { get; set; }
    }

    public class VoiceTranscriptionDto
    {
        public string TranscribedText { get; set; }
        public float Confidence { get; set; }
        public string Language { get; set; }
        public TimeSpan ProcessingTime { get; set; }
        public IEnumerable<WordTimingDto> WordTimings { get; set; }
    }

    public class WordTimingDto
    {
        public string Word { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public float Confidence { get; set; }
    }

    public class VoiceBiometricDto
    {
        public Guid UserId { get; set; }
        public byte[] VoiceData { get; set; }
        public string VoiceText { get; set; }
        public Guid TenantId { get; set; }
    }

    public class VoiceBiometricResultDto
    {
        public bool IsMatch { get; set; }
        public float MatchScore { get; set; }
        public float Threshold { get; set; }
        public bool LivenessDetected { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class VoiceEnrollmentDto
    {
        public Guid UserId { get; set; }
        public IEnumerable<byte[]> VoiceSamples { get; set; }
        public string EnrollmentText { get; set; }
        public string Language { get; set; }
        public Guid TenantId { get; set; }
    }

    public class VoiceProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Language { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdated { get; set; }
        public int SampleCount { get; set; }
        public float QualityScore { get; set; }
    }

    public class CreateVoiceProfileDto
    {
        public Guid UserId { get; set; }
        public string Language { get; set; }
        public IEnumerable<byte[]> InitialSamples { get; set; }
        public string EnrollmentText { get; set; }
    }

    public class UpdateVoiceProfileDto
    {
        public string Language { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<byte[]> AdditionalSamples { get; set; }
    }

    public class VoiceCommandConfigDto
    {
        public Guid TenantId { get; set; }
        public IEnumerable<string> EnabledLanguages { get; set; }
        public IEnumerable<VoiceCommandMappingDto> CommandMappings { get; set; }
        public VoiceSecuritySettingsDto SecuritySettings { get; set; }
        public bool VoiceBiometricEnabled { get; set; }
        public float ConfidenceThreshold { get; set; }
    }

    public class VoiceCommandMappingDto
    {
        public string Command { get; set; }
        public string Action { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool RequiresBiometric { get; set; }
    }

    public class VoiceSecuritySettingsDto
    {
        public bool RequireLivenessDetection { get; set; }
        public float BiometricThreshold { get; set; }
        public int MaxRetryAttempts { get; set; }
        public TimeSpan LockoutDuration { get; set; }
        public bool LogAllAttempts { get; set; }
    }

    public class UpdateVoiceCommandConfigDto
    {
        public IEnumerable<string> EnabledLanguages { get; set; }
        public IEnumerable<VoiceCommandMappingDto> CommandMappings { get; set; }
        public VoiceSecuritySettingsDto SecuritySettings { get; set; }
        public bool VoiceBiometricEnabled { get; set; }
        public float ConfidenceThreshold { get; set; }
    }

    public class SupportedLanguageDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NativeName { get; set; }
        public bool SupportsBiometric { get; set; }
        public bool SupportsCommands { get; set; }
    }

    public class VoiceAnalyticsDto
    {
        public int TotalCommands { get; set; }
        public int SuccessfulCommands { get; set; }
        public int FailedCommands { get; set; }
        public float AverageConfidence { get; set; }
        public Dictionary<string, int> CommandUsage { get; set; }
        public Dictionary<string, int> LanguageUsage { get; set; }
        public IEnumerable<VoiceUsagePatternDto> UsagePatterns { get; set; }
    }

    public class VoiceUsagePatternDto
    {
        public DateTime Date { get; set; }
        public int CommandCount { get; set; }
        public float AverageConfidence { get; set; }
        public string MostUsedCommand { get; set; }
    }

    public class VoiceTrainingDataDto
    {
        public byte[] AudioData { get; set; }
        public string TranscriptionText { get; set; }
        public string Language { get; set; }
        public Dictionary<string, object> Metadata { get; set; }
    }

    public class VoiceQualityAssessmentDto
    {
        public float QualityScore { get; set; }
        public float NoiseLevel { get; set; }
        public float Clarity { get; set; }
        public bool IsAcceptable { get; set; }
        public IEnumerable<string> QualityIssues { get; set; }
        public string Recommendation { get; set; }
    }

    public class VoiceSecurityDto
    {
        public Guid UserId { get; set; }
        public byte[] VoiceData { get; set; }
        public string SecurityContext { get; set; }
        public Dictionary<string, object> SecurityParameters { get; set; }
    }

    public class VoiceSecurityResultDto
    {
        public bool IsSecure { get; set; }
        public float SecurityScore { get; set; }
        public IEnumerable<string> SecurityFlags { get; set; }
        public bool AntiSpoofingPassed { get; set; }
        public string RiskLevel { get; set; }
    }

    public class CreateCustomCommandDto
    {
        public string Command { get; set; }
        public string Action { get; set; }
        public IEnumerable<string> Aliases { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public bool RequiresBiometric { get; set; }
        public Guid TenantId { get; set; }
        public string Description { get; set; }
    }
}
