using Vrumm.Domain.Common;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Events;
using Vrumm.Domain.Exceptions;

namespace Vrumm.Domain.Entities;
public class Rental : Entity<Guid>
{
    public Guid MotorcycleId { get; private set; }
    public Guid DriverId { get; private set; }
    public int PlanId { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime ExpectedEndDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public decimal? TotalValue { get; private set; }
    public RentalStatus Status { get; private set; }

    private bool _createdEventEmitted;
    private bool _finalizedEventEmitted;

    private Rental() { }

    public Rental(Guid motorcycleId, Guid driverId, int planId, DateTime startDate, DateTime expectedEndDate)
    {
        if (motorcycleId == Guid.Empty)
            throw new DomainException("ID da moto inválido");

        if (driverId == Guid.Empty)
            throw new DomainException("ID do entregador inválido");

        if (startDate.Date < DateTime.Today)
            throw new DomainException("Data de início não pode ser no passado");

        if (expectedEndDate.Date < startDate.Date)
            throw new DomainException("Data de término esperada deve ser posterior à data de início");

        Id = Guid.NewGuid();
        MotorcycleId = motorcycleId;
        DriverId = driverId;
        PlanId = planId;
        StartDate = startDate.Date;
        ExpectedEndDate = expectedEndDate.Date;
        Status = RentalStatus.Active;

        _createdEventEmitted = false;
        _finalizedEventEmitted = false;
    }

    public decimal CalculateReturnValue(DateTime returnDate, Plan plan)
    {
        if (Status != RentalStatus.Active)
            throw new DomainException("Não é possível calcular valor para uma locação não ativa");

        if (returnDate.Date == ExpectedEndDate.Date)
            return plan.CalculateTotalValue();

        var actualDays = (int)(returnDate.Date - StartDate.Date).TotalDays + 1;

        if (returnDate.Date < ExpectedEndDate.Date)
        {
            var baseValue = actualDays * plan.DailyRate;
            var penalty = plan.CalculateEarlyReturnPenalty(actualDays);
            return baseValue + penalty;
        }

        var extraDays = (int)(returnDate.Date - ExpectedEndDate.Date).TotalDays;
        var normalValue = plan.CalculateTotalValue();
        var additionalValue = plan.CalculateAdditionalDaysValue(extraDays);

        return normalValue + additionalValue;
    }

    public void FinalizeRental(DateTime returnDate, decimal totalValue)
    {
        if (Status != RentalStatus.Active)
            throw new DomainException("Não é possível finalizar uma locação não ativa");

        if (returnDate.Date < StartDate.Date)
            throw new DomainException("Data de devolução não pode ser anterior à data de início");

        if (totalValue < 0)
            throw new DomainException("Valor total não pode ser negativo");

        EndDate = returnDate.Date;
        TotalValue = totalValue;
        Status = RentalStatus.Finalized;
        UpdateModificationDate();
    }

    public void CancelRental()
    {
        if (Status != RentalStatus.Active)
            throw new DomainException("Não é possível cancelar uma locação não ativa");

        Status = RentalStatus.Cancelled;
        UpdateModificationDate();
    }

    public RentalCreated GenerateCreatedEvent()
    {
        if (_createdEventEmitted)
            throw new DomainException("Evento de criação já foi emitido para esta locação.");

        _createdEventEmitted = true;

        return new RentalCreated(Id, MotorcycleId, DriverId, PlanId, StartDate, ExpectedEndDate);
    }

    public RentalFinalized GenerateFinalizedEvent()
    {
        if (Status != RentalStatus.Finalized || !EndDate.HasValue || !TotalValue.HasValue)
            throw new DomainException("Locação não está finalizada corretamente");

        if (_finalizedEventEmitted)
            throw new DomainException("Evento de finalização já foi emitido para esta locação.");

        _finalizedEventEmitted = true;

        return new RentalFinalized(Id, MotorcycleId, DriverId, EndDate.Value, TotalValue.Value);
    }

    public static Rental CreateNextDayRental(Guid motorcycleId, Guid driverId, int planId, DateTime creationDate, Plan plan)
    {
        DateTime startDate = creationDate.Date.AddDays(1);
        DateTime expectedEndDate = startDate.AddDays(plan.DayCount - 1);

        return new Rental(motorcycleId, driverId, planId, startDate, expectedEndDate);
    }
}