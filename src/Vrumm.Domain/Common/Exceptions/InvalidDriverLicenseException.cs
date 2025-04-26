namespace Vrumm.Domain.Common.Exceptions;
public class InvalidDriverLicenseException : DomainException
{
    public InvalidDriverLicenseException(string message) : base(message)
    {
    }
}