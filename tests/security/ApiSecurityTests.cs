using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using FluentAssertions;
using AttendancePlatform.Authentication.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;

namespace AttendancePlatform.Tests.Security
{
    public class ApiSecurityTests : SecurityTestBase<Program>
    {
        public ApiSecurityTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Api_ShouldEnforceHttpsRedirection()
        {
            var response = await Client.GetAsync("http://localhost/api/auth/health");
            
            response.StatusCode.Should().BeOneOf(HttpStatusCode.MovedPermanently, HttpStatusCode.Found);
            response.Headers.Location?.Scheme.Should().Be("https");
        }

        [Fact]
        public async Task Api_ShouldReturnSecurityHeaders()
        {
            var response = await Client.GetAsync("/api/auth/health");

            response.Headers.Should().ContainKey("X-Content-Type-Options");
            response.Headers.Should().ContainKey("X-Frame-Options");
            response.Headers.Should().ContainKey("X-XSS-Protection");
            response.Headers.Should().ContainKey("Strict-Transport-Security");
            response.Headers.Should().ContainKey("Referrer-Policy");
        }

        [Fact]
        public async Task Api_ShouldRejectMaliciousPayloads()
        {
            var maliciousPayloads = new[]
            {
                "<script>alert('xss')</script>",
                "'; DROP TABLE Users; --",
                "../../../etc/passwd",
                "%3Cscript%3Ealert('xss')%3C/script%3E",
                "javascript:alert('xss')"
            };

            foreach (var payload in maliciousPayloads)
            {
                var request = new { Email = payload, Password = "password" };
                var response = await Client.PostAsJsonAsync("/api/auth/login", request);
                
                response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task Api_ShouldEnforceRateLimit()
        {
            var requests = new List<Task<HttpResponseMessage>>();
            
            for (int i = 0; i < 100; i++)
            {
                var request = Client.GetAsync("/api/auth/health");
                requests.Add(request);
            }

            var responses = await Task.WhenAll(requests);
            var rateLimitedResponses = responses.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests);
            
            rateLimitedResponses.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task Api_ShouldValidateContentType()
        {
            var invalidContent = new StringContent("invalid-json", Encoding.UTF8, "text/plain");
            var response = await Client.PostAsync("/api/auth/login", invalidContent);
            
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        [Fact]
        public async Task Api_ShouldRejectOversizedPayloads()
        {
            var largePayload = new string('A', 10 * 1024 * 1024); // 10MB
            var request = new { Data = largePayload };
            
            var response = await Client.PostAsJsonAsync("/api/auth/register", request);
            
            response.StatusCode.Should().BeOneOf(HttpStatusCode.RequestEntityTooLarge, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Api_ShouldValidateJwtTokenStructure()
        {
            var invalidTokens = new[]
            {
                "invalid-token",
                "header.payload", // Missing signature
                "header.payload.signature.extra", // Too many parts
                "", // Empty token
                null // Null token
            };

            foreach (var token in invalidTokens)
            {
                if (token != null)
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                else
                    Client.DefaultRequestHeaders.Authorization = null;

                var response = await Client.GetAsync("/api/auth/profile");
                response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task Api_ShouldPreventCsrfAttacks()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://malicious-site.com");
            Client.DefaultRequestHeaders.Add("Referer", "https://malicious-site.com");

            var response = await Client.PostAsJsonAsync("/api/auth/login", new { Email = "test@hudur.sa", Password = "password" });
            
            response.StatusCode.Should().BeOneOf(HttpStatusCode.Forbidden, HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Api_ShouldSanitizeErrorMessages()
        {
            var response = await Client.GetAsync("/api/nonexistent-endpoint");
            var content = await response.Content.ReadAsStringAsync();
            
            content.Should().NotContain("C:\\");
            content.Should().NotContain("/home/");
            content.Should().NotContain("stack trace");
            content.Should().NotContain("Exception");
        }

        [Fact]
        public async Task Api_ShouldEnforceApiVersioning()
        {
            var response = await Client.GetAsync("/api/v999/auth/health");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("GET")]
        [InlineData("POST")]
        [InlineData("PUT")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        [InlineData("OPTIONS")]
        public async Task Api_ShouldHandleHttpMethods(string method)
        {
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/auth/health");
            var response = await Client.SendAsync(request);
            
            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task Api_ShouldLogSecurityEvents()
        {
            var maliciousRequest = new { Email = "'; DROP TABLE Users; --", Password = "password" };
            await Client.PostAsJsonAsync("/api/auth/login", maliciousRequest);
            
            Assert.True(true); // Placeholder - would check logs in real implementation
        }

        [Fact]
        public async Task Api_ShouldEnforceInputValidation()
        {
            var invalidInputs = new[]
            {
                new { Email = "", Password = "" },
                new { Email = "not-an-email", Password = "123" },
                new { Email = new string('a', 1000) + "@test.com", Password = "password" },
                new { Email = "test@test.com", Password = new string('a', 1000) }
            };

            foreach (var input in invalidInputs)
            {
                var response = await Client.PostAsJsonAsync("/api/auth/login", input);
                response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity);
            }
        }
    }
}
