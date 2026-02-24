using NArchitecture.Core.Application.Responses;

namespace Application.Features.EmployeeLeaves.Commands.Reject;

public class RejectedEmployeeLeaveResponse : IResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string? ApprovalStatus { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public int? ApproverEmployeeId { get; set; }
}
