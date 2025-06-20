using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Authentication.Api;
using AttendancePlatform.Shared.Domain.Entities;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Net;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace AttendancePlatform.Tests.Integration
{
    public class IntegrationTestFixture : IDisposable
    {
        public WebApplicationFactory<Program> Factory { get; private set; }
        public HttpClient Client { get; private set; }
        public AttendancePlatformDbContext Context { get; private set; }
        public string AuthToken { get; private set; } = string.Empty;
        public Guid TestUserId { get; private set; }
        public Guid TestTenantId { get; private set; }

        public IntegrationTestFixture()
        {
            Factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AttendancePlatformDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<AttendancePlatformDbContext>(options =>
                            options.UseInMemoryDatabase("IntegrationTestDb"));
                    });
                });

            Client = Factory.CreateClient();
            
            using var scope = Factory.Services.CreateScope();
            Context = scope.ServiceProvider.GetRequiredService<AttendancePlatformDbContext>();
            
            SeedTestData();
            SetupAuthentication().Wait();
        }

        private void SeedTestData()
        {
            TestTenantId = Guid.NewGuid();
            TestUserId = Guid.NewGuid();

            var tenant = new Tenant
            {
                Id = TestTenantId,
                Name = "Integration Test Tenant",
                Subdomain = "integration-test",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user = new User
            {
                Id = TestUserId,
                Email = "integration@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("IntegrationTest123!"),
                FirstName = "Integration",
                LastName = "Test",
                IsActive = true,
                TenantId = TestTenantId,
                CreatedAt = DateTime.UtcNow
            };

            Context.Tenants.Add(tenant);
            Context.Users.Add(user);
            Context.SaveChanges();
        }

        private async Task SetupAuthentication()
        {
            var loginRequest = new
            {
                Email = "integration@test.com",
                Password = "IntegrationTest123!",
                TenantSubdomain = "integration-test"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await Client.PostAsync("/api/auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (result.TryGetProperty("token", out var tokenElement))
                    {
                        AuthToken = tokenElement.GetString() ?? string.Empty;
                        Client.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", AuthToken);
                    }
                }
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Client.Dispose();
            Factory.Dispose();
        }
    }

    public class AuthenticationIntegrationTests : IClassFixture<IntegrationTestFixture>
    {
        private readonly IntegrationTestFixture _fixture;
        private readonly HttpClient _client;

        public AuthenticationIntegrationTests(IntegrationTestFixture fixture)
        {
            _fixture = fixture;
            _client = _fixture.Client;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new
            {
                Email = "admin@test.com",
                Password = "Test123!",
                TenantSubdomain = "test"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<dynamic>(responseContent);
            
            Assert.NotNull(result);
            // Additional assertions for token validation
        }

        [Fact]
        public async Task Register_WithValidData_CreatesUser()
        {
            // Arrange
            var registerRequest = new
            {
                Email = "newuser@test.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User",
                TenantSubdomain = "test"
            };

            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            
            Assert.Contains("User registered successfully", responseContent);
        }

        [Fact]
        public async Task RefreshToken_WithValidToken_ReturnsNewToken()
        {
            // Arrange
            var refreshRequest = new
            {
                RefreshToken = "valid-refresh-token"
            };

            var json = JsonSerializer.Serialize(refreshRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/refresh", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }

    public class AttendanceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AttendanceIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CheckIn_WithValidData_CreatesAttendanceRecord()
        {
            // Arrange
            var checkInRequest = new
            {
                UserId = Guid.NewGuid(),
                CheckInTime = DateTime.UtcNow,
                Location = new { Latitude = 40.7128, Longitude = -74.0060 },
                Method = "GPS"
            };

            var json = JsonSerializer.Serialize(checkInRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add authorization header
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/attendance/checkin", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task CheckOut_WithValidData_UpdatesAttendanceRecord()
        {
            // Arrange
            var checkOutRequest = new
            {
                UserId = Guid.NewGuid(),
                CheckOutTime = DateTime.UtcNow,
                Location = new { Latitude = 40.7128, Longitude = -74.0060 }
            };

            var json = JsonSerializer.Serialize(checkOutRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/attendance/checkout", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetAttendanceHistory_WithValidUser_ReturnsRecords()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.GetAsync($"/api/attendance/history/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
        }
    }

    public class FaceRecognitionIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public FaceRecognitionIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task EnrollFace_WithValidImage_CreatesTemplate()
        {
            // Arrange
            var enrollRequest = new
            {
                UserId = Guid.NewGuid(),
                ImageData = "base64-encoded-image-data"
            };

            var json = JsonSerializer.Serialize(enrollRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/face-recognition/enroll", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task VerifyFace_WithValidImage_ReturnsMatch()
        {
            // Arrange
            var verifyRequest = new
            {
                ImageData = "base64-encoded-image-data"
            };

            var json = JsonSerializer.Serialize(verifyRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/face-recognition/verify", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }

    public class LeaveManagementIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public LeaveManagementIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task SubmitLeaveRequest_WithValidData_CreatesRequest()
        {
            // Arrange
            var leaveRequest = new
            {
                UserId = Guid.NewGuid(),
                LeaveType = "Annual",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Family vacation"
            };

            var json = JsonSerializer.Serialize(leaveRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/leave/submit", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task ApproveLeaveRequest_WithValidId_UpdatesStatus()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var approvalRequest = new
            {
                RequestId = requestId,
                ApprovalStatus = "Approved",
                Comments = "Approved by manager"
            };

            var json = JsonSerializer.Serialize(approvalRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/leave/approve", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }

    public class NotificationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public NotificationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task SendEmail_WithValidData_SendsSuccessfully()
        {
            // Arrange
            var emailRequest = new
            {
                To = "test@example.com",
                Subject = "Test Email",
                Body = "This is a test email",
                IsHtml = false
            };

            var json = JsonSerializer.Serialize(emailRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/notifications/email", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SendPushNotification_WithValidData_SendsSuccessfully()
        {
            // Arrange
            var pushRequest = new
            {
                UserId = Guid.NewGuid(),
                Title = "Test Notification",
                Body = "This is a test push notification",
                Data = new { type = "attendance", action = "reminder" }
            };

            var json = JsonSerializer.Serialize(pushRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "valid-jwt-token");

            // Act
            var response = await _client.PostAsync("/api/notifications/push", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}

