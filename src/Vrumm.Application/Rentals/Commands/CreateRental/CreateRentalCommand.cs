namespace Vrumm.Application.Rentals.Commands.CreateRental;
public class CreateRentalCommand
{
    public Guid MotorcycleId { get; set; }
    public Guid DriverId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDate { get; set; }
}