using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IHrIntegrationService
    {
        Task<bool> SyncEmployeeDataAsync(string hrSystemType);
        Task<IEnumerable<HrEmployeeDto>> GetEmployeesFromHrSystemAsync(string hrSystemType);
        Task<HrEmployeeDto> GetEmployeeFromHrSystemAsync(string hrSystemType, string employeeId);
        Task<bool> UpdateEmployeeInHrSystemAsync(string hrSystemType, string employeeId, HrEmployeeDto employee);
        Task<bool> CreateEmployeeInHrSystemAsync(string hrSystemType, HrEmployeeDto employee);
        Task<bool> DeactivateEmployeeInHrSystemAsync(string hrSystemType, string employeeId);
        
        Task<IEnumerable<HrDepartmentDto>> GetDepartmentsFromHrSystemAsync(string hrSystemType);
        Task<IEnumerable<HrPositionDto>> GetPositionsFromHrSystemAsync(string hrSystemType);
        Task<HrSyncStatusDto> GetSyncStatusAsync(string hrSystemType);
        Task<bool> TestConnectionAsync(string hrSystemType);
    }

    public class HrEmployeeDto
    {
        public string Id { get; set; }
        public string EmployeeNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string ManagerId { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
        public string Location { get; set; }
        public Dictionary<string, object> CustomFields { get; set; }
        public DateTime LastModified { get; set; }
    }

    public class HrDepartmentDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string ManagerId { get; set; }
        public string ParentDepartmentId { get; set; }
        public bool IsActive { get; set; }
    }

    public class HrPositionDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string DepartmentId { get; set; }
        public string Description { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public bool IsActive { get; set; }
    }

    public class HrSyncStatusDto
    {
        public string HrSystemType { get; set; }
        public DateTime? LastSyncTime { get; set; }
        public bool IsConnected { get; set; }
        public string Status { get; set; }
        public int EmployeesSynced { get; set; }
        public int DepartmentsSynced { get; set; }
        public int PositionsSynced { get; set; }
        public List<string> Errors { get; set; }
        public DateTime NextScheduledSync { get; set; }
    }
}
