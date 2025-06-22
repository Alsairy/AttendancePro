using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Hudur.Shared.Domain.Interfaces;
using Hudur.Shared.Infrastructure.Services;

namespace Hudur.Shared.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<HudurDbContext>
    {
        public HudurDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<HudurDbContext>();
            
            var connectionString = "Server=host.docker.internal,1433;Database=Hudur;User Id=sa;Password=AttendanceP@ssw0rd123;TrustServerCertificate=true";
            
            optionsBuilder.UseSqlServer(connectionString, b => {
                b.MigrationsAssembly(typeof(HudurDbContext).Assembly.FullName);
                b.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                b.CommandTimeout(30);
            });

            var mockTenantContext = new MockTenantContext();
            var mockCurrentUserService = new MockCurrentUserService();
            var mockDateTimeProvider = new DateTimeProvider();

            return new HudurDbContext(optionsBuilder.Options, mockTenantContext, mockCurrentUserService, mockDateTimeProvider);
        }
    }

    public class MockTenantContext : ITenantContext
    {
        public Guid? TenantId => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public string? TenantSubdomain => "design-time";
        
        public void SetTenant(Guid tenantId, string subdomain)
        {
        }
    }

    public class MockCurrentUserService : ICurrentUserService
    {
        public Guid? UserId => Guid.Parse("00000000-0000-0000-0000-000000000001");
        public string? UserName => "Design Time User";
        public string? Email => "design@time.com";
        public IEnumerable<string> Roles => new[] { "Admin" };
        public bool IsAuthenticated => true;
        
        public bool HasPermission(string permission) => true;
        public bool HasRole(string role) => true;
    }
}
