using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using QRCoder;
using OtpNet;
using AttendancePlatform.Shared.Infrastructure.Services;

namespace AttendancePlatform.Shared.Infrastructure.Security;

public interface IMultiFactorAuthenticationService
{
    Task<MfaSetupResult> SetupMfaAsync(Guid userId, string userEmail);
    Task<bool> ValidateMfaTokenAsync(Guid userId, string token);
    Task<bool> ValidateBackupCodeAsync(Guid userId, string backupCode);
    Task<List<string>> GenerateBackupCodesAsync(Guid userId);
    Task<bool> DisableMfaAsync(Guid userId);
    Task<MfaStatus> GetMfaStatusAsync(Guid userId);
}

public class MultiFactorAuthenticationService : IMultiFactorAuthenticationService
{
    private readonly ILogger<MultiFactorAuthenticationService> _logger;
    private readonly MfaOptions _options;
    private readonly ICacheService _cacheService;
    private readonly IEncryptionService _encryptionService;

    public MultiFactorAuthenticationService(
        ILogger<MultiFactorAuthenticationService> logger,
        IOptions<MfaOptions> options,
        ICacheService cacheService,
        IEncryptionService encryptionService)
    {
        _logger = logger;
        _options = options.Value;
        _cacheService = cacheService;
        _encryptionService = encryptionService;
    }

