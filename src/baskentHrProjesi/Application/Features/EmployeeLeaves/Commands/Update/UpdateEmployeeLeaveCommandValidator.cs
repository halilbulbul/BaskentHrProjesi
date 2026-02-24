using FluentValidation;

namespace Application.Features.EmployeeLeaves.Commands.Update;

public class UpdateEmployeeLeaveCommandValidator : AbstractValidator<UpdateEmployeeLeaveCommand>
{
    public UpdateEmployeeLeaveCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.EmployeeId).NotEmpty();
        RuleFor(c => c.LeaveTypeId).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.EndDate).NotEmpty();
        RuleFor(c => c.TotalDays).NotEmpty();
        RuleFor(c => c.ApprovalStatus).NotEmpty();
        RuleFor(c => c.RequestDate).NotEmpty();
    }
}