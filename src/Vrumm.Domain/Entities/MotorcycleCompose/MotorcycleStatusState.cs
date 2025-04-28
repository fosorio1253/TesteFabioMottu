using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Exceptions.Motorcycles;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities.MotorcycleCompose;
public sealed class MotorcycleStatusState
{
    private readonly MotorcycleStatus _status;

    public MotorcycleStatusState(MotorcycleStatus status)
    {
        _status = status;
    }

    public static MotorcycleStatusState Available() => new MotorcycleStatusState(MotorcycleStatus.Available);

    public MotorcycleStatusState Rent()
    {
        if (_status != MotorcycleStatus.Available)
            throw new MotorcycleNotAvailableException("Moto não está disponível para aluguel");

        return new MotorcycleStatusState(MotorcycleStatus.Rented);
    }

    public MotorcycleStatusState Return()
    {
        if (_status != MotorcycleStatus.Rented)
            throw new DomainException("Esta moto não está alugada");

        return new MotorcycleStatusState(MotorcycleStatus.Available);
    }

    public MotorcycleStatusState SetUnderMaintenance()
    {
        if (_status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível colocar uma moto alugada em manutenção");

        return new MotorcycleStatusState(MotorcycleStatus.UnderMaintenance);
    }

    public MotorcycleStatusState SetInactive()
    {
        if (_status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível inativar uma moto alugada");

        return new MotorcycleStatusState(MotorcycleStatus.Inactive);
    }

    public bool CanBeRemoved() => _status != MotorcycleStatus.Rented || _status != MotorcycleStatus.UnderMaintenance;

    public bool CanBeDeleted(IEnumerable<Rental> rentals) =>
        _status == MotorcycleStatus.Available && (rentals == null || !rentals.Any());

    public MotorcycleStatus ToStatus() => _status;
}