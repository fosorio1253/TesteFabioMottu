namespace Vrumm.Application.Motorcycles.Dtos;
public class MotorcycleRegistrationEventDto
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public int Year { get; set; }
    public string Model { get; set; }
    public string LicensePlate { get; set; }
    public DateTime EventTimestamp { get; set; }
    public DateTime ProcessedAt { get; set; }
}