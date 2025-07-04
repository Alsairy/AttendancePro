using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.Security
{
    public class SecurityTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SecurityTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task API_ShouldHaveSecurityHeaders()
        {
            var response = await _client.GetAsync("/health");

            response.Headers.Should().ContainKey("X-Content-Type-Options");
            response.Headers.Should().ContainKey("X-Frame-Options");
            response.Headers.Should().ContainKey("X-XSS-Protection");
            response.Headers.GetValues("X-Content-Type-Options").Should().Contain("nosniff");
            response.Headers.GetValues("X-Frame-Options").Should().Contain("DENY");
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutAuth_ShouldReturn401()
        {
            var response = await _client.GetAsync("/api/attendance/history");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task MaliciousInput_ShouldBeSanitized()
        {
            var maliciousPayload = "{\"name\":\"<script>alert('xss')</script>\"}";
            var content = new StringContent(maliciousPayload, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/test/input", content);

            var responseContent = await response.Content.ReadAsStringAsync();
            responseContent.Should().NotContain("<script>");
            responseContent.Should().NotContain("alert");
        }

        [Theory]
        [InlineData("' OR '1'='1")]
        [InlineData("<script>alert('xss')</script>")]
        [InlineData("javascript:alert('xss')")]
        public async Task InputValidation_ShouldRejectMaliciousInput(string maliciousInput)
        {
            var payload = $"{{\"email\":\"{maliciousInput}\"}}";
            var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
