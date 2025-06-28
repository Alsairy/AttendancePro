using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IFaceRecognitionService
{
    Task<ApiResponse<Application.DTOs.FaceEnrollmentDto>> EnrollFaceAsync(Guid userId, byte[] imageData, string deviceId = "", string deviceType = "");
    Task<ApiResponse<Application.DTOs.FaceVerificationDto>> VerifyFaceAsync(Guid userId, byte[] imageData, string deviceId = "", string deviceType = "");
    Task<ApiResponse<IEnumerable<Application.DTOs.BiometricTemplateDto>>> GetUserTemplatesAsync(Guid userId);
    Task<ApiResponse<bool>> DeleteTemplateAsync(Guid userId, Guid templateId);
    Task<ApiResponse<Application.DTOs.FaceIdentificationDto>> IdentifyFaceAsync(byte[] imageData, string deviceId = "", string deviceType = "");
    Task<ApiResponse<bool>> UpdateTemplateAsync(Guid userId, Guid templateId, byte[] newImageData);
}
