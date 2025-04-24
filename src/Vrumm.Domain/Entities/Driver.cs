using System.Text.RegularExpressions;
using Vrumm.Domain.Commom.Enums;
using Vrumm.Domain.Commom.Exceptions;
using Vrumm.Domain.Commom;

namespace Vrumm.Domain.Entities;
public class Driver : Entity<Guid>
{
    public string Name { get; private set; }
    public string TaxId { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string LicenseNumber { get; private set; }
    public LicenseType LicenseType { get; private set; }
    public string LicenseImagePath { get; private set; }

    private static readonly Regex CpfRegex = new(@"^\d{11}$");

    private Driver() { }

    public Driver(string name, string taxId, DateTime birthDate, string licenseNumber, LicenseType licenseType)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome do entregador não pode estar vazio");

        ValidateTaxId(taxId);
        ValidateBirthDate(birthDate);
        ValidateLicenseNumber(licenseNumber);

        Id = Guid.NewGuid();
        Name = name;
        TaxId = taxId;
        BirthDate = birthDate;
        LicenseNumber = licenseNumber;
        LicenseType = licenseType;
    }

    private void ValidateTaxId(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId))
            throw new DomainException("CPF não pode estar vazio");

        var normalizedTaxId = taxId.Replace(".", "").Replace("-", "").Trim();

        if (!CpfRegex.IsMatch(normalizedTaxId))
            throw new DomainException("CPF inválido");

        TaxId = normalizedTaxId;
    }

    private void ValidateBirthDate(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate > today.AddYears(-age))
            age--;

        if (age < 18)
            throw new DomainException("Entregador deve ter pelo menos 18 anos");

        if (birthDate.Year < 1900)
            throw new DomainException("Data de nascimento inválida");
    }

    private void ValidateLicenseNumber(string licenseNumber)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
            throw new DomainException("Número da CNH não pode estar vazio");
    }

    public void UpdateLicenseImage(string imagePath)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new DomainException("Caminho da imagem da CNH não pode estar vazio");

        LicenseImagePath = imagePath;
        UpdateModificationDate();
    }

    public bool CanRentMotorcycle()
    {
        return LicenseType == LicenseType.A || LicenseType == LicenseType.AB;
    }
}