using FluentValidation;

namespace Vrumm.Application.Rentals.Commands.CancelRental;
public class CancelRentalCommandValidator : AbstractValidator<CancelRentalCommand>
{
    public CancelRentalCommandValidator()
    {
        RuleFor(x => x.RentalId)
            .NotEmpty().WithMessage("ID da locação é obrigatório.");
    }
}