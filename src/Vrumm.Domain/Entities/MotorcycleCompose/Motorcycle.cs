using Vrumm.Domain.Common;
using Vrumm.Domain.Events;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public class Motorcycle : Entity<Guid>
{
    private MotorcycleDetails _details;
    private MotorcycleStatusState _status;

    private Motorcycle() { }

    public Motorcycle(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate) : base()
    {
        Id = Guid.NewGuid();
        _details = new MotorcycleDetails(year, model, licensePlate);
        _status = MotorcycleStatusState.Available();
    }

    public void Update(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate)
    {
        _details = _details.Update(model, year, licensePlate);
        UpdateModificationDate();
    }

    public void Rent()
    {
        _status = _status.Rent();
        UpdateModificationDate();
    }

    public void Return()
    {
        _status = _status.Return();
        UpdateModificationDate();
    }

    public void SetUnderMaintenance()
    {
        _status = _status.SetUnderMaintenance();
        UpdateModificationDate();
    }

    public void SetInactive()
    {
        _status = _status.SetInactive();
        UpdateModificationDate();
    }

    public bool CanBeRemoved() => _status.CanBeRemoved();

    public MotorcycleRegistered GenerateRegisteredEvent()
    {
        return new MotorcycleRegistered(
            Id,
            _details.Model().ToStringRepresentation(),
            _details.Year().ToInt(),
            _details.LicensePlate().ToStringRepresentation()
        );
    }

    public bool CanBeDeleted(IEnumerable<Rental> rentals) => _status.CanBeDeleted(rentals);

    public MotorcycleDetails Details() => _details;
    public MotorcycleStatusState Status() => _status;
}