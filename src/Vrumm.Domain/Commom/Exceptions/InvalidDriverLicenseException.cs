namespace Vrumm.Domain.Commom.Exceptions;
public class InvalidDriverLicenseException : DomainException
{
    public InvalidDriverLicenseException(string message) : base(message)
    {
    }
}