using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Application.DTOs;

namespace AttendancePlatform.Application.Services;

public interface IAttendanceService
{
    Task<ApiResponse<Application.DTOs.AttendanceRecordDto>> CheckInAsync(Application.DTOs.CheckInRequest request, Guid userId);
    Task<ApiResponse<Application.DTOs.AttendanceRecordDto>> CheckOutAsync(Application.DTOs.CheckOutRequest request, Guid userId);
    Task<ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>> GetUserAttendanceAsync(Guid userId, DateTime? startDate = null, DateTime? endDate = null);
    Task<ApiResponse<IEnumerable<Application.DTOs.AttendanceRecordDto>>> GetTodayAttendanceAsync(Guid userId);
    Task<ApiResponse<Application.DTOs.AttendanceRecordDto?>> GetLastAttendanceAsync(Guid userId);
    Task<ApiResponse<bool>> ValidateGeofenceAsync(double latitude, double longitude, Guid userId);
    Task<ApiResponse<bool>> ValidateBeaconAsync(string beaconId, Guid userId);
    Task<ApiResponse<PagedResult<Application.DTOs.AttendanceRecordDto>>> GetAttendanceReportsAsync(PagedRequest request, Guid? userId = null);
}
