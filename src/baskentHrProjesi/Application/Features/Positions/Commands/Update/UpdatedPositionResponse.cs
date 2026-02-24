using NArchitecture.Core.Application.Responses;

namespace Application.Features.Positions.Commands.Update;

public class UpdatedPositionResponse : IResponse
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}