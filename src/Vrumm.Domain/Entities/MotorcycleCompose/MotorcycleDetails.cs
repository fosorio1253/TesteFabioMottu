namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class MotorcycleDetails
{
    private readonly ManufactureYear _year;
    private readonly MotorcycleModel _model;
    private readonly LicensePlate _licensePlate;

    public MotorcycleDetails(ManufactureYear year, MotorcycleModel model, LicensePlate licensePlate)
    {
        _year = year;
        _model = model;
        _licensePlate = licensePlate;
    }

    public ManufactureYear Year() => _year;
    public MotorcycleModel Model() => _model;
    public LicensePlate LicensePlate() => _licensePlate;

    public MotorcycleDetails Update(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate)
    {
        return new MotorcycleDetails(year, model, licensePlate);
    }
}