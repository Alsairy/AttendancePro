using Serilog;
using Serilog.Context;

namespace AttendancePlatform.Api.Services
{
    public interface ILoggingService
    {
        void LogSecurityEvent(string eventType, string userId, string details, bool isSuccess = true);
        void LogUserActivity(string userId, string activity, string details);
        void LogSystemEvent(string eventType, string details);
        void LogError(Exception exception, string context, string userId = null);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void LogSecurityEvent(string eventType, string userId, string details, bool isSuccess = true)
        {
            using (LogContext.PushProperty("EventType", "Security"))
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("SecurityEventType", eventType))
            using (LogContext.PushProperty("Success", isSuccess))
            {
                if (isSuccess)
                {
                    _logger.LogInformation("Security Event: {EventType} for User {UserId}. Details: {Details}", 
                        eventType, userId, details);
                }
                else
                {
                    _logger.LogWarning("Security Event Failed: {EventType} for User {UserId}. Details: {Details}", 
                        eventType, userId, details);
                }
            }
        }

        public void LogUserActivity(string userId, string activity, string details)
        {
            using (LogContext.PushProperty("EventType", "UserActivity"))
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("Activity", activity))
            {
                _logger.LogInformation("User Activity: {Activity} by User {UserId}. Details: {Details}", 
                    activity, userId, details);
            }
        }

        public void LogSystemEvent(string eventType, string details)
        {
            using (LogContext.PushProperty("EventType", "System"))
            using (LogContext.PushProperty("SystemEventType", eventType))
            {
                _logger.LogInformation("System Event: {EventType}. Details: {Details}", eventType, details);
            }
        }

        public void LogError(Exception exception, string context, string userId = null)
        {
            using (LogContext.PushProperty("EventType", "Error"))
            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("Context", context))
            {
                _logger.LogError(exception, "Error in {Context} for User {UserId}: {Message}", 
                    context, userId, exception.Message);
            }
        }
    }
}
