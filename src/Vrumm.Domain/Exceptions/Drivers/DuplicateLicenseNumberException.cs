namespace Vrumm.Domain.Exceptions.Drivers; public class DuplicateLicenseNumberException : DomainException
{
    public DuplicateLicenseNumberException(string licenseNumber)
        : base($"A driver with license number {licenseNumber} already exists.")
    {
    }

    public DuplicateLicenseNumberException(string licenseNumber, Exception innerException)
        : base($"A driver with license number {licenseNumber} already exists.", innerException)
    {
    }
}