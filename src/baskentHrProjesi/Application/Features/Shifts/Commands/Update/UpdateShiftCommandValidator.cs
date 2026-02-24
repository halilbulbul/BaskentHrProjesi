using FluentValidation;

namespace Application.Features.Shifts.Commands.Update;

public class UpdateShiftCommandValidator : AbstractValidator<UpdateShiftCommand>
{
    public UpdateShiftCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.ShiftStartTime).NotEmpty();
        RuleFor(c => c.ShiftEndTime).NotEmpty();
        RuleFor(c => c.EarlyArrivalToleranceMinutes).NotEmpty();
        RuleFor(c => c.LateArrivalToleranceMinutes).NotEmpty();
        RuleFor(c => c.EarlyLeaveToleranceMinutes).NotEmpty();
        RuleFor(c => c.LateLeaveToleranceMinutes).NotEmpty();
    }
}