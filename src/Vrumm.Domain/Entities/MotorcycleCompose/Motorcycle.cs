using Vrumm.Domain.Common;
using Vrumm.Domain.Events;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public class Motorcycle : Entity<Guid>
{
    private MotorcycleDetails _details;
    private MotorcycleStatusState _status;

    private Motorcycle() { }

    public Motorcycle(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate)
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
        _status = _status.TransitionToRent();
        UpdateModificationDate();
    }

    public void Return()
    {
        _status = _status.TransitionToReturn();
        UpdateModificationDate();
    }

    public void SetUnderMaintenance()
    {
        _status = _status.TransitionToUnderMaintenance();
        UpdateModificationDate();
    }

    public void SetInactive()
    {
        _status = _status.TransitionToInactive();
        UpdateModificationDate();
    }

    public bool CanBeRemoved() => _status.CanBeRemoved();

    public bool CanBeDeleted(IEnumerable<Rental> rentals) => _status.CanBeDeleted(rentals);

    public MotorcycleDetails Details() => _details;
    public MotorcycleStatusState Status() => _status;

    public MotorcycleRegistered GenerateRegisteredEvent()
    {
        return new MotorcycleRegistered(
            Id,
            _details.Model().ToStringRepresentation(),
            _details.Year().ToInt(),
            _details.LicensePlate().ToStringRepresentation()
        );
    }
}