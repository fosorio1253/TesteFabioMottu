namespace Vrumm.Application.Motorcycles.Dtos;
public class MotorcycleDto
{
    public Guid Id { get; set; }
    public int Year { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
}