using FluentValidation;

namespace Application.Features.TimeEntries.Commands.Create;

public class CreateTimeEntryCommandValidator : AbstractValidator<CreateTimeEntryCommand>
{
    public CreateTimeEntryCommandValidator()
    {
        RuleFor(c => c.EmployeeId).NotEmpty();
        RuleFor(c => c.EventTime).NotEmpty();
        RuleFor(c => c.Direction).NotEmpty();
        RuleFor(c => c.Source).NotEmpty();
        RuleFor(c => c.DeviceId).NotEmpty();
    }
}