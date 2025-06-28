using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Shared.Infrastructure.Data;
using Testcontainers.SqlServer;
using Xunit;
using Respawn;

namespace AttendancePlatform.Tests.Database
{
    public abstract class DatabaseTestBase : IAsyncLifetime
    {
        private readonly SqlServerContainer _sqlServerContainer;
        protected AttendancePlatformDbContext DbContext { get; private set; }
        private Respawner _respawner;

        protected DatabaseTestBase()
        {
            _sqlServerContainer = new SqlServerBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("TestPassword123!")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _sqlServerContainer.StartAsync();

            var connectionString = _sqlServerContainer.GetConnectionString();

            var services = new ServiceCollection();
            services.AddDbContext<AttendancePlatformDbContext>(options =>
                options.UseSqlServer(connectionString));

            var serviceProvider = services.BuildServiceProvider();
            DbContext = serviceProvider.GetRequiredService<AttendancePlatformDbContext>();

            await DbContext.Database.EnsureCreatedAsync();

            _respawner = await Respawner.CreateAsync(connectionString, new RespawnerOptions
            {
                TablesToIgnore = new[] { "__EFMigrationsHistory" }
            });
        }

        public async Task DisposeAsync()
        {
            await DbContext.DisposeAsync();
            await _sqlServerContainer.DisposeAsync();
        }

        protected async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_sqlServerContainer.GetConnectionString());
        }

        protected async Task SeedTestDataAsync()
        {
            var tenant = new AttendancePlatform.Shared.Domain.Entities.Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Test Tenant",
                Domain = "test.hudur.sa",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.Tenants.Add(tenant);

            var user = new AttendancePlatform.Shared.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                TenantId = tenant.Id,
                Email = "test@hudur.sa",
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
        }
    }
}
