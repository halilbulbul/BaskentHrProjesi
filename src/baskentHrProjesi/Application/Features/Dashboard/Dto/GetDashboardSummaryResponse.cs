using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardSummaryResponse : IResponse
{
    public int ActiveEmployeeCount { get; set; }
    public int OpenLeaveCount { get; set; }
    public int NewHireCount { get; set; }
    public int OpenTimesheetDayCount { get; set; }
    public int LateEmployeeCount { get; set; }
    public bool IsAdmin { get; set; }
}
