using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace AttendancePlatform.Api.Middleware
{
    public class HostFilteringMiddleware
    {
        private readonly RequestDelegate _next;

        public HostFilteringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }
}
