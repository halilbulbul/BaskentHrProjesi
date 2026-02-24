using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardEmployeeStatusResponse : IResponse
{
    public IList<DashboardEmployeeDto> PresentEmployees { get; set; }
    public IList<DashboardEmployeeDto> AbsentEmployees { get; set; }
    public IList<DashboardEmployeeDto> LeaveEmployees { get; set; }
}
