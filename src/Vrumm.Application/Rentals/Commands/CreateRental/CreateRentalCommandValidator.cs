using FluentValidation;

namespace Vrumm.Application.Rentals.Commands.CreateRental;
public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalCommandValidator()
    {
        RuleFor(x => x.MotorcycleId)
            .NotEmpty().WithMessage("ID da motocicleta é obrigatório.");

        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("ID do entregador é obrigatório.");

        RuleFor(x => x.PlanId)
            .GreaterThan(0).WithMessage("ID do plano inválido.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data de início é obrigatória.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Data de início não pode ser no passado.");
    }
}