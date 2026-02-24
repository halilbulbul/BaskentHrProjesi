using FluentValidation;

namespace Application.Features.LeaveTypes.Commands.Create;

public class CreateLeaveTypeCommandValidator : AbstractValidator<CreateLeaveTypeCommand>
{
    public CreateLeaveTypeCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}