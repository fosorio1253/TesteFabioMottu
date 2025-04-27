namespace Vrumm.Application.Common.Exceptions;
public class DuplicateTaxIdException : Exception
{
    public DuplicateTaxIdException(string taxId)
        : base($"A driver with tax ID \"{taxId}\" already exists.")
    {
    }
}