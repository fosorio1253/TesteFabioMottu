using System.Text.RegularExpressions;
using Vrumm.Domain.Common;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Common.Exceptions;
using Vrumm.Domain.Events;

namespace Vrumm.Domain.Entities;
public class Motorcycle : Entity<Guid>
{
    public int Year { get; private set; }
    public string Model { get; private set; }
    public string LicensePlate { get; private set; }
    public MotorcycleStatus Status { get; private set; }

    private static readonly Regex BrazilianLicensePlateRegex = new(@"^[A-Z]{3}[0-9][0-9A-Z][0-9]{2}$");

    private Motorcycle() { }

    public Motorcycle(string model, int year, string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new DomainException("Modelo da moto não pode estar vazio");

        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new DomainException("Ano da moto inválido");

        Id = Guid.NewGuid();
        Model = model;
        Year = year;
        Status = MotorcycleStatus.Available;

        SetLicensePlate(licensePlate);
    }

    public void SetLicensePlate(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new InvalidLicensePlateException("Placa não pode estar vazia");

        var normalizedPlate = licensePlate.Trim().ToUpper();

        if (!BrazilianLicensePlateRegex.IsMatch(normalizedPlate))
            throw new InvalidLicensePlateException("Formato de placa inválido");

        LicensePlate = normalizedPlate;
        UpdateModificationDate();
    }

    public void RentMotorcycle()
    {
        if (Status != MotorcycleStatus.Available)
            throw new MotorcycleNotAvailableException($"Moto com placa {LicensePlate} não está disponível para aluguel");

        Status = MotorcycleStatus.Rented;
        UpdateModificationDate();
    }

    public void ReturnMotorcycle()
    {
        if (Status != MotorcycleStatus.Rented)
            throw new DomainException("Esta moto não está alugada");

        Status = MotorcycleStatus.Available;
        UpdateModificationDate();
    }

    public void SetUnderMaintenance()
    {
        if (Status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível colocar uma moto alugada em manutenção");

        Status = MotorcycleStatus.UnderMaintenance;
        UpdateModificationDate();
    }

    public void SetInactive()
    {
        if (Status == MotorcycleStatus.Rented)
            throw new DomainException("Não é possível inativar uma moto alugada");

        Status = MotorcycleStatus.Inactive;
        UpdateModificationDate();
    }

    public bool CanBeRemoved()
    {
        return Status != MotorcycleStatus.Rented;
    }

    public MotorcycleRegistered GenerateRegisteredEvent()
    {
        return new MotorcycleRegistered(Id, Model, Year, LicensePlate);
    }
}