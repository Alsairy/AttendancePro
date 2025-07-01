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

[assembly: InternalsVisibleTo("AttendancePlatform.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Kestrel to accept any host header
builder.WebHost.ConfigureKestrel(options =>
{
    options.AllowSynchronousIO = true;
    options.Limits.MaxRequestBodySize = null;
    options.ListenAnyIP(5002);
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

// Add CORS with specific frontend origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "https://attendancepro-fixapp-jur4spo0.devinapps.com",
                "http://localhost:3000",
                "http://localhost:5173",
                "https://localhost:5173"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("*");
    });
    
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders("*");
    });
});

builder.Services.AddHudurTelemetry("Hudur Enterprise Platform");

builder.Services.AddSignalR();

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

// Configure forwarded headers for proxy support
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost,
    RequireHeaderSymmetry = false,
    KnownNetworks = { },
    KnownProxies = { }
});

// Accept all hostnames for deployment
app.Use(async (context, next) =>
{
    await next();
});

app.Use(async (context, next) =>
{
    var origin = context.Request.Headers["Origin"].ToString();
    
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
    }
    else
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    }
    
    context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS, PATCH");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization, X-Requested-With, Accept, Origin, X-CSRF-Token");
    context.Response.Headers.Add("Access-Control-Expose-Headers", "*");
    
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("");
        return;
    }
    
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

app.MapHub<NotificationHub>("/api/hubs/realtime");

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();

public partial class Program { }
