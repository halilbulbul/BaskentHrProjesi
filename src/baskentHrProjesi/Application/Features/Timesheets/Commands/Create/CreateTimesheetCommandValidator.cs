using FluentValidation;

namespace Application.Features.Timesheets.Commands.Create;

public class CreateTimesheetCommandValidator : AbstractValidator<CreateTimesheetCommand>
{
    public CreateTimesheetCommandValidator()
    {
        //RuleFor(c => c.EmployeeId).NotEmpty();
        //RuleFor(c => c.WorkDate).NotEmpty();
        //RuleFor(c => c.ShiftId).NotEmpty();
        //RuleFor(c => c.PlannedMinutes).NotEmpty();
        //RuleFor(c => c.ActualMinutes).NotEmpty();
        //RuleFor(c => c.OvertimeMinutes).NotEmpty();
        //RuleFor(c => c.MissingMinutes).NotEmpty();
        //RuleFor(c => c.LateArrivalMinutes).NotEmpty();
        //RuleFor(c => c.EarlyLeaveMinutes).NotEmpty();
        //RuleFor(c => c.EarlyArrivalMinutes).NotEmpty();
        //RuleFor(c => c.LateLeaveMinutes).NotEmpty();
        //RuleFor(c => c.Status).NotEmpty();
    }
}