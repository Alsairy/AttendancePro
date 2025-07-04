using Microsoft.AspNetCore.Antiforgery;

namespace AttendancePlatform.Api.Middleware
{
    public class CsrfValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery _antiforgery;

        public CsrfValidationMiddleware(RequestDelegate next, IAntiforgery antiforgery)
        {
            _next = next;
            _antiforgery = antiforgery;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (HttpMethods.IsPost(context.Request.Method) ||
                HttpMethods.IsPut(context.Request.Method) ||
                HttpMethods.IsDelete(context.Request.Method))
            {
                if (context.Request.Path.StartsWithSegments("/api"))
                {
                    try
                    {
                        await _antiforgery.ValidateRequestAsync(context);
                    }
                    catch (AntiforgeryValidationException)
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid CSRF token");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