    public async Task<MfaSetupResult> SetupMfaAsync(Guid userId, string userEmail)
    {
        try
        {
            var secretKey = GenerateSecretKey();
            var encryptedSecret = await _encryptionService.EncryptAsync(secretKey);
            
            await _cacheService.SetAsync($"mfa_secret_{userId}", encryptedSecret, TimeSpan.FromMinutes(10));
            
            var issuer = _options.Issuer;
            var accountTitle = $"{issuer}:{userEmail}";
            var qrCodeUrl = $"otpauth://totp/{Uri.EscapeDataString(accountTitle)}?secret={secretKey}&issuer={Uri.EscapeDataString(issuer)}";
            
            var qrCodeBytes = GenerateQrCode(qrCodeUrl);
            var backupCodes = GenerateBackupCodes();
            
            await _cacheService.SetAsync($"mfa_backup_codes_{userId}", 
                await _encryptionService.EncryptAsync(string.Join(",", backupCodes)), 
                TimeSpan.FromMinutes(10));

            _logger.LogInformation("MFA setup initiated for user {UserId}", userId);

            return new MfaSetupResult
            {
                SecretKey = secretKey,
                QrCodeImage = qrCodeBytes,
                BackupCodes = backupCodes,
                ManualEntryKey = FormatSecretKeyForManualEntry(secretKey)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up MFA for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ValidateMfaTokenAsync(Guid userId, string token)
    {
        try
        {
            var encryptedSecret = await _cacheService.GetAsync<string>($"mfa_secret_{userId}");
            if (string.IsNullOrEmpty(encryptedSecret))
            {
                _logger.LogWarning("MFA secret not found for user {UserId}", userId);
                return false;
            }

            var secretKey = await _encryptionService.DecryptAsync(encryptedSecret);
            var secretBytes = Base32Encoding.ToBytes(secretKey);
            
            var totp = new Totp(secretBytes, step: _options.TimeStepSeconds);
            var verificationWindow = new VerificationWindow(previous: _options.TimeWindow, future: _options.TimeWindow);
            var isValid = totp.VerifyTotp(token, out var timeStepMatched, verificationWindow);

            if (isValid)
            {
                var usedTokenKey = $"mfa_used_token_{userId}_{token}_{timeStepMatched}";
                var tokenAlreadyUsed = await _cacheService.GetAsync<bool>(usedTokenKey);
                
                if (tokenAlreadyUsed)
                {
                    _logger.LogWarning("MFA token replay attempt detected for user {UserId}", userId);
                    return false;
                }

                await _cacheService.SetAsync(usedTokenKey, true, TimeSpan.FromSeconds(_options.TimeStepSeconds * 2));
                _logger.LogInformation("Valid MFA token provided for user {UserId}", userId);
            }
            else
            {
                _logger.LogWarning("Invalid MFA token provided for user {UserId}", userId);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating MFA token for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ValidateBackupCodeAsync(Guid userId, string backupCode)
    {
        try
        {
            var encryptedBackupCodes = await _cacheService.GetAsync<string>($"mfa_backup_codes_{userId}");
            if (string.IsNullOrEmpty(encryptedBackupCodes))
            {
                return false;
            }

            var backupCodesString = await _encryptionService.DecryptAsync(encryptedBackupCodes);
            var backupCodes = backupCodesString.Split(',').ToList();

            if (backupCodes.Contains(backupCode))
            {
                backupCodes.Remove(backupCode);
                var updatedBackupCodes = string.Join(",", backupCodes);
                await _cacheService.SetAsync($"mfa_backup_codes_{userId}", 
                    await _encryptionService.EncryptAsync(updatedBackupCodes), 
                    TimeSpan.FromDays(30));

                _logger.LogInformation("Valid MFA backup code used for user {UserId}", userId);
                return true;
            }

            _logger.LogWarning("Invalid MFA backup code provided for user {UserId}", userId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating MFA backup code for user {UserId}", userId);
            return false;
        }
    }

    public async Task<List<string>> GenerateBackupCodesAsync(Guid userId)
    {
        var backupCodes = GenerateBackupCodes();
        await _cacheService.SetAsync($"mfa_backup_codes_{userId}", 
            await _encryptionService.EncryptAsync(string.Join(",", backupCodes)), 
            TimeSpan.FromDays(30));
        
        _logger.LogInformation("New MFA backup codes generated for user {UserId}", userId);
        return backupCodes;
    }

    public async Task<bool> DisableMfaAsync(Guid userId)
    {
        try
        {
            await _cacheService.RemoveAsync($"mfa_secret_{userId}");
            await _cacheService.RemoveAsync($"mfa_backup_codes_{userId}");
            
            _logger.LogInformation("MFA disabled for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disabling MFA for user {UserId}", userId);
            return false;
        }
    }

    public async Task<MfaStatus> GetMfaStatusAsync(Guid userId)
    {
        var hasSecret = !string.IsNullOrEmpty(await _cacheService.GetAsync<string>($"mfa_secret_{userId}"));
        var hasBackupCodes = !string.IsNullOrEmpty(await _cacheService.GetAsync<string>($"mfa_backup_codes_{userId}"));

        return new MfaStatus
        {
            IsEnabled = hasSecret,
            HasBackupCodes = hasBackupCodes,
            BackupCodesRemaining = hasBackupCodes ? await GetBackupCodesCountAsync(userId) : 0
        };
    }

    private string GenerateSecretKey()
    {
        var key = new byte[20];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(key);
        return Base32Encoding.ToString(key);
    }

    private byte[] GenerateQrCode(string qrCodeUrl)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUrl, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }

    private List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        using var rng = RandomNumberGenerator.Create();
        
        for (int i = 0; i < _options.BackupCodesCount; i++)
        {
            var codeBytes = new byte[5];
            rng.GetBytes(codeBytes);
            var code = Convert.ToHexString(codeBytes).ToLowerInvariant();
            codes.Add($"{code[..4]}-{code[4..]}");
        }
        
        return codes;
    }

    private string FormatSecretKeyForManualEntry(string secretKey)
    {
        return string.Join(" ", Enumerable.Range(0, secretKey.Length / 4)
            .Select(i => secretKey.Substring(i * 4, 4)));
    }

    private async Task<int> GetBackupCodesCountAsync(Guid userId)
    {
        try
        {
            var encryptedBackupCodes = await _cacheService.GetAsync<string>($"mfa_backup_codes_{userId}");
            if (string.IsNullOrEmpty(encryptedBackupCodes))
            {
                return 0;
            }

            var backupCodesString = await _encryptionService.DecryptAsync(encryptedBackupCodes);
            return backupCodesString.Split(',', StringSplitOptions.RemoveEmptyEntries).Length;
        }
        catch
        {
            return 0;
        }
    }
}

public class MfaSetupResult
{
    public string SecretKey { get; set; } = string.Empty;
    public byte[] QrCodeImage { get; set; } = Array.Empty<byte>();
    public List<string> BackupCodes { get; set; } = new();
    public string ManualEntryKey { get; set; } = string.Empty;
}

public class MfaStatus
{
    public bool IsEnabled { get; set; }
    public bool HasBackupCodes { get; set; }
    public int BackupCodesRemaining { get; set; }
}

public class MfaOptions
{
    public string Issuer { get; set; } = "Hudur";
    public int TimeStepSeconds { get; set; } = 30;
    public int TimeWindow { get; set; } = 1;
    public int BackupCodesCount { get; set; } = 10;
}
