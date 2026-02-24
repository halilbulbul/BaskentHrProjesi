using FluentValidation;

namespace Application.Features.Timesheets.Commands.Delete;

public class DeleteTimesheetCommandValidator : AbstractValidator<DeleteTimesheetCommand>
{
    public DeleteTimesheetCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}