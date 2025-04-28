using System.Text.RegularExpressions;
using Vrumm.Domain.Exceptions.Motorcycles;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class LicensePlate
{
    private readonly string _value;
    private static readonly Regex BrazilianLicensePlateRegex = new(@"^[A-Z]{3}[0-9][0-9A-Z][0-9]{2}$");

    private LicensePlate(string value)
    {
        _value = value;
    }

    public static LicensePlate Create(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new InvalidLicensePlateException("Placa não pode estar vazia");

        var normalizedPlate = licensePlate.Trim().ToUpper();

        if (!BrazilianLicensePlateRegex.IsMatch(normalizedPlate))
            throw new InvalidLicensePlateException("Formato de placa inválido");

        return new LicensePlate(normalizedPlate);
    }

    public string ToStringRepresentation() => _value; // Para uso seguro externo (por ex.: DTO, Log)

    public string GetValue() => _value; // Alternativa para obter o valor controladamente

    public override bool Equals(object? obj) =>
        obj is LicensePlate plate && _value == plate._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString()
    {
        return $"LicensePlate: {_value}"; // Apenas debug interno, não expor valor puro.
    }
}