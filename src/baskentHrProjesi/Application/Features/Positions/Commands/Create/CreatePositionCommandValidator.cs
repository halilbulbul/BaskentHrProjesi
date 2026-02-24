using FluentValidation;

namespace Application.Features.Positions.Commands.Create;

public class CreatePositionCommandValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}