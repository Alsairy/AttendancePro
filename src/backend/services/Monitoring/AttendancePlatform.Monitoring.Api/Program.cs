using AttendancePlatform.Shared.Infrastructure.Extensions;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Prometheus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("Service", "MonitoringService")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AttendancePlatformDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSharedInfrastructure(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddSource("AttendancePlatform.Monitoring")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("AttendancePlatform.Monitoring.Api"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddJaegerExporter())
    .WithMetrics(meterProviderBuilder =>
        meterProviderBuilder
            .AddMeter("AttendancePlatform.Monitoring")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("AttendancePlatform.Monitoring.Api"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter());

builder.Services.AddHealthChecks()
    .AddDbContext<AttendancePlatformDbContext>()
    .AddCheck("monitoring_service", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();
app.MapMetrics();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
