namespace Vrumm.Domain.Exceptions.Motorcycles;
public class DuplicateLicensePlateException : DomainException
{
    public string LicensePlate { get; }

    public DuplicateLicensePlateException(string licensePlate)
        : base($"Já existe uma moto com a placa {licensePlate}")
    {
        LicensePlate = licensePlate;
    }

    public DuplicateLicensePlateException(string licensePlate, Exception innerException)
        : base($"Já existe uma moto com a placa {licensePlate}", innerException)
    {
        LicensePlate = licensePlate;
    }
}