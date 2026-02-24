using NArchitecture.Core.Application.Dtos;

namespace Application.Features.Timesheets.Queries.GetList;

public class GetListTimesheetListItemDto : IDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public int ShiftId { get; set; }
    public int PlannedMinutes { get; set; }
    public int ActualMinutes { get; set; }
    public int OvertimeMinutes { get; set; }
    public int MissingMinutes { get; set; }
    public int LateArrivalMinutes { get; set; }
    public int EarlyLeaveMinutes { get; set; }
    public int EarlyArrivalMinutes { get; set; }
    public int LateLeaveMinutes { get; set; }
    public string Status { get; set; }
    public int? EmployeeLeaveId { get; set; }
}