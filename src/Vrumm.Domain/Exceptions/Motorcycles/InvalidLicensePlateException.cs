namespace Vrumm.Domain.Exceptions.Motorcycles;
public class InvalidLicensePlateException : DomainException
{
    public InvalidLicensePlateException(string message) : base(message)
    {
    }
}