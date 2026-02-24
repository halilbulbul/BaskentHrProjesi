using FluentValidation;

namespace Application.Features.TimeEntries.Commands.Update;

public class UpdateTimeEntryCommandValidator : AbstractValidator<UpdateTimeEntryCommand>
{
    public UpdateTimeEntryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.EmployeeId).NotEmpty();
        RuleFor(c => c.EventTime).NotEmpty();
        RuleFor(c => c.Direction).NotEmpty();
        RuleFor(c => c.Source).NotEmpty();
        RuleFor(c => c.DeviceId).NotEmpty();
    }
}