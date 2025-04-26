using Vrumm.Domain.Common;

namespace Vrumm.Domain.Events;
public class MotorcycleRegistered : DomainEvent
{
    public Guid MotorcycleId { get; }
    public string Model { get; }
    public int Year { get; }
    public string LicensePlate { get; }

    public MotorcycleRegistered(Guid motorcycleId, string model, int year, string licensePlate)
    {
        MotorcycleId = motorcycleId;
        Model = model;
        Year = year;
        LicensePlate = licensePlate;
    }
}