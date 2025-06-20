using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Infrastructure.Repositories;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace AttendancePlatform.Shared.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with connection pooling optimization
            services.AddDbContext<AttendancePlatformDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => {
                        b.MigrationsAssembly(typeof(AttendancePlatformDbContext).Assembly.FullName);
                        b.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                        b.CommandTimeout(30);
                    }), ServiceLifetime.Scoped);

            services.AddDbContextPool<AttendancePlatformDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")), poolSize: 128);

            var redisConnectionString = configuration.GetConnectionString("Redis") ?? 
                                      configuration["Redis:ConnectionString"] ?? 
                                      "localhost:6379";
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "AttendancePlatform";
            });

            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1024; // Limit cache size
                options.CompactionPercentage = 0.25; // Compact when 25% over limit
            });

            services.AddScoped<ICacheService, CacheService>();

            // Add repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(ITenantRepository<>), typeof(TenantRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Add services
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Add HTTP context accessor
            services.AddHttpContextAccessor();

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

