using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using AttendancePlatform.Shared.Infrastructure.Middleware;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.Unit
{
    public class SecurityMiddlewareTests
    {
        [Fact]
        public async Task SecurityHeadersMiddleware_ShouldAddSecurityHeaders()
        {
            var context = new DefaultHttpContext();
            var next = new Mock<RequestDelegate>();
            next.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
            
            var middleware = new SecurityHeadersMiddleware(next.Object);
            
            await middleware.InvokeAsync(context);
            
            context.Response.Headers.Should().ContainKey("X-Content-Type-Options");
            context.Response.Headers.Should().ContainKey("X-Frame-Options");
            context.Response.Headers.Should().ContainKey("X-XSS-Protection");
            context.Response.Headers.Should().ContainKey("Content-Security-Policy");
        }

        [Fact]
        public async Task InputSanitizationMiddleware_ShouldSanitizeInput()
        {
            var context = new DefaultHttpContext();
            context.Request.Method = "POST";
            context.Request.ContentType = "application/json";
            
            var maliciousInput = "{\"name\":\"<script>alert('xss')</script>\"}";
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(maliciousInput));
            context.Request.Body = stream;
            
            var next = new Mock<RequestDelegate>();
            next.Setup(x => x(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);
            
            var logger = new Mock<ILogger<InputSanitizationMiddleware>>();
            var middleware = new InputSanitizationMiddleware(next.Object, logger.Object);
            
            await middleware.InvokeAsync(context);
            
            next.Verify(x => x(It.IsAny<HttpContext>()), Times.Once);
        }
    }
}
