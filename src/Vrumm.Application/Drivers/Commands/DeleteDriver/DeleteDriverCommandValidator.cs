using FluentValidation;

namespace Vrumm.Application.Drivers.Commands.DeleteDriver;
public class DeleteDriverCommandValidator : AbstractValidator<DeleteDriverCommand>
{
    public DeleteDriverCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("O ID do entregador não pode ser vazio.");
    }
}