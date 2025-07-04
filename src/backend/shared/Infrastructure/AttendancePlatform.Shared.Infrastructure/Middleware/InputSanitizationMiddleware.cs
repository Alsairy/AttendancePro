using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace AttendancePlatform.Shared.Infrastructure.Middleware
{
    public class InputSanitizationMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Regex HtmlTagRegex = new(@"<[^>]*>", RegexOptions.Compiled);
        private static readonly Regex ScriptTagRegex = new(@"<script[^>]*>.*?</script>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex JavaScriptRegex = new(@"javascript:", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public InputSanitizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.ContentType?.Contains("application/json") == true)
            {
                context.Request.EnableBuffering();
                
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    var sanitizedBody = SanitizeInput(body);
                    
                    if (sanitizedBody != body)
                    {
                        var sanitizedBytes = Encoding.UTF8.GetBytes(sanitizedBody);
                        context.Request.Body = new MemoryStream(sanitizedBytes);
                        context.Request.ContentLength = sanitizedBytes.Length;
                    }
                }
            }

            await _next(context);
        }

        private static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sanitized = input;
            
            sanitized = ScriptTagRegex.Replace(sanitized, string.Empty);
            sanitized = JavaScriptRegex.Replace(sanitized, string.Empty);
            sanitized = HtmlTagRegex.Replace(sanitized, string.Empty);
            
            sanitized = sanitized.Replace("&lt;", "<")
                                 .Replace("&gt;", ">")
                                 .Replace("&amp;", "&")
                                 .Replace("&quot;", "\"")
                                 .Replace("&#x27;", "'")
                                 .Replace("&#x2F;", "/");

            return sanitized;
        }
    }
}
