using NArchitecture.Core.Application.Responses;

namespace Application.Features.LeaveTypes.Commands.Delete;

public class DeletedLeaveTypeResponse : IResponse
{
    public int Id { get; set; }
}