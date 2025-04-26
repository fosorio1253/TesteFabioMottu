namespace Vrumm.Domain.Common.Exceptions;
public class MotorcycleNotAvailableException : DomainException
{
    public MotorcycleNotAvailableException(string message) : base(message)
    {
    }
}