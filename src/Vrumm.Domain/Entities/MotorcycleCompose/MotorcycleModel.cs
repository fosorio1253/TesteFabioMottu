using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class MotorcycleModel
{
    private readonly string _value;

    private MotorcycleModel(string value)
    {
        _value = value;
    }

    public static MotorcycleModel Create(string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Modelo da moto não pode estar vazio");

        return new MotorcycleModel(model);
    }

    public string ToStringRepresentation() => _value;

    public string GetValue() => _value;

    public override bool Equals(object? obj) =>
        obj is MotorcycleModel model && _value == model._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString()
    {
        return $"MotorcycleModel: {_value}";
    }
}