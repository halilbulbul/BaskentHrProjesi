using FluentValidation;

namespace Application.Features.EmployeeLeaves.Commands.Create;

public class CreateEmployeeLeaveCommandValidator : AbstractValidator<CreateEmployeeLeaveCommand>
{
    public CreateEmployeeLeaveCommandValidator()
    {

    }
}