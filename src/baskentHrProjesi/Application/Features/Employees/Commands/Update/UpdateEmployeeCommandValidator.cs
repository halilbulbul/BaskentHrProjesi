using FluentValidation;

namespace Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.EmployeeNumber).NotEmpty();
        RuleFor(c => c.IdentityNo).NotEmpty();
        RuleFor(c => c.FirstName).NotEmpty();
        RuleFor(c => c.LastName).NotEmpty();
        RuleFor(c => c.Gender).NotEmpty();
        RuleFor(c => c.MaritalStatus).NotEmpty();
        RuleFor(c => c.PhoneNumber).NotEmpty();
        RuleFor(c => c.Email).NotEmpty();
        RuleFor(c => c.AddressLine).NotEmpty();
        RuleFor(c => c.City).NotEmpty();
        RuleFor(c => c.District).NotEmpty();
        RuleFor(c => c.PostalCode).NotEmpty();
        RuleFor(c => c.HireDate).NotEmpty();
        RuleFor(c => c.DepartmentId).NotEmpty();
    }
}