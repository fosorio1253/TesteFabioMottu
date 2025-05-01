using Vrumm.Domain.Common;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public class MotorcycleRegistrationEvent : Entity<Guid>
{
    public Guid MotorcycleId { get; private set; }
    public int Year { get; private set; }
    public string Model { get; private set; }
    public string LicensePlate { get; private set; }
    public DateTime EventTimestamp { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    private MotorcycleRegistrationEvent() { }

    public MotorcycleRegistrationEvent(
        Guid motorcycleId,
        int year,
        string model,
        string licensePlate,
        DateTime eventTimestamp)
    {
        Id = Guid.NewGuid();
        MotorcycleId = motorcycleId;
        Year = year;
        Model = model;
        LicensePlate = licensePlate;
        EventTimestamp = eventTimestamp;
        ProcessedAt = DateTime.UtcNow;
    }
}