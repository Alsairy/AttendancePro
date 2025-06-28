using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IEmailService
{
    Task<ApiResponse<bool>> SendEmailAsync(string to, string subject, string body);
    Task<ApiResponse<bool>> SendPasswordResetEmailAsync(string email, string resetToken);
    Task<ApiResponse<bool>> SendWelcomeEmailAsync(string email, string firstName);
    Task<ApiResponse<bool>> SendTwoFactorCodeAsync(string email, string code);
    Task<ApiResponse<bool>> SendLeaveApprovalEmailAsync(string email, string leaveDetails);
}
