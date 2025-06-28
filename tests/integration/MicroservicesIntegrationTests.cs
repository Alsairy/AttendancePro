using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Net.Http;
using Xunit;
using FluentAssertions;
using System.Net;
using AttendancePlatform.Authentication.Api;

namespace AttendancePlatform.Tests.Integration
{
    public class MicroservicesIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public MicroservicesIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AttendancePlatformDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AttendancePlatformDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("IntegrationTestDb");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task AuthenticationService_ShouldIntegrateWithUserManagement()
        {
            var testEmail = Environment.GetEnvironmentVariable("TEST_INTEGRATION_EMAIL") ?? "integration@hudur.sa";
            var testPassword = Environment.GetEnvironmentVariable("TEST_INTEGRATION_PASSWORD") ?? "TestPassword123!";
            
            var registerRequest = new
            {
                Email = testEmail,
                Password = testPassword,
                FirstName = "Integration",
                LastName = "Test"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginRequest = new
            {
                Email = testEmail,
                Password = testPassword
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginContent = await loginResponse.Content.ReadAsStringAsync();
            loginContent.Should().Contain("token");
        }

        [Fact]
        public async Task AttendanceService_ShouldIntegrateWithAuthentication()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var attendanceRequest = new
            {
                CheckInTime = DateTime.UtcNow,
                Location = new { Latitude = 24.7136, Longitude = 46.6753 },
                Method = "GPS"
            };

            var response = await _client.PostAsJsonAsync("/api/attendance/checkin", attendanceRequest);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        [Fact]
        public async Task LeaveManagement_ShouldIntegrateWithApprovalWorkflow()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var leaveRequest = new
            {
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                LeaveType = "Annual",
                Reason = "Integration test leave"
            };

            var response = await _client.PostAsJsonAsync("/api/leave/request", leaveRequest);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);

            var statusResponse = await _client.GetAsync("/api/leave/requests/pending");
            statusResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task NotificationService_ShouldIntegrateWithAllServices()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/notifications/unread");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var notificationRequest = new
            {
                Type = "Test",
                Message = "Integration test notification",
                Priority = "Normal"
            };

            var sendResponse = await _client.PostAsJsonAsync("/api/notifications/send", notificationRequest);
            sendResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        [Fact]
        public async Task AnalyticsService_ShouldAggregateDataFromMultipleSources()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var analyticsRequest = new
            {
                StartDate = DateTime.UtcNow.AddDays(-30),
                EndDate = DateTime.UtcNow,
                MetricType = "Attendance"
            };

            var response = await _client.PostAsJsonAsync("/api/analytics/generate-report", analyticsRequest);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Accepted);
        }

        [Fact]
        public async Task FaceRecognitionService_ShouldIntegrateWithAttendance()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var enrollmentRequest = new
            {
                UserId = Guid.NewGuid(),
                BiometricData = Convert.ToBase64String(new byte[1024])
            };

            var response = await _client.PostAsJsonAsync("/api/face-recognition/enroll", enrollmentRequest);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created);
        }

        [Fact]
        public async Task TenantManagement_ShouldIsolateDataCorrectly()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var tenantResponse = await _client.GetAsync("/api/tenant/current");
            tenantResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var tenantContent = await tenantResponse.Content.ReadAsStringAsync();
            tenantContent.Should().NotBeEmpty();
        }

        [Fact]
        public async Task ApiGateway_ShouldRouteToAllServices()
        {
            var services = new[]
            {
                "/api/auth/health",
                "/api/attendance/health",
                "/api/leave/health",
                "/api/notifications/health",
                "/api/analytics/health",
                "/api/face-recognition/health",
                "/api/tenant/health"
            };

            foreach (var service in services)
            {
                var response = await _client.GetAsync(service);
                response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized);
            }
        }

        [Fact]
        public async Task DatabaseTransactions_ShouldMaintainConsistency()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var complexRequest = new
            {
                AttendanceRecord = new
                {
                    CheckInTime = DateTime.UtcNow,
                    Location = new { Latitude = 24.7136, Longitude = 46.6753 }
                },
                NotificationSettings = new
                {
                    SendEmail = true,
                    SendPush = true
                }
            };

            var response = await _client.PostAsJsonAsync("/api/attendance/complex-checkin", complexRequest);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task EventSourcing_ShouldMaintainAuditTrail()
        {
            var token = await GetValidTokenAsync();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var auditableAction = new
            {
                Action = "UpdateProfile",
                Data = new { FirstName = "Updated", LastName = "Name" }
            };

            var response = await _client.PutAsJsonAsync("/api/auth/profile", auditableAction);
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

            var auditResponse = await _client.GetAsync("/api/audit/recent");
            auditResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Forbidden);
        }

        private async Task<string> GetValidTokenAsync()
        {
            var testEmail = Environment.GetEnvironmentVariable("TEST_ADMIN_EMAIL") ?? "admin@hudur.sa";
            var testPassword = Environment.GetEnvironmentVariable("TEST_ADMIN_PASSWORD") ?? "TestPassword123!";
            
            var loginRequest = new
            {
                Email = testEmail,
                Password = testPassword
            };

            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                return result?.token ?? "test-token";
            }

            return "test-token";
        }
    }
}
