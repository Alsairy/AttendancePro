using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Net.Http;
using Xunit;

namespace AttendancePlatform.Tests.Security
{
    public abstract class SecurityTestBase<TStartup> : IClassFixture<WebApplicationFactory<TStartup>>
        where TStartup : class
    {
        protected readonly WebApplicationFactory<TStartup> Factory;
        protected readonly HttpClient Client;

        protected SecurityTestBase(WebApplicationFactory<TStartup> factory)
        {
            Factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AttendancePlatformDbContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<AttendancePlatformDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("SecurityTestDb");
                    });
                });
            });

            Client = Factory.CreateClient();
        }

        protected async Task<string> GetValidJwtTokenAsync()
        {
            var loginRequest = new
            {
                Email = "test@hudur.sa",
                Password = "TestPassword123!"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
            return result.token;
        }

        protected async Task<string> GetExpiredJwtTokenAsync()
        {
            return "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyLCJleHAiOjE1MTYyMzkwMjJ9.invalid";
        }

        protected async Task<string> GetInvalidJwtTokenAsync()
        {
            return "invalid.jwt.token";
        }
    }
}
