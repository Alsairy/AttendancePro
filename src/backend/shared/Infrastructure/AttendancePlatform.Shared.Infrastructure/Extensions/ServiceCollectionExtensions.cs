using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Infrastructure.Repositories;
using AttendancePlatform.Shared.Infrastructure.Security;
using AttendancePlatform.Shared.Infrastructure.Middleware;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace AttendancePlatform.Shared.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext with optimized configuration
            services.AddDbContext<AttendancePlatformDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => {
                        b.MigrationsAssembly(typeof(AttendancePlatformDbContext).Assembly.FullName);
                        b.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null);
                        b.CommandTimeout(30);
                    }), ServiceLifetime.Scoped);

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

            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Add advanced services
            services.AddScoped<IAdvancedAnalyticsService, AdvancedAnalyticsService>();
            services.AddScoped<IAdvancedBiometricService, AdvancedBiometricService>();
            services.AddScoped<IWorkflowAutomationService, WorkflowAutomationService>();
            services.AddScoped<INotificationService, NotificationService>();

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

        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            var encryptionKey = configuration["Security:EncryptionKey"] ?? 
                               configuration["ENCRYPTION_KEY"] ?? 
                               throw new InvalidOperationException("Encryption key not configured");
            
            services.AddSingleton<IEncryptionService>(provider => new EncryptionService(encryptionKey));
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IComplianceReportingService, ComplianceReportingService>();
            
            services.Configure<RateLimitOptions>(options =>
            {
                options.MaxRequests = configuration.GetValue<int>("RATE_LIMIT_REQUESTS_PER_MINUTE", 100);
                options.WindowSizeInMinutes = 1;
            });

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    var origins = configuration["CORS_ORIGINS"]?.Split(',') ?? new[] { "https://localhost:3000" };
                    builder.WithOrigins(origins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddWorkflowServices(this IServiceCollection services)
        {
            services.AddScoped<IShiftSchedulingService, ShiftSchedulingService>();
            services.AddScoped<IAdvancedWorkflowService, AdvancedWorkflowService>();
            services.AddScoped<IWorkflowExecutionEngine, WorkflowExecutionEngine>();
            services.AddScoped<IWorkflowTemplateService, WorkflowTemplateService>();
            services.AddScoped<IWorkflowExecutionService, WorkflowExecutionService>();
            services.AddScoped<IAutomationService, AutomationService>();
            
            return services;
        }

        public static IServiceCollection AddVoiceRecognitionServices(this IServiceCollection services)
        {
            services.AddScoped<IVoiceRecognitionService, VoiceRecognitionService>();
            services.AddScoped<IVoiceCommandProcessor, VoiceCommandProcessor>();
            services.AddScoped<IVoiceAuthenticationService, VoiceAuthenticationService>();
            
            return services;
        }

        public static IServiceCollection AddComplianceServices(this IServiceCollection services)
        {
            services.AddScoped<IComplianceService, ComplianceService>();
            services.AddScoped<IComplianceAuditService, ComplianceAuditService>();
            services.AddScoped<IComplianceReportingService, ComplianceReportingService>();
            
            return services;
        }

        public static IServiceCollection AddKioskServices(this IServiceCollection services)
        {
            services.AddScoped<IKioskService, KioskService>();
            services.AddScoped<IKioskAuthenticationService, KioskAuthenticationService>();
            
            return services;
        }
    }
}

