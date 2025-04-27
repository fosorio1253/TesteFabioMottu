using FluentValidation;

namespace Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
public class CreateMotorcycleCommandValidator : AbstractValidator<CreateMotorcycleCommand>
{
    public CreateMotorcycleCommandValidator()
    {
        RuleFor(x => x.Model)
            .NotEmpty().WithMessage("Model is required.")
            .MaximumLength(100).WithMessage("Model must not exceed 100 characters.");

        RuleFor(x => x.Year)
            .GreaterThan(1900).WithMessage("Year must be greater than 1900.")
            .LessThanOrEqualTo(DateTime.Now.Year + 1).WithMessage("Year cannot be greater than next year.");

        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("License plate is required.")
            .Matches(@"^[A-Z]{3}[0-9][0-9A-Z][0-9]{2}$").WithMessage("Invalid license plate format. Use AAA0A00 or AAA0000.");
    }
}