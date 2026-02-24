using NArchitecture.Core.Application.Responses;

namespace Application.Features.EmployeeLeaves.Commands.Update;

public class UpdatedEmployeeLeaveResponse : IResponse
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