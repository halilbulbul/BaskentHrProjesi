using NArchitecture.Core.Application.Responses;

namespace Application.Features.Positions.Queries.GetById;

public class GetByIdPositionResponse : IResponse
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}