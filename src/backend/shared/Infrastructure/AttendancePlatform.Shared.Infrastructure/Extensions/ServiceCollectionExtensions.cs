using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Infrastructure.Repositories;

namespace AttendancePlatform.Shared.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<AttendancePlatformDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AttendancePlatformDbContext).Assembly.FullName)));

            // Add repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ITenantRepository<>), typeof(TenantRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add services
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Add HTTP context accessor
            // services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddMultiTenancy(this IServiceCollection services)
        {
            services.AddScoped<ITenantContext, TenantContext>();
            return services;
        }

        public static async Task<IServiceProvider> MigrateDatabaseAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AttendancePlatformDbContext>();
            
            try
            {
                await context.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Database migration failed: {ex.Message}");
                throw;
            }

            return serviceProvider;
        }
    }
}

