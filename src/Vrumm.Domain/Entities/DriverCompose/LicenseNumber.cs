using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.DriverCompose;
public sealed class LicenseNumber
{
    private readonly string _value;

    private LicenseNumber(string value)
    {
        _value = value;
    }

    public static LicenseNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("License number cannot be empty.");

        if (value.Length > 20)
            throw new DomainException("License number must not exceed 20 characters.");

        return new LicenseNumber(value);
    }

    public string Value => _value;

    public override bool Equals(object? obj) =>
        obj is LicenseNumber license && _value == license._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value;
}