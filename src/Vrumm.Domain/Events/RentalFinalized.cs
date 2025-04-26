using Vrumm.Domain.Common;

namespace Vrumm.Domain.Events;
public class RentalFinalized : DomainEvent
{
    public Guid RentalId { get; }
    public Guid MotorcycleId { get; }
    public Guid DriverId { get; }
    public DateTime EndDate { get; }
    public decimal TotalValue { get; }

    public RentalFinalized(Guid rentalId, Guid motorcycleId, Guid driverId, DateTime endDate, decimal totalValue)
    {
        RentalId = rentalId;
        MotorcycleId = motorcycleId;
        DriverId = driverId;
        EndDate = endDate;
        TotalValue = totalValue;
    }
}