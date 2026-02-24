using NArchitecture.Core.Application.Responses;

namespace Application.Features.Shifts.Commands.Create;

public class CreatedShiftResponse : IResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TimeSpan ShiftStartTime { get; set; }
    public TimeSpan ShiftEndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public int EarlyArrivalToleranceMinutes { get; set; }
    public int LateArrivalToleranceMinutes { get; set; }
    public int EarlyLeaveToleranceMinutes { get; set; }
    public int LateLeaveToleranceMinutes { get; set; }
}