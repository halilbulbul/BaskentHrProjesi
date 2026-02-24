using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardDepartmentCountsResponse : IResponse
{
    public IList<DashboardDepartmentCountDto> DepartmentCounts { get; set; }
}
