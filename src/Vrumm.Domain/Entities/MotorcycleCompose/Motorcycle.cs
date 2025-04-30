using Vrumm.Domain.Common;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Events;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public class Motorcycle : Entity<Guid>
{
    private MotorcycleDetails _details;
    private MotorcycleStatusState _statusState;

    private Motorcycle() { }

    public Motorcycle(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate)
    {
        Id = Guid.NewGuid();
        _details = new MotorcycleDetails(year, model, licensePlate);
        _statusState = MotorcycleStatusState.Available();
    }

    public void Update(MotorcycleModel model, ManufactureYear year, LicensePlate licensePlate)
    {
        _details = _details.Update(model, year, licensePlate);
        UpdateModificationDate();
    }

    public void Rent()
    {
        _statusState = _statusState.TransitionToRent();
        UpdateModificationDate();
    }

    public void Return()
    {
        _statusState = _statusState.TransitionToReturn();
        UpdateModificationDate();
    }

    public void SetUnderMaintenance()
    {
        _statusState = _statusState.TransitionToUnderMaintenance();
        UpdateModificationDate();
    }

    public void SetInactive()
    {
        _statusState = _statusState.TransitionToInactive();
        UpdateModificationDate();
    }

    public MotorcycleRegistered GenerateRegisteredEvent()
    {
        return new MotorcycleRegistered(
            Id,
            _details.Model().ToStringRepresentation(),
            _details.Year().ToInt(),
            _details.LicensePlate().ToStringRepresentation()
        );
    }

    public bool CanBeRemoved()
        => _statusState.CanBeRemoved();

    public string StatusToStringRepresentation()
        => _statusState.ToStatus().ToString();

    public MotorcycleStatus Status()
        => _statusState.ToStatus();

    public bool CanBeDeleted(IEnumerable<Rental> rentals)
        => _statusState.CanBeDeleted(rentals);

    public string LicensePlate()
        => _details.LicensePlate().ToStringRepresentation();

    public int Year()
        => _details.Year().ToInt();

    public string Model()
        => _details.Model().ToStringRepresentation();

    public MotorcycleDetails Details()
        => _details;

    public MotorcycleStatusState StatusState()
        => _statusState;
}