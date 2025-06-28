using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace AttendancePlatform.Shared.Infrastructure.Security;

public class MutualTlsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MutualTlsMiddleware> _logger;
    private readonly MutualTlsOptions _options;

    public MutualTlsMiddleware(RequestDelegate next, ILogger<MutualTlsMiddleware> logger, MutualTlsOptions options)
    {
        _next = next;
        _logger = logger;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.RequireMutualTls && ShouldValidateClientCertificate(context))
        {
            var clientCertificate = context.Connection.ClientCertificate;
            
            if (clientCertificate == null)
            {
                _logger.LogWarning("Client certificate required but not provided for {Path}", context.Request.Path);
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Client certificate required");
                return;
            }

            if (!IsValidClientCertificate(clientCertificate))
            {
                _logger.LogWarning("Invalid client certificate provided for {Path}", context.Request.Path);
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid client certificate");
                return;
            }

            _logger.LogInformation("Valid client certificate provided by {Subject}", clientCertificate.Subject);
        }

        await _next(context);
    }

    private bool ShouldValidateClientCertificate(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant();
        
        return _options.ProtectedPaths.Any(protectedPath => 
            path?.StartsWith(protectedPath.ToLowerInvariant()) == true);
    }

    private bool IsValidClientCertificate(X509Certificate2 certificate)
    {
        try
        {
            var chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
            
            if (_options.TrustedCertificateAuthorities?.Any() == true)
            {
                foreach (var cert in _options.TrustedCertificateAuthorities)
                {
                    chain.ChainPolicy.ExtraStore.Add(cert);
                }
            }

            var isValid = chain.Build(certificate);
            
            if (!isValid)
            {
                _logger.LogWarning("Certificate chain validation failed: {Errors}", 
                    string.Join(", ", chain.ChainStatus.Select(s => s.StatusInformation)));
            }

            return isValid && certificate.NotAfter > DateTime.UtcNow && certificate.NotBefore <= DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating client certificate");
            return false;
        }
    }
}

public class MutualTlsOptions
{
    public bool RequireMutualTls { get; set; } = false;
    public List<string> ProtectedPaths { get; set; } = new();
    public List<X509Certificate2> TrustedCertificateAuthorities { get; set; } = new();
}
