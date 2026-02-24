using FluentValidation;

namespace Application.Features.TimeEntries.Commands.Delete;

public class DeleteTimeEntryCommandValidator : AbstractValidator<DeleteTimeEntryCommand>
{
    public DeleteTimeEntryCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}