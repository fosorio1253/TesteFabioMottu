namespace Vrumm.Domain.Exceptions.Drivers;
public class InvalidDriverLicenseException : DomainException
{
    public InvalidDriverLicenseException(string message) : base(message)
    {
    }
}