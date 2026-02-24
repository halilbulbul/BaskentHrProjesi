using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardDailyTrendResponse : IResponse
{
    public IList<DashboardDailyTrendItemDto> DailyTrend { get; set; }
}
