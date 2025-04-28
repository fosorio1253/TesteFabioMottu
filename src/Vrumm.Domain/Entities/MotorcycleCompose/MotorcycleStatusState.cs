using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class MotorcycleStatusState
{
    private readonly MotorcycleStatus _status;

    private MotorcycleStatusState(MotorcycleStatus status)
    {
        _status = status;
    }

    public static MotorcycleStatusState Available() => new MotorcycleStatusState(MotorcycleStatus.Available);
    public static MotorcycleStatusState Rented() => new MotorcycleStatusState(MotorcycleStatus.Rented);
    public static MotorcycleStatusState UnderMaintenance() => new MotorcycleStatusState(MotorcycleStatus.UnderMaintenance);
    public static MotorcycleStatusState Inactive() => new MotorcycleStatusState(MotorcycleStatus.Inactive);

    public MotorcycleStatusState TransitionToRent()
    {
        if (_status != MotorcycleStatus.Available)
            throw new MotorcycleNotAvailableException("Moto não está disponível para aluguel.");
        return Rented();
    }

    public MotorcycleStatusState TransitionToReturn()
    {
        if (_status != MotorcycleStatus.Rented)
            throw new DomainException("Esta moto não está alugada.");
        return Available();
    }

    public MotorcycleStatusState TransitionToUnderMaintenance()
    {
        if (_status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível colocar uma moto alugada em manutenção.");
        return UnderMaintenance();
    }

    public MotorcycleStatusState TransitionToInactive()
    {
        if (_status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível inativar uma moto alugada.");
        return Inactive();
    }

    public bool CanBeRemoved() => _status != MotorcycleStatus.Rented && _status != MotorcycleStatus.UnderMaintenance;

    public bool CanBeDeleted(IEnumerable<Rental> rentals) =>
        _status == MotorcycleStatus.Available && (rentals == null || !rentals.Any());

    public MotorcycleStatus ToStatus() => _status;
}