using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class ManufactureYear
{
    private readonly int _value;

    private ManufactureYear(int value)
    {
        _value = value;
    }

    public static ManufactureYear Create(int year)
    {
        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new DomainException("Ano da moto inválido");

        return new ManufactureYear(year);
    }

    public int ToInt() => _value;

    public override bool Equals(object? obj) =>
        obj is ManufactureYear year && _value == year._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => _value.ToString();
}