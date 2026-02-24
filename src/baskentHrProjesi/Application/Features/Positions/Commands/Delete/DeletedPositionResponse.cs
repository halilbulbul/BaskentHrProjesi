using NArchitecture.Core.Application.Responses;

namespace Application.Features.Positions.Commands.Delete;

public class DeletedPositionResponse : IResponse
{
    public int Id { get; set; }
}