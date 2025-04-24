namespace Vrumm.Domain.Commom.Exceptions;
public class MotorcycleNotAvailableException : DomainException
{
    public MotorcycleNotAvailableException(string message) : base(message)
    {
    }
}