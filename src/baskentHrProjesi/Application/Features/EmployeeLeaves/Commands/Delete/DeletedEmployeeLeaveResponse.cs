using NArchitecture.Core.Application.Responses;

namespace Application.Features.EmployeeLeaves.Commands.Delete;

public class DeletedEmployeeLeaveResponse : IResponse
{
    public int Id { get; set; }
}