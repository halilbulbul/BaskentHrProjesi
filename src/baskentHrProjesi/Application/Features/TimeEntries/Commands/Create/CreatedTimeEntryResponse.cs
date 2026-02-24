using NArchitecture.Core.Application.Responses;

namespace Application.Features.TimeEntries.Commands.Create;

public class CreatedTimeEntryResponse : IResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime EventTime { get; set; }
    public string Direction { get; set; }
    public string Source { get; set; }
    public string DeviceId { get; set; }
}