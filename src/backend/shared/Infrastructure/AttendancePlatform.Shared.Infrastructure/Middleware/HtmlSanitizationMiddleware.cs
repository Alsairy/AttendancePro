using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AttendancePlatform.Shared.Infrastructure.Middleware
{
    public class HtmlSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<HtmlSanitizationMiddleware> _logger;

        public HtmlSanitizationMiddleware(RequestDelegate next, ILogger<HtmlSanitizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method == "POST" || context.Request.Method == "PUT")
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body) && context.Request.ContentType?.Contains("application/json") == true)
                {
                    try
                    {
                        var sanitizedBody = SanitizeJsonContent(body);
                        if (sanitizedBody != body)
                        {
                            _logger.LogWarning("HTML content detected and sanitized in request body");
                            
                            var sanitizedBytes = Encoding.UTF8.GetBytes(sanitizedBody);
                            context.Request.Body = new MemoryStream(sanitizedBytes);
                            context.Request.ContentLength = sanitizedBytes.Length;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sanitizing request body");
                    }
                }
            }

            await _next(context);
        }

        private string SanitizeJsonContent(string jsonContent)
        {
            var htmlPattern = @"<[^>]*>";
            var scriptPattern = @"<script[^>]*>.*?</script>";
            var onEventPattern = @"on\w+\s*=\s*[""'][^""']*[""']";
            var javascriptPattern = @"javascript\s*:";

            var sanitized = Regex.Replace(jsonContent, scriptPattern, "", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, onEventPattern, "", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, javascriptPattern, "", RegexOptions.IgnoreCase);
            sanitized = Regex.Replace(sanitized, htmlPattern, "");

            return sanitized;
        }
    }
}
