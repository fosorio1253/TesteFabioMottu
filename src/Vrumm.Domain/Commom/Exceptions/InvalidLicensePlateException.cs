namespace Vrumm.Domain.Commom.Exceptions;
public class InvalidLicensePlateException : DomainException
{
    public InvalidLicensePlateException(string message) : base(message)
    {
    }
}