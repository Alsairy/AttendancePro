using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Shared.Infrastructure.GraphQL
{
    public class AttendanceGraphQLSchema : Schema
    {
        public AttendanceGraphQLSchema(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Query = serviceProvider.GetRequiredService<AttendanceQuery>();
            Mutation = serviceProvider.GetRequiredService<AttendanceMutation>();
        }
    }

    public class AttendanceQuery : ObjectGraphType
    {
        public AttendanceQuery(AttendancePlatformDbContext context)
        {
            Field<ListGraphType<UserType>>("users")
                .Argument<GuidGraphType>("tenantId")
                .Argument<IntGraphType>("first", "Number of users to fetch")
                .Argument<IntGraphType>("offset", "Number of users to skip")
                .ResolveAsync(async context =>
                {
                    var tenantId = context.GetArgument<Guid>("tenantId");
                    var first = context.GetArgument<int>("first", 10);
                    var offset = context.GetArgument<int>("offset", 0);

                    return await context.RequestServices
                        .GetRequiredService<AttendancePlatformDbContext>()
                        .Users
                        .Where(u => u.TenantId == tenantId && u.Status == UserStatus.Active)
                        .Skip(offset)
                        .Take(first)
                        .AsNoTracking()
                        .ToListAsync();
                });

            Field<ListGraphType<AttendanceRecordType>>("attendanceRecords")
                .Argument<GuidGraphType>("userId")
                .Argument<DateTimeGraphType>("startDate")
                .Argument<DateTimeGraphType>("endDate")
                .Argument<IntGraphType>("first", "Number of records to fetch")
                .Argument<IntGraphType>("offset", "Number of records to skip")
                .ResolveAsync(async context =>
                {
                    var userId = context.GetArgument<Guid>("userId");
                    var startDate = context.GetArgument<DateTime>("startDate");
                    var endDate = context.GetArgument<DateTime>("endDate");
                    var first = context.GetArgument<int>("first", 50);
                    var offset = context.GetArgument<int>("offset", 0);

                    return await context.RequestServices
                        .GetRequiredService<AttendancePlatformDbContext>()
                        .AttendanceRecords
                        .Where(ar => ar.UserId == userId && 
                                   ar.Timestamp >= startDate && 
                                   ar.Timestamp <= endDate)
                        .OrderByDescending(ar => ar.Timestamp)
                        .Skip(offset)
                        .Take(first)
                        .AsNoTracking()
                        .ToListAsync();
                });

            Field<ListGraphType<LeaveRequestType>>("leaveRequests")
                .Argument<GuidGraphType>("tenantId")
                .Argument<StringGraphType>("status")
                .Argument<IntGraphType>("first", "Number of requests to fetch")
                .Argument<IntGraphType>("offset", "Number of requests to skip")
                .ResolveAsync(async context =>
                {
                    var tenantId = context.GetArgument<Guid>("tenantId");
                    var status = context.GetArgument<string>("status");
                    var first = context.GetArgument<int>("first", 20);
                    var offset = context.GetArgument<int>("offset", 0);

                    var query = context.RequestServices
                        .GetRequiredService<AttendancePlatformDbContext>()
                        .LeaveRequests
                        .Where(lr => lr.TenantId == tenantId);

                    if (!string.IsNullOrEmpty(status) && Enum.TryParse<LeaveRequestStatus>(status, out var statusEnum))
                    {
                        query = query.Where(lr => lr.Status == statusEnum);
                    }

                    return await query
                        .OrderByDescending(lr => lr.CreatedAt)
                        .Skip(offset)
                        .Take(first)
                        .AsNoTracking()
                        .ToListAsync();
                });

            Field<DashboardDataType>("dashboardData")
                .Argument<GuidGraphType>("tenantId")
                .Argument<GuidGraphType>("userId")
                .ResolveAsync(async context =>
                {
                    var tenantId = context.GetArgument<Guid>("tenantId");
                    var userId = context.GetArgument<Guid>("userId");
                    var dbContext = context.RequestServices.GetRequiredService<AttendancePlatformDbContext>();

                    var today = DateTime.UtcNow.Date;
                    var thisMonth = new DateTime(today.Year, today.Month, 1);

                    var todayAttendanceCount = await dbContext.AttendanceRecords
                        .Where(ar => ar.TenantId == tenantId && 
                                   ar.Timestamp >= today && 
                                   ar.Timestamp < today.AddDays(1))
                        .CountAsync();

                    var userTodayAttendance = await dbContext.AttendanceRecords
                        .Where(ar => ar.UserId == userId && 
                                   ar.Timestamp >= today && 
                                   ar.Timestamp < today.AddDays(1))
                        .OrderByDescending(ar => ar.Timestamp)
                        .FirstOrDefaultAsync();

                    var pendingLeaveRequests = await dbContext.LeaveRequests
                        .Where(lr => lr.TenantId == tenantId && lr.Status == LeaveRequestStatus.Pending)
                        .CountAsync();

                    var monthlyStats = await dbContext.AttendanceRecords
                        .Where(ar => ar.TenantId == tenantId && 
                                   ar.Timestamp >= thisMonth && 
                                   ar.Timestamp < thisMonth.AddMonths(1))
                        .GroupBy(ar => ar.Timestamp.Date)
                        .Select(g => new { Date = g.Key, Count = g.Count() })
                        .ToListAsync();

                    return new DashboardData
                    {
                        TodayAttendanceCount = todayAttendanceCount,
                        UserTodayAttendance = userTodayAttendance,
                        PendingLeaveRequests = pendingLeaveRequests,
                        MonthlyStats = new MonthlyStats
                        {
                            TotalDays = monthlyStats.Count,
                            TotalAttendance = monthlyStats.Sum(s => s.Count),
                            AverageDaily = monthlyStats.Any() ? monthlyStats.Average(s => s.Count) : 0,
                            DailyBreakdown = monthlyStats.ToDictionary(s => s.Date, s => s.Count)
                        }
                    };
                });
        }
    }

    public class AttendanceMutation : ObjectGraphType
    {
        public AttendanceMutation()
        {
            Field<AttendanceRecordType>("createAttendanceRecord")
                .Argument<NonNullGraphType<AttendanceRecordInputType>>("input")
                .ResolveAsync(async context =>
                {
                    var input = context.GetArgument<AttendanceRecordInput>("input");
                    var dbContext = context.RequestServices.GetRequiredService<AttendancePlatformDbContext>();

                    var attendanceRecord = new AttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        UserId = input.UserId,
                        TenantId = input.TenantId,
                        Timestamp = input.Timestamp,
                        Status = Enum.Parse<AttendanceStatus>(input.Status),
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.AttendanceRecords.Add(attendanceRecord);
                    await dbContext.SaveChangesAsync();

                    return attendanceRecord;
                });

            Field<LeaveRequestType>("createLeaveRequest")
                .Argument<NonNullGraphType<LeaveRequestInputType>>("input")
                .ResolveAsync(async context =>
                {
                    var input = context.GetArgument<LeaveRequestInput>("input");
                    var dbContext = context.RequestServices.GetRequiredService<AttendancePlatformDbContext>();

                    var leaveRequest = new LeaveRequest
                    {
                        Id = Guid.NewGuid(),
                        UserId = input.UserId,
                        TenantId = input.TenantId,
                        StartDate = input.StartDate,
                        EndDate = input.EndDate,
                        LeaveType = Enum.Parse<LeaveType>(input.LeaveType),
                        Reason = input.Reason,
                        Status = LeaveRequestStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.LeaveRequests.Add(leaveRequest);
                    await dbContext.SaveChangesAsync();

                    return leaveRequest;
                });
        }
    }

    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field(x => x.Id);
            Field(x => x.Email);
            Field(x => x.FirstName);
            Field(x => x.LastName);
            Field(x => x.Department, nullable: true);
            Field(x => x.Position, nullable: true);
            Field<UserStatusType>("status");
            Field(x => x.CreatedAt);
        }
    }

    public class AttendanceRecordType : ObjectGraphType<AttendanceRecord>
    {
        public AttendanceRecordType()
        {
            Field(x => x.Id);
            Field(x => x.UserId);
            Field(x => x.Timestamp);
            Field(x => x.Status);
            Field(x => x.CreatedAt);
        }
    }

    public class LeaveRequestType : ObjectGraphType<LeaveRequest>
    {
        public LeaveRequestType()
        {
            Field(x => x.Id);
            Field(x => x.UserId);
            Field(x => x.StartDate);
            Field(x => x.EndDate);
            Field(x => x.LeaveType);
            Field(x => x.Reason, nullable: true);
            Field<LeaveRequestStatusType>("status");
            Field(x => x.CreatedAt);
        }
    }

    public class DashboardDataType : ObjectGraphType<DashboardData>
    {
        public DashboardDataType()
        {
            Field(x => x.TodayAttendanceCount);
            Field(x => x.PendingLeaveRequests);
            Field<AttendanceRecordType>("userTodayAttendance");
            Field<MonthlyStatsType>("monthlyStats");
        }
    }

    public class MonthlyStatsType : ObjectGraphType<MonthlyStats>
    {
        public MonthlyStatsType()
        {
            Field(x => x.TotalDays);
            Field(x => x.TotalAttendance);
            Field(x => x.AverageDaily);
        }
    }

    public class AttendanceRecordInputType : InputObjectGraphType<AttendanceRecordInput>
    {
        public AttendanceRecordInputType()
        {
            Field(x => x.UserId);
            Field(x => x.TenantId);
            Field(x => x.Timestamp);
            Field(x => x.Status);
        }
    }

    public class LeaveRequestInputType : InputObjectGraphType<LeaveRequestInput>
    {
        public LeaveRequestInputType()
        {
            Field(x => x.UserId);
            Field(x => x.TenantId);
            Field(x => x.StartDate);
            Field(x => x.EndDate);
            Field(x => x.LeaveType);
            Field(x => x.Reason, nullable: true);
        }
    }

    public class UserStatusType : EnumerationGraphType<UserStatus>
    {
    }

    public class LeaveRequestStatusType : EnumerationGraphType<LeaveRequestStatus>
    {
    }

    public class DashboardData
    {
        public int TodayAttendanceCount { get; set; }
        public AttendanceRecord UserTodayAttendance { get; set; }
        public int PendingLeaveRequests { get; set; }
        public MonthlyStats MonthlyStats { get; set; }
    }

    public class MonthlyStats
    {
        public int TotalDays { get; set; }
        public int TotalAttendance { get; set; }
        public double AverageDaily { get; set; }
        public Dictionary<DateTime, int> DailyBreakdown { get; set; } = new();
    }

    public class AttendanceRecordInput
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
    }

    public class LeaveRequestInput
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }
}
