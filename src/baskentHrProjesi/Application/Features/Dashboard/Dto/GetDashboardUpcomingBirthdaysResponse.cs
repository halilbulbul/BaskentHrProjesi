using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardUpcomingBirthdaysResponse : IResponse
{
    public IList<DashboardEmployeeDto> UpcomingBirthdays { get; set; }
}
