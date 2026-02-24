using NArchitecture.Core.Application.Dtos;

namespace Application.Features.EmployeeLeaves.Queries.GetList;

public class GetListEmployeeLeaveListItemDto : IDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime RequestDate { get; set; }
    public int? ApproverEmployeeId { get; set; }
}