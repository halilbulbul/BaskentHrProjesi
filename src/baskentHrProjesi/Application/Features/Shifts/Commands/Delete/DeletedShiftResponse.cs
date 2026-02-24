using NArchitecture.Core.Application.Responses;

namespace Application.Features.Shifts.Commands.Delete;

public class DeletedShiftResponse : IResponse
{
    public int Id { get; set; }
}