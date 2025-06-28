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

[assembly: InternalsVisibleTo("AttendancePlatform.Tests.Integration")]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add infrastructure services with SQL Server database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
                       (builder.Environment.IsDevelopment() ? 
                        "Server=localhost;Database=AttendancePlatform;Trusted_Connection=true;Encrypt=true;" :
                        "Server=localhost;Database=AttendancePlatform;Trusted_Connection=true;Encrypt=true;");

builder.Services.AddDbContext<AttendancePlatformDbContext>(options =>
    options.UseSqlServer(connectionString));

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

builder.Services.AddScoped<IJwtTokenService, AttendancePlatform.Api.Services.JwtTokenService>();
builder.Services.AddScoped<ITwoFactorService, AttendancePlatform.Api.Services.TwoFactorService>();
builder.Services.AddScoped<AttendancePlatform.Application.Services.IEmailService, AttendancePlatform.Api.Services.EmailService>();
builder.Services.AddScoped<IRefreshTokenService, AttendancePlatform.Api.Services.RefreshTokenService>();

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
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173", 
                "http://localhost:5174",
                "http://localhost:3000",
                "https://project-review-app-7tx5ua47.devinapps.com",
                "https://attendancepro-auth-api.devinapps.com"
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
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

app.UseHttpsRedirection();

app.UseCors("AllowAll");

if (!app.Environment.IsDevelopment())
{
    app.UseMiddleware<RateLimitingMiddleware>();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/api/hubs/realtime");

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();

public partial class Program { }
