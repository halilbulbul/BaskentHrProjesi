using NArchitecture.Core.Application.Dtos;

namespace Application.Features.EmployeeLeaves.Queries.GetListByCurrentUser;

public class GetListByUserEmployeeLeaveListItemDto : IDto
{
    public int Id { get; set; }
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Description { get; set; } = null!;
    public string? ApprovalStatus { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public string? ApproverFullName { get; set; }
}

