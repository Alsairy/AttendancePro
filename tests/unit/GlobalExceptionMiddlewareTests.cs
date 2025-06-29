using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using AttendancePlatform.Shared.Infrastructure.Middleware;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.Unit
{
    public class GlobalExceptionMiddlewareTests
    {
        private readonly Mock<ILogger<GlobalExceptionMiddleware>> _loggerMock;
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly GlobalExceptionMiddleware _middleware;

        public GlobalExceptionMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionMiddleware>>();
            _nextMock = new Mock<RequestDelegate>();
            _middleware = new GlobalExceptionMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_WhenNoException_ShouldCallNext()
        {
            var context = new DefaultHttpContext();
            
            await _middleware.InvokeAsync(context);
            
            _nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WhenUnauthorizedAccessException_ShouldReturn401()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _nextMock.Setup(x => x(context)).ThrowsAsync(new UnauthorizedAccessException("Test exception"));
            
            await _middleware.InvokeAsync(context);
            
            context.Response.StatusCode.Should().Be(401);
            context.Response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task InvokeAsync_WhenArgumentException_ShouldReturn400()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _nextMock.Setup(x => x(context)).ThrowsAsync(new ArgumentException("Test exception"));
            
            await _middleware.InvokeAsync(context);
            
            context.Response.StatusCode.Should().Be(400);
            context.Response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task InvokeAsync_WhenGenericException_ShouldReturn500()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            _nextMock.Setup(x => x(context)).ThrowsAsync(new Exception("Test exception"));
            
            await _middleware.InvokeAsync(context);
            
            context.Response.StatusCode.Should().Be(500);
            context.Response.ContentType.Should().Be("application/json");
        }
    }
}
