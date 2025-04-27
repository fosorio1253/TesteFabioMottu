namespace Vrumm.Application.Motorcycles.Commands.CreateMotorcycle;
public class CreateMotorcycleCommand
{
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
}