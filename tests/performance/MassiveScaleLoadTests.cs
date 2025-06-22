using NBomber.Contracts;
using NBomber.CSharp;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AttendancePlatform.Tests.Performance
{
    public class MassiveScaleLoadTests
    {
        private const string BaseUrl = "https://api.hudu.sa";
        private const string AuthBaseUrl = "https://auth.hudu.sa";
        private const string AttendanceBaseUrl = "https://attendance.hudu.sa";
        
        [Fact]
        public void Load_500K_ConcurrentUsers_AuthenticationFlow()
        {
            var authScenario = Scenario.Create("massive_auth_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var loginRequest = new
                {
                    Email = $"loadtest{context.InvocationNumber}@hudu.sa",
                    Password = "LoadTest123!",
                    TenantSubdomain = "enterprise"
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync($"{AuthBaseUrl}/api/Auth/login", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail($"Auth failed: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail($"Exception: {ex.Message}");
                }
            })
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 10000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(5)),
                Simulation.KeepConstant(copies: 500000, during: TimeSpan.FromMinutes(30)),
                Simulation.RampingInject(rate: -10000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(5))
            );

            var stats = NBomberRunner
                .RegisterScenarios(authScenario)
                .WithReportFolder("load_test_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
                .Run();

            Assert.True(stats.AllOkCount > 0, "Should have successful authentication requests");
            Assert.True(stats.AllFailCount < stats.AllOkCount * 0.05, "Error rate should be less than 5%");
            Assert.True(stats.ScenarioStats[0].Ok.Response.Mean < TimeSpan.FromSeconds(2), "Average response time should be under 2 seconds");
        }

        [Fact]
        public void Load_500K_ConcurrentUsers_AttendanceCheckIn()
        {
            var attendanceScenario = Scenario.Create("massive_attendance_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                if (string.IsNullOrEmpty(token))
                {
                    return Response.Fail("Failed to get auth token");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var checkInRequest = new
                {
                    UserId = $"user_{context.InvocationNumber}",
                    Latitude = 24.7136 + (Random.Shared.NextDouble() - 0.5) * 0.01,
                    Longitude = 46.6753 + (Random.Shared.NextDouble() - 0.5) * 0.01,
                    CheckInTime = DateTime.UtcNow,
                    DeviceId = $"device_{context.InvocationNumber}",
                    BiometricVerified = true
                };

                var json = JsonSerializer.Serialize(checkInRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync($"{AttendanceBaseUrl}/api/Attendance/checkin", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail($"Check-in failed: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail($"Exception: {ex.Message}");
                }
            })
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 8000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(10)),
                Simulation.KeepConstant(copies: 400000, during: TimeSpan.FromMinutes(45)),
                Simulation.RampingInject(rate: -8000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(10))
            );

            var stats = NBomberRunner
                .RegisterScenarios(attendanceScenario)
                .WithReportFolder("load_test_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
                .Run();

            Assert.True(stats.AllOkCount > 0, "Should have successful check-in requests");
            Assert.True(stats.AllFailCount < stats.AllOkCount * 0.03, "Error rate should be less than 3%");
            Assert.True(stats.ScenarioStats[0].Ok.Response.Mean < TimeSpan.FromSeconds(1.5), "Average response time should be under 1.5 seconds");
        }

        [Fact]
        public void Load_500K_ConcurrentUsers_FaceRecognition()
        {
            var faceRecognitionScenario = Scenario.Create("massive_face_recognition_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                if (string.IsNullOrEmpty(token))
                {
                    return Response.Fail("Failed to get auth token");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var faceData = Convert.ToBase64String(GenerateMockFaceData());
                var verificationRequest = new
                {
                    UserId = $"user_{context.InvocationNumber}",
                    FaceImageData = faceData,
                    ConfidenceThreshold = 0.85
                };

                var json = JsonSerializer.Serialize(verificationRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync($"{BaseUrl}/api/FaceRecognition/verify", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail($"Face verification failed: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail($"Exception: {ex.Message}");
                }
            })
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 5000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(8)),
                Simulation.KeepConstant(copies: 200000, during: TimeSpan.FromMinutes(20)),
                Simulation.RampingInject(rate: -5000, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromMinutes(8))
            );

            var stats = NBomberRunner
                .RegisterScenarios(faceRecognitionScenario)
                .WithReportFolder("load_test_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Csv)
                .Run();

            Assert.True(stats.AllOkCount > 0, "Should have successful face recognition requests");
            Assert.True(stats.AllFailCount < stats.AllOkCount * 0.08, "Error rate should be less than 8%");
            Assert.True(stats.ScenarioStats[0].Ok.Response.Mean < TimeSpan.FromSeconds(3), "Average response time should be under 3 seconds");
        }

        [Fact]
        public void Load_500K_ConcurrentUsers_MixedWorkload()
        {
            var authScenario = Scenario.Create("auth_scenario", async context =>
            {
                using var httpClient = new HttpClient();
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                return string.IsNullOrEmpty(token) ? Response.Fail() : Response.Ok();
            })
            .WithWeight(30)
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 150000, during: TimeSpan.FromMinutes(60))
            );

            var attendanceScenario = Scenario.Create("attendance_scenario", async context =>
            {
                using var httpClient = new HttpClient();
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                if (string.IsNullOrEmpty(token)) return Response.Fail();

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{AttendanceBaseUrl}/api/Attendance/status");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithWeight(40)
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 200000, during: TimeSpan.FromMinutes(60))
            );

            var analyticsScenario = Scenario.Create("analytics_scenario", async context =>
            {
                using var httpClient = new HttpClient();
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                if (string.IsNullOrEmpty(token)) return Response.Fail();

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{BaseUrl}/api/Analytics/dashboard");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithWeight(20)
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 100000, during: TimeSpan.FromMinutes(60))
            );

            var reportsScenario = Scenario.Create("reports_scenario", async context =>
            {
                using var httpClient = new HttpClient();
                var token = await GetAuthToken(httpClient, context.InvocationNumber);
                if (string.IsNullOrEmpty(token)) return Response.Fail();

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"{BaseUrl}/api/Reports/attendance-summary");
                return response.IsSuccessStatusCode ? Response.Ok() : Response.Fail();
            })
            .WithWeight(10)
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: 50000, during: TimeSpan.FromMinutes(60))
            );

            var stats = NBomberRunner
                .RegisterScenarios(authScenario, attendanceScenario, analyticsScenario, reportsScenario)
                .WithReportFolder("load_test_reports")
                .WithReportFormats(ReportFormat.Html, ReportFormat.Csv, ReportFormat.Md)
                .Run();

            Assert.True(stats.AllOkCount > 0, "Should have successful requests across all scenarios");
            Assert.True(stats.AllFailCount < stats.AllOkCount * 0.05, "Overall error rate should be less than 5%");
        }

        private async Task<string> GetAuthToken(HttpClient httpClient, int invocationNumber)
        {
            var loginRequest = new
            {
                Email = $"loadtest{invocationNumber % 10000}@hudu.sa",
                Password = "LoadTest123!",
                TenantSubdomain = "enterprise"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync($"{AuthBaseUrl}/api/Auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    if (result.TryGetProperty("token", out var tokenElement))
                    {
                        return tokenElement.GetString() ?? string.Empty;
                    }
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        private byte[] GenerateMockFaceData()
        {
            var random = new Random();
            var data = new byte[1024];
            random.NextBytes(data);
            return data;
        }
    }
}
