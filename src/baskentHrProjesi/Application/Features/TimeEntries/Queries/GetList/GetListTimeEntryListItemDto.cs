using NArchitecture.Core.Application.Dtos;

namespace Application.Features.TimeEntries.Queries.GetList;

public class GetListTimeEntryListItemDto : IDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = null!;
    public DateTime EventTime { get; set; }
    public string Direction { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
}
