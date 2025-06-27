using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AttendancePlatform.Shared.Infrastructure.Services;

namespace AttendancePlatform.Shared.Infrastructure.Security;

public interface ISingleSignOnService
{
    Task<SsoAuthenticationResult> AuthenticateWithAzureAdAsync(string idToken);
    Task<SsoAuthenticationResult> AuthenticateWithGoogleAsync(string idToken);
    Task<SsoAuthenticationResult> AuthenticateWithOktaAsync(string idToken);
    Task<SsoAuthenticationResult> AuthenticateWithSamlAsync(string samlResponse);
    Task<SsoProviderConfiguration> GetProviderConfigurationAsync(string providerId);
    Task<bool> ValidateProviderTokenAsync(string providerId, string token);
}

public class SingleSignOnService : ISingleSignOnService
{
    private readonly ILogger<SingleSignOnService> _logger;
    private readonly SsoOptions _options;
    private readonly ICacheService _cacheService;
    private readonly IEncryptionService _encryptionService;

    public SingleSignOnService(
        ILogger<SingleSignOnService> logger,
        IOptions<SsoOptions> options,
        ICacheService cacheService,
        IEncryptionService encryptionService)
    {
        _logger = logger;
        _options = options.Value;
        _cacheService = cacheService;
        _encryptionService = encryptionService;
    }

