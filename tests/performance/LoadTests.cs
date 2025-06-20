using NBomber.Contracts;
using NBomber.CSharp;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AttendancePlatform.Tests.Performance
{
    public class LoadTests
    {
        private const string BaseUrl = "http://localhost:5000";
        private const int TestDurationMinutes = 2;
        private const int MaxConcurrentUsers = 50;

        [Fact]
        public void AuthenticationLoad_ShouldHandleConcurrentLogins()
        {
            var scenario = Scenario.Create("authentication_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var loginRequest = new
                {
                    Email = $"loadtest{context.ScenarioInfo.ThreadId}@test.com",
                    Password = "LoadTest123!",
                    TenantSubdomain = "loadtest"
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync($"{BaseUrl}/api/auth/login", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return Response.Ok(responseContent);
                    }
                    else
                    {
                        return Response.Fail($"HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex.Message);
                }
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 10, during: TimeSpan.FromMinutes(TestDurationMinutes))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.AllScenarioStats.First();
            Assert.True(scnStats.Ok.Request.Mean < 1000, "Average response time should be less than 1 second");
            Assert.True(scnStats.AllFailCount < scnStats.AllOkCount * 0.01, "Error rate should be less than 1%");
        }

        [Fact]
        public void AttendanceLoad_ShouldHandleConcurrentCheckIns()
        {
            var scenario = Scenario.Create("attendance_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var authToken = await GetAuthToken(httpClient, context.ScenarioInfo.ThreadId);
                if (string.IsNullOrEmpty(authToken))
                {
                    return Response.Fail("Authentication failed");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                var checkInRequest = new
                {
                    UserId = Guid.NewGuid(),
                    CheckInTime = DateTime.UtcNow,
                    Location = new { Latitude = 40.7128, Longitude = -74.0060 },
                    Method = "GPS"
                };

                var json = JsonSerializer.Serialize(checkInRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    var response = await httpClient.PostAsync($"{BaseUrl}/api/attendance/checkin", content);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return Response.Ok(responseContent);
                    }
                    else
                    {
                        return Response.Fail($"HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex.Message);
                }
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 15, during: TimeSpan.FromMinutes(TestDurationMinutes))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.AllScenarioStats.First();
            Assert.True(scnStats.Ok.Request.Mean < 2000, "Average response time should be less than 2 seconds");
            Assert.True(scnStats.AllFailCount < scnStats.AllOkCount * 0.05, "Error rate should be less than 5%");
        }

        [Fact]
        public void DatabaseLoad_ShouldHandleConcurrentQueries()
        {
            var scenario = Scenario.Create("database_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var authToken = await GetAuthToken(httpClient, context.ScenarioInfo.ThreadId);
                if (string.IsNullOrEmpty(authToken))
                {
                    return Response.Fail("Authentication failed");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                try
                {
                    var endpoints = new[]
                    {
                        "/api/attendance/history",
                        "/api/analytics/dashboard",
                        "/api/users/profile",
                        "/api/leave/requests"
                    };

                    var endpoint = endpoints[context.ScenarioInfo.ThreadId % endpoints.Length];
                    var response = await httpClient.GetAsync($"{BaseUrl}{endpoint}");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        return Response.Ok(responseContent);
                    }
                    else
                    {
                        return Response.Fail($"HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex.Message);
                }
            })
            .WithLoadSimulations(
                Simulation.KeepConstant(copies: MaxConcurrentUsers, during: TimeSpan.FromMinutes(TestDurationMinutes))
            );

            var stats = NBomberRunner
                .RegisterScenarios(scenario)
                .Run();

            var scnStats = stats.AllScenarioStats.First();
            Assert.True(scnStats.Ok.Request.Mean < 500, "Average response time should be less than 500ms");
            Assert.True(scnStats.Ok.Request.P95 < 1000, "95th percentile should be less than 1 second");
            Assert.True(scnStats.AllFailCount < scnStats.AllOkCount * 0.02, "Error rate should be less than 2%");
        }

        [Fact]
        public void CacheLoad_ShouldImprovePerformanceUnderLoad()
        {
            var scenarioWithoutCache = Scenario.Create("no_cache_load", async context =>
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("X-Skip-Cache", "true");
                
                var authToken = await GetAuthToken(httpClient, context.ScenarioInfo.ThreadId);
                if (string.IsNullOrEmpty(authToken))
                {
                    return Response.Fail("Authentication failed");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                try
                {
                    var response = await httpClient.GetAsync($"{BaseUrl}/api/analytics/dashboard");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail($"HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex.Message);
                }
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromMinutes(1))
            );

            var scenarioWithCache = Scenario.Create("with_cache_load", async context =>
            {
                using var httpClient = new HttpClient();
                
                var authToken = await GetAuthToken(httpClient, context.ScenarioInfo.ThreadId);
                if (string.IsNullOrEmpty(authToken))
                {
                    return Response.Fail("Authentication failed");
                }

                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                try
                {
                    var response = await httpClient.GetAsync($"{BaseUrl}/api/analytics/dashboard");
                    
                    if (response.IsSuccessStatusCode)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail($"HTTP {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    return Response.Fail(ex.Message);
                }
            })
            .WithLoadSimulations(
                Simulation.InjectPerSec(rate: 20, during: TimeSpan.FromMinutes(1))
            );

            var noCacheStats = NBomberRunner
                .RegisterScenarios(scenarioWithoutCache)
                .Run();

            var withCacheStats = NBomberRunner
                .RegisterScenarios(scenarioWithCache)
                .Run();

            var noCacheScnStats = noCacheStats.AllScenarioStats.First();
            var withCacheScnStats = withCacheStats.AllScenarioStats.First();

            Assert.True(withCacheScnStats.Ok.Request.Mean < noCacheScnStats.Ok.Request.Mean, 
                "Cache should improve average response time");
        }

        private async Task<string> GetAuthToken(HttpClient httpClient, int threadId)
        {
            var loginRequest = new
            {
                Email = $"loadtest{threadId}@test.com",
                Password = "LoadTest123!",
                TenantSubdomain = "loadtest"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await httpClient.PostAsync($"{BaseUrl}/api/auth/login", content);
                
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
    }
}
