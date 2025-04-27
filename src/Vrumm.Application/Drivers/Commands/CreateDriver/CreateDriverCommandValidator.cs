using FluentValidation;

namespace Vrumm.Application.Drivers.Commands.CreateDriver;
public class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("Tax ID is required.")
            .Matches(@"^\d{11}$").WithMessage("Tax ID must be an 11-digit number.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .LessThanOrEqualTo(DateTime.Today.AddYears(-18)).WithMessage("Driver must be at least 18 years old.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .MaximumLength(20).WithMessage("License number must not exceed 20 characters.");

        RuleFor(x => x.LicenseType)
            .NotEmpty().WithMessage("License type is required.")
            .Must(type => type is "A" or "B" or "AB").WithMessage("License type must be A, B, or AB.");
    }
}