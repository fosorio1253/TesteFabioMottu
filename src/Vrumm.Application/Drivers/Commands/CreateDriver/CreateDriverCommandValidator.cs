using FluentValidation;
using Vrumm.Domain.Common;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Application.Drivers.Commands.CreateDriver;
public class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.LicenseImageBase64)
            .NotEmpty().WithMessage("License image is required.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ is required.")
            .Must(BeValidCnpj).WithMessage("CNPJ must be a valid 14-digit number with correct check digits.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Birth date is required.")
            .Must(BeValidBirthDate).WithMessage("Driver must be at least 18 years old or birth date is invalid.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .Must(BeValidLicenseNumber).WithMessage("License number must not exceed 20 characters.");

        RuleFor(x => x.LicenseType)
            .NotEmpty().WithMessage("License type is required.")
            .Must(BeValidLicenseType).WithMessage("License type must be A, B, or AB.");
    }

    private bool BeValidCnpj(string cnpj)
    {
        try
        {
            Cnpj.Create(cnpj);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    private bool BeValidBirthDate(DateTime birthDate)
    {
        try
        {
            BirthDate.Create(birthDate);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    private bool BeValidLicenseNumber(string licenseNumber)
    {
        try
        {
            LicenseNumber.Create(licenseNumber);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }

    private bool BeValidLicenseType(string licenseType)
    {
        try
        {
            LicenseTypeValue.Create(licenseType);
            return true;
        }
        catch (DomainException)
        {
            return false;
        }
    }
}