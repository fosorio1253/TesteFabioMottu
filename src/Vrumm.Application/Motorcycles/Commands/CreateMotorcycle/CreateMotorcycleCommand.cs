using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
public class CreateMotorcycleCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string LicensePlate { get; set; }
}