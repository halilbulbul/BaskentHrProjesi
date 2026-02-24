using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardDepartmentDailyTrendResponse : IResponse
{
    public IList<DashboardDepartmentDailyTrendDto> DepartmentDailyTrend { get; set; }
}
