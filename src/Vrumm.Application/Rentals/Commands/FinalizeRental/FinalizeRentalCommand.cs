namespace Vrumm.Application.Rentals.Commands.FinalizeRental;
public class FinalizeRentalCommand
{
    public Guid RentalId { get; set; }
    public DateTime ReturnDate { get; set; }
}