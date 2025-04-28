using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.DriverCompose;
public sealed class LicenseTypeValue
{
    private readonly LicenseType _value;

    private LicenseTypeValue(LicenseType value)
    {
        _value = value;
    }

    public static LicenseTypeValue Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("License type cannot be empty.");

        if (!Enum.TryParse<LicenseType>(value, true, out var licenseType) ||
            licenseType != LicenseType.A && licenseType != LicenseType.B && licenseType != LicenseType.AB)
        {
            throw new DomainException("License type must be A, B, or AB.");
        }

        return new LicenseTypeValue(licenseType);
    }

    public static LicenseTypeValue Create(LicenseType value)
    {
        return new LicenseTypeValue(value);
    }

    public LicenseType Value => _value;

    public override bool Equals(object? obj) =>
        obj is LicenseTypeValue licenseType && _value == licenseType._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString();
}