using FluentValidation;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Rentals.Commands.CreateRental;
public class CreateRentalCommandValidator : AbstractValidator<CreateRentalCommand>
{
    private readonly IDateTime _dateTime;

    public CreateRentalCommandValidator(IDateTime dateTime)
    {
        _dateTime = dateTime;

        RuleFor(x => x.MotorcycleId)
            .NotEmpty().WithMessage("ID da motocicleta é obrigatório.");

        RuleFor(x => x.DriverId)
            .NotEmpty().WithMessage("ID do entregador é obrigatório.");

        RuleFor(x => x.PlanId)
            .GreaterThan(0).WithMessage("ID do plano inválido.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data de início é obrigatória.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Data de início não pode ser no passado.");

        RuleFor(x => x.StartDate)
            .Must(BeNextDay)
            .WithMessage("A data de início da locação deve ser exatamente o dia seguinte à data atual.");
    }

    private bool BeNextDay(DateTime startDate)
    {
        DateTime expectedStartDate = _dateTime.Now.Date.AddDays(1);
        return startDate.Date == expectedStartDate;
    }
}