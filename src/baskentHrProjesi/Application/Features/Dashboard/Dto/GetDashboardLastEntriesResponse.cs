using System.Collections.Generic;
using NArchitecture.Core.Application.Responses;

namespace Application.Features.Dashboard.Dto;

public class GetDashboardLastEntriesResponse : IResponse
{
    public IList<DashboardLastEntryDto> LastEntries { get; set; }
}
