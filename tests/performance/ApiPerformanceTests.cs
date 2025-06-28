using NBomber.CSharp;
using NBomber.Http.CSharp;
using System.Net.Http;
using Xunit;
using FluentAssertions;

namespace AttendancePlatform.Tests.Performance
{
    public class ApiPerformanceTests
    {
        private const string BaseUrl = "https://localhost:7001";

        [Fact]
        public async Task AuthenticationApi_ShouldHandleHighLoad()
        {
            var scenario = Scenario.Create("auth_load_test", async context =>
            {
                var loginRequest = new
                {
                    Email = "test@hudur.sa",
                    Password = "TestPassword123!"
                };

                var response = await Http.CreateRequest("POST", $"{BaseUrl}/api/auth/login")
                    .WithJsonBody(loginRequest)
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromMinutes(2))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var authStats = stats.AllOkCount;
            var errorRate = (double)stats.AllFailCount / (stats.AllOkCount + stats.AllFailCount);

            authStats.Should().BeGreaterThan(0);
            errorRate.Should().BeLessThan(0.01); // Less than 1% error rate
        }

        [Fact]
        public async Task AttendanceApi_ShouldMaintainPerformanceUnderLoad()
        {
            var scenario = Scenario.Create("attendance_load_test", async context =>
            {
                var attendanceRequest = new
                {
                    UserId = Guid.NewGuid(),
                    CheckInTime = DateTime.UtcNow,
                    Location = new { Latitude = 24.7136, Longitude = 46.6753 }
                };

                var response = await Http.CreateRequest("POST", $"{BaseUrl}/api/attendance/checkin")
                    .WithJsonBody(attendanceRequest)
                    .WithHeader("Authorization", "Bearer test-token")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 50, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var avgResponseTime = stats.ScenarioStats[0].Ok.Response.Mean;
            avgResponseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(500));
        }

        [Fact]
        public async Task DatabaseQueries_ShouldPerformWithinLimits()
        {
            var scenario = Scenario.Create("database_query_test", async context =>
            {
                var response = await Http.CreateRequest("GET", $"{BaseUrl}/api/attendance/reports/monthly")
                    .WithHeader("Authorization", "Bearer test-token")
                    .WithQuery("month", DateTime.UtcNow.Month.ToString())
                    .WithQuery("year", DateTime.UtcNow.Year.ToString())
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var avgResponseTime = stats.ScenarioStats[0].Ok.Response.Mean;
            avgResponseTime.Should().BeLessThan(TimeSpan.FromSeconds(2));
        }

        [Fact]
        public async Task ConcurrentUsers_ShouldBeHandledEfficiently()
        {
            var scenario = Scenario.Create("concurrent_users_test", async context =>
            {
                var endpoints = new[]
                {
                    "/api/auth/profile",
                    "/api/attendance/status",
                    "/api/leave/balance",
                    "/api/notifications/unread"
                };

                var randomEndpoint = endpoints[Random.Shared.Next(endpoints.Length)];
                
                var response = await Http.CreateRequest("GET", $"{BaseUrl}{randomEndpoint}")
                    .WithHeader("Authorization", "Bearer test-token")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 100, during: TimeSpan.FromMinutes(2))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var successRate = (double)stats.AllOkCount / (stats.AllOkCount + stats.AllFailCount);
            successRate.Should().BeGreaterThan(0.95); // 95% success rate
        }

        [Fact]
        public async Task MemoryUsage_ShouldRemainStable()
        {
            var scenario = Scenario.Create("memory_stability_test", async context =>
            {
                var largeDataRequest = new
                {
                    StartDate = DateTime.UtcNow.AddMonths(-12),
                    EndDate = DateTime.UtcNow,
                    IncludeDetails = true,
                    GroupBy = "Department"
                };

                var response = await Http.CreateRequest("POST", $"{BaseUrl}/api/analytics/attendance-report")
                    .WithJsonBody(largeDataRequest)
                    .WithHeader("Authorization", "Bearer test-token")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(3))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            stats.AllOkCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task ApiGateway_ShouldRouteEfficiently()
        {
            var scenario = Scenario.Create("gateway_routing_test", async context =>
            {
                var services = new[]
                {
                    "/api/auth/health",
                    "/api/attendance/health",
                    "/api/leave/health",
                    "/api/notifications/health",
                    "/api/analytics/health"
                };

                var randomService = services[Random.Shared.Next(services.Length)];
                
                var response = await Http.CreateRequest("GET", $"{BaseUrl}{randomService}")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 200, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var avgResponseTime = stats.ScenarioStats[0].Ok.Response.Mean;
            avgResponseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public async Task RealTimeFeatures_ShouldMaintainLowLatency()
        {
            var scenario = Scenario.Create("realtime_test", async context =>
            {
                var notificationRequest = new
                {
                    Type = "AttendanceAlert",
                    UserId = Guid.NewGuid(),
                    Message = "Test notification"
                };

                var response = await Http.CreateRequest("POST", $"{BaseUrl}/api/notifications/send")
                    .WithJsonBody(notificationRequest)
                    .WithHeader("Authorization", "Bearer test-token")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 30, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var avgResponseTime = stats.ScenarioStats[0].Ok.Response.Mean;
            avgResponseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(200));
        }

        [Fact]
        public async Task FileUpload_ShouldHandleMultipleUploads()
        {
            var scenario = Scenario.Create("file_upload_test", async context =>
            {
                var fileContent = new byte[1024 * 1024]; // 1MB file
                Random.Shared.NextBytes(fileContent);

                var response = await Http.CreateRequest("POST", $"{BaseUrl}/api/face-recognition/upload")
                    .WithMultipartFormData(form => form.AddBytes("file", fileContent, "test.jpg"))
                    .WithHeader("Authorization", "Bearer test-token")
                    .SendAsync(context);

                return response;
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 5, during: TimeSpan.FromMinutes(1))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var avgResponseTime = stats.ScenarioStats[0].Ok.Response.Mean;
            avgResponseTime.Should().BeLessThan(TimeSpan.FromSeconds(5));
        }
    }
}
