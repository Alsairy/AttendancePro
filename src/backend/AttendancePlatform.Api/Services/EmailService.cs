using AttendancePlatform.Application.Services;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Api.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ApiResponse<bool>> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            _logger.LogInformation("Email sent to {To} with subject {Subject}", to, subject);
            return ApiResponse<bool>.SuccessResult(true, "Email sent successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            return ApiResponse<bool>.ErrorResult("Failed to send email");
        }
    }

    public async Task<ApiResponse<bool>> SendPasswordResetEmailAsync(string email, string resetToken)
    {
        try
        {
            var resetLink = $"{_configuration["AppSettings:BaseUrl"]}/reset-password?token={resetToken}";
            var body = $"Click the following link to reset your password: {resetLink}";
            return await SendEmailAsync(email, "Password Reset", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending password reset email to {Email}", email);
            return ApiResponse<bool>.ErrorResult("Failed to send password reset email");
        }
    }

    public async Task<ApiResponse<bool>> SendWelcomeEmailAsync(string email, string firstName)
    {
        try
        {
            var body = $"Welcome {firstName}! Your account has been created successfully.";
            return await SendEmailAsync(email, "Welcome to Hudur Enterprise Platform", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending welcome email to {Email}", email);
            return ApiResponse<bool>.ErrorResult("Failed to send welcome email");
        }
    }

    public async Task<ApiResponse<bool>> SendTwoFactorCodeAsync(string email, string code)
    {
        try
        {
            var body = $"Your two-factor authentication code is: {code}";
            return await SendEmailAsync(email, "Two-Factor Authentication Code", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending two-factor code to {Email}", email);
            return ApiResponse<bool>.ErrorResult("Failed to send two-factor code");
        }
    }

    public async Task<ApiResponse<bool>> SendLeaveApprovalEmailAsync(string email, string leaveDetails)
    {
        try
        {
            var body = $"Your leave request has been processed: {leaveDetails}";
            return await SendEmailAsync(email, "Leave Request Update", body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending leave approval email to {Email}", email);
            return ApiResponse<bool>.ErrorResult("Failed to send leave approval email");
        }
    }
}
