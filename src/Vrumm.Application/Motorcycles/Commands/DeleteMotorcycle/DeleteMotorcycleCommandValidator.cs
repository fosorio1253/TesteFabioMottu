using FluentValidation;

namespace Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
public class DeleteMotorcycleCommandValidator : AbstractValidator<DeleteMotorcycleCommand>
{
    public DeleteMotorcycleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Motorcycle ID is required.");
    }
}