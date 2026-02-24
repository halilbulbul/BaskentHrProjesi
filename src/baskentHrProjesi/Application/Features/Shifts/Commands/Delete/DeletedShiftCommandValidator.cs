using FluentValidation;

namespace Application.Features.Shifts.Commands.Delete;

public class DeleteShiftCommandValidator : AbstractValidator<DeleteShiftCommand>
{
    public DeleteShiftCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}