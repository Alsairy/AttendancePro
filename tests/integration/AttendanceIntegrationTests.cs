using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Tests.Integration
{
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
        public async Task CheckIn_WithValidData_ShouldReturnSuccess()
        {
            var checkInRequest = new CheckInRequest
            {
                Location = new LocationDto
                {
                    Latitude = 24.7136,
                    Longitude = 46.6753
                },
                DeviceInfo = new DeviceInfoDto
                {
                    DeviceId = "test-device",
                    Platform = "Test"
                }
            };

            var json = JsonSerializer.Serialize(checkInRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/attendance/checkin", content);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetAttendanceHistory_ShouldReturnPaginatedResults()
        {
            var response = await _client.GetAsync("/api/attendance/history?page=1&pageSize=10");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Health_ShouldReturnHealthy()
        {
            var response = await _client.GetAsync("/health");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("healthy");
        }
    }
}
