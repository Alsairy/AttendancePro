using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Infrastructure.Hubs;
using AttendancePlatform.Shared.Infrastructure.Extensions;
using AttendancePlatform.Shared.Infrastructure.Middleware;
using AttendancePlatform.Shared.Infrastructure.Telemetry;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Infrastructure.Repositories;
using AttendancePlatform.Shared.Domain.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using StackExchange.Redis;
using AttendancePlatform.Api.Services;
using AttendancePlatform.Api.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HostFiltering;

[assembly: InternalsVisibleTo("AttendancePlatform.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Kestrel to accept any host header and use Fly.io port
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
    options.Limits.MaxRequestBodySize = null;
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    options.Listen(System.Net.IPAddress.Any, int.Parse(port));
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});

// Disable host filtering completely for deployment
builder.Services.Configure<HostFilteringOptions>(options =>
{
    options.AllowedHosts.Clear();
    options.AllowedHosts.Add("*");
    options.AllowEmptyHosts = true;
    options.IncludeFailureMessage = false;
});

// Add infrastructure services with in-memory database for deployment
builder.Services.AddDbContext<AttendancePlatformDbContext>(options =>
    options.UseInMemoryDatabase("AttendancePlatform"));

// Add infrastructure services without database context (to avoid conflict)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? 
                          builder.Configuration["Redis:ConnectionString"] ?? 
                          "localhost:6379";

