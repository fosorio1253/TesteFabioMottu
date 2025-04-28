namespace Vrumm.Domain.Exceptions.Drivers;
public class DuplicateDriverException : DomainException
{
    public DuplicateDriverException(string cnpj)
        : base($"A driver with CNPJ {cnpj} already exists.")
    {
    }

    public DuplicateDriverException(string cnpj, Exception innerException)
        : base($"A driver with CNPJ {cnpj} already exists.", innerException)
    {
    }
}