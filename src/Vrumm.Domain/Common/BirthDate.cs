using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Common;
public sealed class BirthDate
{
    private readonly DateTime _value;

    private BirthDate(DateTime value)
    {
        _value = value;
    }

    public static BirthDate Create(DateTime value)
    {
        if (value == default)
            throw new DomainException("Birth date cannot be empty.");

        var today = DateTime.Today;
        var age = today.Year - value.Year;
        if (value > today.AddYears(-age))
            age--;

        if (age < 18)
            throw new DomainException("Driver must be at least 18 years old.");

        if (value.Year < 1900)
            throw new DomainException("Birth date is invalid (too old).");

        return new BirthDate(value);
    }

    public DateTime Value => _value;

    public override bool Equals(object? obj) =>
        obj is BirthDate birthDate && _value == birthDate._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString("yyyy-MM-dd");
}