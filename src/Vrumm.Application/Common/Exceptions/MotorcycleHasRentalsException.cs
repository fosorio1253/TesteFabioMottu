namespace Vrumm.Application.Common.Exceptions;
public class MotorcycleHasRentalsException : Exception
{
    public MotorcycleHasRentalsException()
        : base("Cannot delete motorcycle with rental history")
    {
    }

    public MotorcycleHasRentalsException(string message)
        : base(message)
    {
    }

    public MotorcycleHasRentalsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public MotorcycleHasRentalsException(Guid motorcycleId)
        : base($"Cannot delete motorcycle with ID {motorcycleId} because it has rental history")
    {
    }
}