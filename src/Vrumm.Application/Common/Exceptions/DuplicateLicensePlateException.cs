namespace Vrumm.Application.Common.Exceptions;
public class DuplicateLicensePlateException : Exception
{
    public DuplicateLicensePlateException(string licensePlate)
        : base($"A motorcycle with license plate \"{licensePlate}\" already exists.")
    {
    }
}