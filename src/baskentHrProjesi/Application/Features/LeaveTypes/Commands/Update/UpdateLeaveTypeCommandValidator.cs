using FluentValidation;

namespace Application.Features.LeaveTypes.Commands.Update;

public class UpdateLeaveTypeCommandValidator : AbstractValidator<UpdateLeaveTypeCommand>
{
    public UpdateLeaveTypeCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.IsPaid).NotEmpty();
    }
}