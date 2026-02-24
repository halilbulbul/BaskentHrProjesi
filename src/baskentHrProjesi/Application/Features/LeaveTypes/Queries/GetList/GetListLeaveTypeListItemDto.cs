using NArchitecture.Core.Application.Dtos;

namespace Application.Features.LeaveTypes.Queries.GetList;

public class GetListLeaveTypeListItemDto : IDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsPaid { get; set; }
}