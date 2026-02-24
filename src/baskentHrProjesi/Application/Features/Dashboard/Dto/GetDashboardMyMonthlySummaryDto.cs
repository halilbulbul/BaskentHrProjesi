using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardMyMonthlySummaryDto : IResponse
{
    public int OvertimeMinutes { get; set; }     
    public int LateArrivalMinutes { get; set; }   
    public int UsedLeaveDays { get; set; }        
}
