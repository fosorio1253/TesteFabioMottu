namespace Vrumm.Application.Drivers.Dtos;
public class DriverDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string TaxId { get; init; }
    public DateTime BirthDate { get; init; }
    public string LicenseNumber { get; init; }
    public string LicenseType { get; init; }
    public string LicenseImagePath { get; init; }
    public DateTime CreationDate { get; init; }
    public DateTime UpdateDate { get; init; }
    public int Age => CalculateAge(BirthDate, DateTime.Today);

    private int CalculateAge(DateTime birthDate, DateTime currentDate)
    {
        var age = currentDate.Year - birthDate.Year;
        if (birthDate.Date > currentDate.AddYears(-age)) age--;
        return age;
    }
}