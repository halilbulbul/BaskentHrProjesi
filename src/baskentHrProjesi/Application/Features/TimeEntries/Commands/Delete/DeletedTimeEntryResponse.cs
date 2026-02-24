using NArchitecture.Core.Application.Responses;

namespace Application.Features.TimeEntries.Commands.Delete;

public class DeletedTimeEntryResponse : IResponse
{
    public int Id { get; set; }
}