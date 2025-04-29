using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Rentals.Commands.FinalizeRental;
public class FinalizeRentalCommand : ICommand
{
    public Guid RentalId { get; set; }
    public DateTime ReturnDate { get; set; }
}