using NArchitecture.Core.Application.Responses;

namespace Application.Features.LeaveTypes.Commands.Update;

public class UpdatedLeaveTypeResponse : IResponse
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsPaid { get; set; }
}