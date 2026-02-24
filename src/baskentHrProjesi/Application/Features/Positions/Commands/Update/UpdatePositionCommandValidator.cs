using FluentValidation;

namespace Application.Features.Positions.Commands.Update;

public class UpdatePositionCommandValidator : AbstractValidator<UpdatePositionCommand>
{
    public UpdatePositionCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}