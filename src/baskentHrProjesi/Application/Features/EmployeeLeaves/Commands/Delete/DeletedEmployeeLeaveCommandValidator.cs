using FluentValidation;

namespace Application.Features.EmployeeLeaves.Commands.Delete;

public class DeleteEmployeeLeaveCommandValidator : AbstractValidator<DeleteEmployeeLeaveCommand>
{
    public DeleteEmployeeLeaveCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}