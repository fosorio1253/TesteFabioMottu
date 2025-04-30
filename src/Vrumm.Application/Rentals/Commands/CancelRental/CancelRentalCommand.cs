using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Rentals.Commands.CancelRental;
public class CancelRentalCommand : ICommand
{
    public Guid RentalId { get; set; }
}