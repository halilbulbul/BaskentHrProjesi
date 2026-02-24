using NArchitecture.Core.Application.Dtos;

namespace Application.Features.Positions.Queries.GetList;

public class GetListPositionListItemDto : IDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}