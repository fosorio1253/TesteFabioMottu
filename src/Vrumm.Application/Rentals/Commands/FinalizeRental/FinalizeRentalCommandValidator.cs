using FluentValidation;

namespace Vrumm.Application.Rentals.Commands.FinalizeRental;
public class FinalizeRentalCommandValidator : AbstractValidator<FinalizeRentalCommand>
{
    public FinalizeRentalCommandValidator()
    {
        RuleFor(x => x.RentalId)
            .NotEmpty().WithMessage("ID da locação é obrigatório.");

        RuleFor(x => x.ReturnDate)
            .NotEmpty().WithMessage("Data de devolução é obrigatória.")
            .GreaterThanOrEqualTo(DateTime.Today.AddDays(-1)).WithMessage("Data de devolução não pode ser anterior a ontem.");
    }
}