builder.Services.AddMemoryCache(options =>
{
    options.SizeLimit = 1024;
    options.CompactionPercentage = 0.25;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(ITenantRepository<>), typeof(TenantRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSecurityServices(builder.Configuration);

builder.Services.AddScoped<IAuthenticationService, AttendancePlatform.Api.Services.AuthenticationService>();
builder.Services.AddScoped<IAttendanceService, AttendancePlatform.Api.Services.AttendanceService>();
builder.Services.AddScoped<ILeaveManagementService, AttendancePlatform.Api.Services.LeaveManagementService>();
builder.Services.AddScoped<IFaceRecognitionService, AttendancePlatform.Api.Services.FaceRecognitionService>();
builder.Services.AddScoped<IActiveDirectoryService, AttendancePlatform.Api.Services.ActiveDirectoryService>();
builder.Services.AddScoped<IUserManagementService, AttendancePlatform.Api.Services.UserManagementService>();
builder.Services.AddScoped<IBusinessIntelligenceService, AttendancePlatform.Api.Services.BusinessIntelligenceService>();
builder.Services.AddScoped<IWorkflowEngineService, AttendancePlatform.Api.Services.WorkflowEngineService>();
builder.Services.AddScoped<IVoiceRecognitionService, AttendancePlatform.Api.Services.VoiceRecognitionService>();
builder.Services.AddScoped<ICollaborationService, AttendancePlatform.Api.Services.CollaborationService>();
builder.Services.AddScoped<IComprehensiveReportingService, AttendancePlatform.Api.Services.ComprehensiveReportingService>();
builder.Services.AddScoped<AttendancePlatform.Api.Services.IAdvancedAnalyticsService, AttendancePlatform.Api.Services.AdvancedAnalyticsService>();
builder.Services.AddScoped<IEnterpriseIntegrationService, AttendancePlatform.Api.Services.EnterpriseIntegrationService>();
builder.Services.AddScoped<IGlobalComplianceService, AttendancePlatform.Api.Services.GlobalComplianceService>();

builder.Services.AddScoped<IJwtTokenService, AttendancePlatform.Api.Services.JwtTokenService>();
builder.Services.AddScoped<ITwoFactorService, AttendancePlatform.Api.Services.TwoFactorService>();
builder.Services.AddScoped<AttendancePlatform.Application.Services.IEmailService, AttendancePlatform.Api.Services.EmailService>();
builder.Services.AddScoped<IRefreshTokenService, AttendancePlatform.Api.Services.RefreshTokenService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();
builder.Services.AddScoped<IComplianceService, ComplianceService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IMonitoringService, MonitoringService>();

builder.Services.AddScoped<IMaintenanceManagementService, MaintenanceManagementService>();
builder.Services.AddScoped<IProcurementManagementService, ProcurementManagementService>();
builder.Services.AddScoped<IFacilityManagementService, FacilityManagementService>();
builder.Services.AddScoped<ILegalManagementService, LegalManagementService>();
builder.Services.AddScoped<IComprehensiveFinanceService, ComprehensiveFinanceService>();
builder.Services.AddScoped<IComprehensiveProcurementService, ComprehensiveProcurementService>();
builder.Services.AddScoped<IComprehensiveHRService, ComprehensiveHRService>();
builder.Services.AddScoped<ComprehensiveFinanceService>();
builder.Services.AddScoped<ComprehensiveProcurementService>();
builder.Services.AddScoped<ComprehensiveHRService>();
builder.Services.AddScoped<IComprehensiveDigitalTransformationService, ComprehensiveDigitalTransformationService>();
builder.Services.AddScoped<IComprehensiveInnovationService, ComprehensiveInnovationService>();
builder.Services.AddScoped<IComprehensiveCustomerServiceService, ComprehensiveCustomerServiceService>();
builder.Services.AddScoped<IComprehensiveMarketingService, ComprehensiveMarketingService>();
builder.Services.AddScoped<IComprehensiveSalesService, ComprehensiveSalesService>();
builder.Services.AddScoped<IComprehensiveProductManagementService, ComprehensiveProductManagementService>();
builder.Services.AddScoped<IComprehensiveAdvancedTechnologyService, ComprehensiveAdvancedTechnologyService>();
builder.Services.AddScoped<IComprehensiveDataScienceService, ComprehensiveDataScienceService>();
builder.Services.AddScoped<IComprehensiveCybersecurityService, ComprehensiveCybersecurityService>();
builder.Services.AddScoped<ITrainingManagementService, TrainingManagementService>();
builder.Services.AddScoped<IDocumentManagementService, DocumentManagementService>();
builder.Services.AddScoped<IAssetManagementService, AssetManagementService>();
builder.Services.AddScoped<IVendorManagementService, VendorManagementService>();
builder.Services.AddScoped<IInventoryManagementService, InventoryManagementService>();
builder.Services.AddScoped<IServiceManagementService, ServiceManagementService>();
builder.Services.AddScoped<IInnovationManagementService, InnovationManagementService>();
builder.Services.AddScoped<IEventManagementService, EventManagementService>();
builder.Services.AddScoped<IOperationsManagementService, OperationsManagementService>();
builder.Services.AddScoped<IDataAnalyticsService, DataAnalyticsService>();
builder.Services.AddScoped<IPayrollIntegrationService, PayrollIntegrationService>();
builder.Services.AddScoped<IGeofencingService, GeofencingService>();
builder.Services.AddScoped<IEnterpriseResourcePlanningService, EnterpriseResourcePlanningService>();
builder.Services.AddScoped<IAdvancedRealtimeAnalyticsService, AdvancedRealtimeAnalyticsService>();
builder.Services.AddScoped<ITalentManagementService, TalentManagementService>();
builder.Services.AddScoped<IStrategicPlanningService, StrategicPlanningService>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();
builder.Services.AddScoped<IProductManagementService, ProductManagementService>();
builder.Services.AddScoped<IAdvancedDataMiningService, AdvancedDataMiningService>();
builder.Services.AddScoped<ICyberSecurityService, CyberSecurityService>();
builder.Services.AddScoped<IRiskManagementService, RiskManagementService>();
builder.Services.AddScoped<IMarketingManagementService, MarketingManagementService>();
builder.Services.AddScoped<IBusinessContinuityService, BusinessContinuityService>();
builder.Services.AddScoped<IQuantumComputingService, QuantumComputingService>();
builder.Services.AddScoped<IDataGovernanceService, DataGovernanceService>();
builder.Services.AddScoped<IPerformanceManagementService, PerformanceManagementService>();
builder.Services.AddScoped<IAugmentedRealityService, AugmentedRealityService>();
builder.Services.AddScoped<IWorkforceAnalyticsService, WorkforceAnalyticsService>();
builder.Services.AddScoped<IDigitalTransformationService, DigitalTransformationService>();
builder.Services.AddScoped<IAdvancedBusinessIntelligenceService, AdvancedBusinessIntelligenceService>();
builder.Services.AddScoped<IHumanResourcesService, HumanResourcesService>();
builder.Services.AddScoped<ISalesManagementService, SalesManagementService>();
builder.Services.AddScoped<IFinancialManagementService, FinancialManagementService>();
builder.Services.AddScoped<IProjectManagementService, ProjectManagementService>();
builder.Services.AddScoped<ICustomerRelationshipService, CustomerRelationshipService>();
builder.Services.AddScoped<IQualityAssuranceService, QualityAssuranceService>();
builder.Services.AddScoped<IEnvironmentalManagementService, EnvironmentalManagementService>();
builder.Services.AddScoped<ISecurityManagementService, SecurityManagementService>();
builder.Services.AddScoped<IChangeManagementService, ChangeManagementService>();
builder.Services.AddScoped<ICommunicationManagementService, CommunicationManagementService>();
builder.Services.AddScoped<IKnowledgeManagementService, KnowledgeManagementService>();
builder.Services.AddScoped<IBusinessProcessManagementService, BusinessProcessManagementService>();
builder.Services.AddScoped<IComplianceManagementService, ComplianceManagementService>();
builder.Services.AddScoped<IGovernanceManagementService, GovernanceManagementService>();
builder.Services.AddScoped<ITechnologyManagementService, TechnologyManagementService>();
builder.Services.AddScoped<IDevOpsManagementService, DevOpsManagementService>();
builder.Services.AddScoped<IArtificialIntelligenceService, ArtificialIntelligenceService>();
builder.Services.AddScoped<IMachineLearningService, MachineLearningService>();
builder.Services.AddScoped<IDataScienceService, DataScienceService>();
builder.Services.AddScoped<IPredictiveAnalyticsService, PredictiveAnalyticsService>();
builder.Services.AddScoped<INaturalLanguageProcessingService, NaturalLanguageProcessingService>();
builder.Services.AddScoped<IComputerVisionService, ComputerVisionService>();
builder.Services.AddScoped<INeuralNetworkService, NeuralNetworkService>();
builder.Services.AddScoped<ICloudComputingManagementService, CloudComputingManagementService>();
builder.Services.AddScoped<IInternetOfThingsService, InternetOfThingsService>();
builder.Services.AddScoped<IBlockchainManagementService, BlockchainManagementService>();
builder.Services.AddScoped<IRoboticsAutomationService, RoboticsAutomationService>();
builder.Services.AddScoped<IVirtualRealityService, VirtualRealityService>();
builder.Services.AddScoped<IEdgeComputingService, EdgeComputingService>();
builder.Services.AddScoped<IAdvancedStreamProcessingService, AdvancedStreamProcessingService>();
builder.Services.AddScoped<IBusinessIntelligenceManagementService, BusinessIntelligenceManagementService>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Disabled for development and tunnel deployment
    options.SaveToken = true;
    options.Authority = null; // Disable authority validation for tunnel deployment
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // Disabled for development
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = false, // Disabled for development
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Disable antiforgery for deployment
// builder.Services.AddAntiforgery();

// Add CORS with permissive configuration for deployment
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHudurTelemetry("Hudur Enterprise Platform");

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}).AddJsonProtocol();

// Configure SignalR with CORS
builder.Services.Configure<Microsoft.AspNetCore.SignalR.HubOptions>(options =>
{
    options.EnableDetailedErrors = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Hudur Enterprise Platform API", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseCors("AllowAll");

// Configure forwarded headers for proxy support
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    RequireHeaderSymmetry = false,
    KnownNetworks = { },
    KnownProxies = { }
});

// Disable host filtering for deployment - accept any hostname
app.Use(async (context, next) =>
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    context.Request.Host = new HostString("localhost", int.Parse(port));
    await next();
});


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AttendancePlatformDbContext>();
    context.Database.EnsureCreated();
    
    if (!context.Users.Any())
    {
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseMiddleware<AttendancePlatform.Shared.Infrastructure.Middleware.GlobalExceptionMiddleware>();
app.UseMiddleware<AttendancePlatform.Shared.Infrastructure.Middleware.InputSanitizationMiddleware>();
app.UseMiddleware<AttendancePlatform.Shared.Infrastructure.Middleware.HtmlSanitizationMiddleware>();
app.UseMiddleware<AttendancePlatform.Shared.Infrastructure.Middleware.SecurityHeadersMiddleware>();
app.UseMiddleware<AttendancePlatform.Shared.Infrastructure.Middleware.RateLimitingMiddleware>();
// Disable CSRF validation for deployment
// app.UseMiddleware<CsrfValidationMiddleware>();

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<AttendancePlatform.Api.Middleware.RateLimitingMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/hubs/realtime").RequireCors("AllowAll");

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();

public partial class Program { }
