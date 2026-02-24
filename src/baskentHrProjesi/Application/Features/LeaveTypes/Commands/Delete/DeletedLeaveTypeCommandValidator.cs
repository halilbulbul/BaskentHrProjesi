using FluentValidation;

namespace Application.Features.LeaveTypes.Commands.Delete;

public class DeleteLeaveTypeCommandValidator : AbstractValidator<DeleteLeaveTypeCommand>
{
    public DeleteLeaveTypeCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}