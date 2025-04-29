using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Motorcycles.Commands.UpdateMotorcycle;
public class UpdateMotorcycleCommand : ICommand
{
    public Guid Id { get; set; }
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}