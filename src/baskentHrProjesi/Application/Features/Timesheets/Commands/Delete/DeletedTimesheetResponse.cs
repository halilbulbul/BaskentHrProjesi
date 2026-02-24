using NArchitecture.Core.Application.Responses;

namespace Application.Features.Timesheets.Commands.Delete;

public class DeletedTimesheetResponse : IResponse
{
    public int Id { get; set; }
}