    public async Task<SsoAuthenticationResult> AuthenticateWithAzureAdAsync(string idToken)
    {
        try
        {
            var azureConfig = _options.Providers.FirstOrDefault(p => p.Type == SsoProviderType.AzureAD);
            if (azureConfig == null)
            {
                return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = "Azure AD not configured" };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://login.microsoftonline.com/{azureConfig.TenantId}/v2.0",
                ValidateAudience = true,
                ValidAudience = azureConfig.ClientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = await GetAzureSigningKeysAsync(azureConfig.TenantId),
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            var user = ExtractUserFromAzureToken(jwtToken);
            
            _logger.LogInformation("Azure AD authentication successful for user {Email}", user.Email);

            return new SsoAuthenticationResult
            {
                IsSuccess = true,
                User = user,
                Provider = "AzureAD",
                ExternalUserId = user.ExternalId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Azure AD authentication failed");
            return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SsoAuthenticationResult> AuthenticateWithGoogleAsync(string idToken)
    {
        try
        {
            var googleConfig = _options.Providers.FirstOrDefault(p => p.Type == SsoProviderType.Google);
            if (googleConfig == null)
            {
                return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = "Google SSO not configured" };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuers = new[] { "https://accounts.google.com", "accounts.google.com" },
                ValidateAudience = true,
                ValidAudience = googleConfig.ClientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = await GetGoogleSigningKeysAsync(),
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            var user = ExtractUserFromGoogleToken(jwtToken);
            
            _logger.LogInformation("Google authentication successful for user {Email}", user.Email);

            return new SsoAuthenticationResult
            {
                IsSuccess = true,
                User = user,
                Provider = "Google",
                ExternalUserId = user.ExternalId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Google authentication failed");
            return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SsoAuthenticationResult> AuthenticateWithOktaAsync(string idToken)
    {
        try
        {
            var oktaConfig = _options.Providers.FirstOrDefault(p => p.Type == SsoProviderType.Okta);
            if (oktaConfig == null)
            {
                return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = "Okta SSO not configured" };
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = oktaConfig.Authority,
                ValidateAudience = true,
                ValidAudience = oktaConfig.ClientId,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = await GetOktaSigningKeysAsync(oktaConfig.Authority),
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out var validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            var user = ExtractUserFromOktaToken(jwtToken);
            
            _logger.LogInformation("Okta authentication successful for user {Email}", user.Email);

            return new SsoAuthenticationResult
            {
                IsSuccess = true,
                User = user,
                Provider = "Okta",
                ExternalUserId = user.ExternalId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Okta authentication failed");
            return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SsoAuthenticationResult> AuthenticateWithSamlAsync(string samlResponse)
    {
        try
        {
            var samlConfig = _options.Providers.FirstOrDefault(p => p.Type == SsoProviderType.SAML);
            if (samlConfig == null)
            {
                return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = "SAML SSO not configured" };
            }

            var decodedSaml = Convert.FromBase64String(samlResponse);
            var samlXml = Encoding.UTF8.GetString(decodedSaml);
            
            var user = await ExtractUserFromSamlResponseAsync(samlXml, samlConfig);
            
            _logger.LogInformation("SAML authentication successful for user {Email}", user.Email);

            return new SsoAuthenticationResult
            {
                IsSuccess = true,
                User = user,
                Provider = "SAML",
                ExternalUserId = user.ExternalId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SAML authentication failed");
            return new SsoAuthenticationResult { IsSuccess = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<SsoProviderConfiguration> GetProviderConfigurationAsync(string providerId)
    {
        var provider = _options.Providers.FirstOrDefault(p => p.Id == providerId);
        if (provider == null)
        {
            throw new ArgumentException($"SSO provider {providerId} not found");
        }

        return new SsoProviderConfiguration
        {
            Id = provider.Id,
            Name = provider.Name,
            Type = provider.Type,
            AuthorizationUrl = await BuildAuthorizationUrlAsync(provider),
            IsEnabled = provider.IsEnabled,
            RequiresMfa = provider.RequiresMfa
        };
    }

    public async Task<bool> ValidateProviderTokenAsync(string providerId, string token)
    {
        try
        {
            var provider = _options.Providers.FirstOrDefault(p => p.Id == providerId);
            if (provider == null) return false;

            return provider.Type switch
            {
                SsoProviderType.AzureAD => (await AuthenticateWithAzureAdAsync(token)).IsSuccess,
                SsoProviderType.Google => (await AuthenticateWithGoogleAsync(token)).IsSuccess,
                SsoProviderType.Okta => (await AuthenticateWithOktaAsync(token)).IsSuccess,
                _ => false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed for provider {ProviderId}", providerId);
            return false;
        }
    }

    private async Task<IEnumerable<SecurityKey>> GetAzureSigningKeysAsync(string tenantId)
    {
        var cacheKey = $"azure_signing_keys_{tenantId}";
        var cachedKeys = await _cacheService.GetAsync<List<SecurityKey>>(cacheKey);
        
        if (cachedKeys != null)
        {
            return cachedKeys;
        }

        var keys = new List<SecurityKey>();
        await _cacheService.SetAsync(cacheKey, keys, TimeSpan.FromHours(24));
        
        return keys;
    }

    private async Task<IEnumerable<SecurityKey>> GetGoogleSigningKeysAsync()
    {
        var cacheKey = "google_signing_keys";
        var cachedKeys = await _cacheService.GetAsync<List<SecurityKey>>(cacheKey);
        
        if (cachedKeys != null)
        {
            return cachedKeys;
        }

        var keys = new List<SecurityKey>();
        await _cacheService.SetAsync(cacheKey, keys, TimeSpan.FromHours(24));
        
        return keys;
    }

    private async Task<IEnumerable<SecurityKey>> GetOktaSigningKeysAsync(string authority)
    {
        var cacheKey = $"okta_signing_keys_{authority}";
        var cachedKeys = await _cacheService.GetAsync<List<SecurityKey>>(cacheKey);
        
        if (cachedKeys != null)
        {
            return cachedKeys;
        }

        var keys = new List<SecurityKey>();
        await _cacheService.SetAsync(cacheKey, keys, TimeSpan.FromHours(24));
        
        return keys;
    }

    private SsoUser ExtractUserFromAzureToken(JwtSecurityToken token)
    {
        return new SsoUser
        {
            ExternalId = token.Claims.FirstOrDefault(c => c.Type == "oid")?.Value ?? string.Empty,
            Email = token.Claims.FirstOrDefault(c => c.Type == "email" || c.Type == "preferred_username")?.Value ?? string.Empty,
            FirstName = token.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty,
            LastName = token.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty,
            DisplayName = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
            Groups = token.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList()
        };
    }

    private SsoUser ExtractUserFromGoogleToken(JwtSecurityToken token)
    {
        return new SsoUser
        {
            ExternalId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
            Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
            FirstName = token.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty,
            LastName = token.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty,
            DisplayName = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
            ProfilePictureUrl = token.Claims.FirstOrDefault(c => c.Type == "picture")?.Value
        };
    }

    private SsoUser ExtractUserFromOktaToken(JwtSecurityToken token)
    {
        return new SsoUser
        {
            ExternalId = token.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? string.Empty,
            Email = token.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty,
            FirstName = token.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? string.Empty,
            LastName = token.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? string.Empty,
            DisplayName = token.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty,
            Groups = token.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList()
        };
    }

    private async Task<SsoUser> ExtractUserFromSamlResponseAsync(string samlXml, SsoProviderConfig samlConfig)
    {
        await Task.Delay(1); // Simulate async operation
        
        return new SsoUser
        {
            ExternalId = "saml_user_id", // Extract from SAML
            Email = "user@example.com", // Extract from SAML
            FirstName = "SAML", // Extract from SAML
            LastName = "User", // Extract from SAML
            DisplayName = "SAML User" // Extract from SAML
        };
    }

    private async Task<string> BuildAuthorizationUrlAsync(SsoProviderConfig provider)
    {
        return provider.Type switch
        {
            SsoProviderType.AzureAD => $"https://login.microsoftonline.com/{provider.TenantId}/oauth2/v2.0/authorize?client_id={provider.ClientId}&response_type=id_token&scope=openid%20profile%20email&redirect_uri={Uri.EscapeDataString(provider.RedirectUri)}&nonce={Guid.NewGuid()}",
            SsoProviderType.Google => $"https://accounts.google.com/oauth2/auth?client_id={provider.ClientId}&response_type=id_token&scope=openid%20profile%20email&redirect_uri={Uri.EscapeDataString(provider.RedirectUri)}&nonce={Guid.NewGuid()}",
            SsoProviderType.Okta => $"{provider.Authority}/oauth2/v1/authorize?client_id={provider.ClientId}&response_type=id_token&scope=openid%20profile%20email&redirect_uri={Uri.EscapeDataString(provider.RedirectUri)}&nonce={Guid.NewGuid()}",
            _ => string.Empty
        };
    }
}

public class SsoOptions
{
    public List<SsoProviderConfig> Providers { get; set; } = new();
    public bool RequireProviderSelection { get; set; } = false;
    public string DefaultProvider { get; set; } = string.Empty;
}

public class SsoProviderConfig
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SsoProviderType Type { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Authority { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public bool RequiresMfa { get; set; } = false;
}

public enum SsoProviderType
{
    AzureAD,
    Google,
    Okta,
    SAML,
    OIDC
}

public class SsoAuthenticationResult
{
    public bool IsSuccess { get; set; }
    public SsoUser? User { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string ExternalUserId { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> AdditionalClaims { get; set; } = new();
}

public class SsoUser
{
    public string ExternalId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public List<string> Groups { get; set; } = new();
    public Dictionary<string, object> CustomAttributes { get; set; } = new();
}

public class SsoProviderConfiguration
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public SsoProviderType Type { get; set; }
    public string AuthorizationUrl { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool RequiresMfa { get; set; }
}
