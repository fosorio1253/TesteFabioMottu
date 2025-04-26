namespace Vrumm.Domain.Common.Exceptions;
public class InvalidLicensePlateException : DomainException
{
    public InvalidLicensePlateException(string message) : base(message)
    {
    }
}