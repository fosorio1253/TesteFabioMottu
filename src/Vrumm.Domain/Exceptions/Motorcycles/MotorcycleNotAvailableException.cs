namespace Vrumm.Domain.Exceptions.Motorcycles;
public class MotorcycleNotAvailableException : DomainException
{
    public MotorcycleNotAvailableException(string message) : base(message)
    {
    }
}