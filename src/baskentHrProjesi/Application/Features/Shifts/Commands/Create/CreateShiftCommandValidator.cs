using FluentValidation;

namespace Application.Features.Shifts.Commands.Create;

public class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.ShiftStartTime).NotEmpty();
        RuleFor(c => c.ShiftEndTime).NotEmpty();
        RuleFor(c => c.EarlyArrivalToleranceMinutes).NotEmpty();
        RuleFor(c => c.LateArrivalToleranceMinutes).NotEmpty();
        RuleFor(c => c.EarlyLeaveToleranceMinutes).NotEmpty();
        RuleFor(c => c.LateLeaveToleranceMinutes).NotEmpty();
    }
}