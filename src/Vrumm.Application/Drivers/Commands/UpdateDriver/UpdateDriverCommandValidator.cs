using FluentValidation;

namespace Vrumm.Application.Drivers.Commands.UpdateDriver;
public class UpdateDriverCommandValidator : AbstractValidator<UpdateDriverCommand>
{
    public UpdateDriverCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty).WithMessage("O ID do entregador não pode ser vazio.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome não pode exceder 100 caracteres.");

        RuleFor(x => x.TaxId)
            .NotEmpty().WithMessage("O CPF é obrigatório.")
            .Length(11).WithMessage("O CPF deve ter 11 dígitos.");

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.UtcNow).WithMessage("A data de nascimento não pode ser futura.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("O número da licença é obrigatório.")
            .MaximumLength(20).WithMessage("O número da licença não pode exceder 20 caracteres.");

        RuleFor(x => x.LicenseType)
            .IsInEnum().WithMessage("O tipo de licença é inválido.");
    }
}