using Vrumm.Domain.Commom;

namespace Vrumm.Domain.Events;
public class RentalCreated : DomainEvent
{
    public Guid RentalId { get; }
    public Guid MotorcycleId { get; }
    public Guid DriverId { get; }
    public int PlanId { get; }
    public DateTime StartDate { get; }
    public DateTime ExpectedEndDate { get; }

    public RentalCreated(Guid rentalId, Guid motorcycleId, Guid driverId, int planId, DateTime startDate, DateTime expectedEndDate)
    {
        RentalId = rentalId;
        MotorcycleId = motorcycleId;
        DriverId = driverId;
        PlanId = planId;
        StartDate = startDate;
        ExpectedEndDate = expectedEndDate;
    }
}