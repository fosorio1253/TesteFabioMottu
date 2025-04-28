namespace Vrumm.Application.Common.Exceptions;
public class DuplicateCnpjException : Exception
{
    public DuplicateCnpjException(string taxId)
        : base($"A driver with tax ID \"{taxId}\" already exists.")
    {
    }
}