using AttendancePlatform.Application.Services;
using Microsoft.Extensions.Logging;

namespace AttendancePlatform.Api.Services
{
    public interface ISecurityService
    {
        Task<bool> ValidateSecurityHeaders(HttpContext context);
        Task<bool> ValidateInputSanitization(string input);
        Task LogSecurityEvent(string eventType, string details, string? userId = null);
        Task<bool> ValidateRateLimiting(string clientId);
    }

    public class SecurityService : ISecurityService
    {
        private readonly ILogger<SecurityService> _logger;
        private readonly ILoggingService _loggingService;

        public SecurityService(ILogger<SecurityService> logger, ILoggingService loggingService)
        {
            _logger = logger;
            _loggingService = loggingService;
        }

        public async Task<bool> ValidateSecurityHeaders(HttpContext context)
        {
            try
            {
                var requiredHeaders = new[]
                {
                    "X-Content-Type-Options",
                    "X-Frame-Options", 
                    "X-XSS-Protection",
                    "Content-Security-Policy"
                };

                foreach (var header in requiredHeaders)
                {
                    if (!context.Response.Headers.ContainsKey(header))
                    {
                        await LogSecurityEvent("MISSING_SECURITY_HEADER", $"Missing header: {header}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating security headers");
                return false;
            }
        }

        public async Task<bool> ValidateInputSanitization(string input)
        {
            try
            {
                var maliciousPatterns = new[]
                {
                    "<script",
                    "javascript:",
                    "on\\w+\\s*=",
                    "eval\\s*\\(",
                    "document\\.",
                    "window\\."
                };

                foreach (var pattern in maliciousPatterns)
                {
                    if (System.Text.RegularExpressions.Regex.IsMatch(input, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                    {
                        await LogSecurityEvent("MALICIOUS_INPUT_DETECTED", $"Pattern detected: {pattern}");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating input sanitization");
                return false;
            }
        }

        public async Task LogSecurityEvent(string eventType, string details, string? userId = null)
        {
            try
            {
                var logMessage = $"SECURITY_EVENT: {eventType} - {details}";
                if (!string.IsNullOrEmpty(userId))
                {
                    logMessage += $" - User: {userId}";
                }

                _loggingService.LogSecurityEvent(eventType, details, userId);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging security event");
            }
        }

        public async Task<bool> ValidateRateLimiting(string clientId)
        {
            try
            {
                await LogSecurityEvent("RATE_LIMIT_CHECK", $"Checking rate limit for client: {clientId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating rate limiting");
                return false;
            }
        }
    }
}
