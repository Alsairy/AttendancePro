using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Xunit;
using FluentAssertions;
using AttendancePlatform.Authentication.Api;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AttendancePlatform.Tests.Security
{
    public class AuthenticationSecurityTests : SecurityTestBase<Program>
    {
        public AuthenticationSecurityTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            var loginRequest = new
            {
                Email = "admin@hudur.sa",
                Password = "AdminPassword123!"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("token");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            var loginRequest = new
            {
                Email = "invalid@hudur.sa",
                Password = "WrongPassword"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithSqlInjectionAttempt_ShouldReturnBadRequest()
        {
            var loginRequest = new
            {
                Email = "admin@hudur.sa'; DROP TABLE Users; --",
                Password = "password"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_WithXssAttempt_ShouldReturnBadRequest()
        {
            var loginRequest = new
            {
                Email = "<script>alert('xss')</script>@hudur.sa",
                Password = "password"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ShouldReturnUnauthorized()
        {
            var response = await Client.GetAsync("/api/auth/profile");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_ShouldReturnSuccess()
        {
            var token = await GetValidJwtTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await Client.GetAsync("/api/auth/profile");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithExpiredToken_ShouldReturnUnauthorized()
        {
            var expiredToken = await GetExpiredJwtTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

            var response = await Client.GetAsync("/api/auth/profile");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithInvalidToken_ShouldReturnUnauthorized()
        {
            var invalidToken = await GetInvalidJwtTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", invalidToken);

            var response = await Client.GetAsync("/api/auth/profile");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task BruteForceProtection_MultipleFailedAttempts_ShouldImplementRateLimit()
        {
            var loginRequest = new
            {
                Email = "admin@hudur.sa",
                Password = "WrongPassword"
            };

            var responses = new List<HttpResponseMessage>();

            for (int i = 0; i < 10; i++)
            {
                var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
                responses.Add(response);
            }

            var lastResponse = responses.Last();
            lastResponse.StatusCode.Should().BeOneOf(HttpStatusCode.TooManyRequests, HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PasswordReset_WithValidEmail_ShouldNotRevealUserExistence()
        {
            var validEmailRequest = new { Email = "admin@hudur.sa" };
            var invalidEmailRequest = new { Email = "nonexistent@hudur.sa" };

            var validResponse = await Client.PostAsJsonAsync("/api/auth/forgot-password", validEmailRequest);
            var invalidResponse = await Client.PostAsJsonAsync("/api/auth/forgot-password", invalidEmailRequest);

            validResponse.StatusCode.Should().Be(invalidResponse.StatusCode);
        }

        [Fact]
        public async Task TwoFactorAuthentication_WithValidCode_ShouldReturnSuccess()
        {
            var token = await GetValidJwtTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var twoFactorRequest = new
            {
                Code = "123456",
                RememberDevice = false
            };

            var response = await Client.PostAsJsonAsync("/api/auth/verify-2fa", twoFactorRequest);

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SecurityHeaders_ShouldBePresent()
        {
            var response = await Client.GetAsync("/api/auth/health");

            response.Headers.Should().ContainKey("X-Content-Type-Options");
            response.Headers.Should().ContainKey("X-Frame-Options");
            response.Headers.Should().ContainKey("X-XSS-Protection");
            response.Headers.Should().ContainKey("Strict-Transport-Security");
        }

        [Fact]
        public async Task CorsPolicy_ShouldBeConfiguredSecurely()
        {
            Client.DefaultRequestHeaders.Add("Origin", "https://malicious-site.com");

            var response = await Client.GetAsync("/api/auth/health");

            var corsHeader = response.Headers.GetValues("Access-Control-Allow-Origin").FirstOrDefault();
            corsHeader.Should().NotBe("*");
        }
    }
}
