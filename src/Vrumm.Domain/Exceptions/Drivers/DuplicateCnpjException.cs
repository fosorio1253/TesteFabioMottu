namespace Vrumm.Domain.Exceptions.Drivers;
public class DuplicateCnpjException : Exception
{
    public DuplicateCnpjException(string taxId)
        : base($"A driver with tax ID \"{taxId}\" already exists.")
    {
    }
}