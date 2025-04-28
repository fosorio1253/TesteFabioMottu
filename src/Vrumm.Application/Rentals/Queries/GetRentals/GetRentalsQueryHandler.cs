using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Drivers.Dtos;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Application.Plans.Dtos;
using Vrumm.Application.Rentals.Dtos;
using Vrumm.Domain.Entities;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Rentals.Queries.GetRentals;
public class GetRentalsQueryHandler
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetRentalsQueryHandler> _logger;

    public GetRentalsQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetRentalsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<RentalDto>> Handle(GetRentalsQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Recuperando locações com filtros: MotorcycleId={MotorcycleId}, DriverId={DriverId}, Status={Status}, StartDateFrom={StartDateFrom}, StartDateTo={StartDateTo}",
            query.MotorcycleId, query.DriverId, query.Status, query.StartDateFrom, query.StartDateTo);

        var rentalsQuery = _unitOfWork.Rentals.GetAll();

        rentalsQuery = RentalFilter.ApplyFilters(
            rentalsQuery,
            query.MotorcycleId,
            query.DriverId,
            query.Status,
            query.StartDateFrom,
            query.StartDateTo);

        rentalsQuery = RentalFilter.ApplySorting(rentalsQuery, query.SortBy, query.SortDescending);

        var rentals = await rentalsQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await rentalsQuery.CountAsync(cancellationToken);

        var dtos = await Task.WhenAll(rentals.Select(r => MapToDto(r, query.IncludeRelatedData, cancellationToken)));

        _logger.LogInformation("Recuperadas {Count} locações", dtos.Length);

        return new PaginatedList<RentalDto>(dtos.ToList(), totalCount, query.PageNumber, query.PageSize);
    }

    private async Task<RentalDto> MapToDto(Rental rental, bool includeRelatedData, CancellationToken cancellationToken)
    {
        var dto = new RentalDto
        {
            Id = rental.Id,
            MotorcycleId = rental.MotorcycleId,
            DriverId = rental.DriverId,
            PlanId = rental.PlanId,
            StartDate = rental.StartDate,
            ExpectedEndDate = rental.ExpectedEndDate,
            EndDate = rental.EndDate,
            TotalValue = rental.TotalValue,
            Status = rental.Status.ToString(),
            CreationDate = rental.CreationDate,
            UpdateDate = rental.UpdateDate
        };

        if (includeRelatedData)
        {
            var motorcycle = await _unitOfWork.Motorcycles.GetByIdAsync(rental.MotorcycleId, cancellationToken);
            var driver = await _unitOfWork.Drivers.GetByIdAsync(rental.DriverId, cancellationToken);
            var plan = await _unitOfWork.Plans.GetByIdAsync(rental.PlanId, cancellationToken);

            dto.Motorcycle = motorcycle != null ? new MotorcycleDto
            {
                Id = motorcycle.Id,
                Year = motorcycle.Details().Year().ToInt(),
                Model = motorcycle.Details().Model().ToStringRepresentation(),
                LicensePlate = motorcycle.Details().LicensePlate().ToStringRepresentation(),
                Status = motorcycle.Status().ToStatus().ToString(),
                CreationDate = motorcycle.CreationDate,
                UpdateDate = motorcycle.UpdateDate
            } : null;

            dto.Driver = driver != null ? new DriverDto
            {
                Id = driver.Id,
                Name = driver.Name,
                TaxId = driver.Cnpj.Value,
                BirthDate = driver.BirthDate.Value,
                LicenseNumber = driver.LicenseNumber.Value,
                LicenseType = driver.LicenseType.Value.ToString(),
                LicenseImagePath = driver.LicenseImagePath,
                CreationDate = driver.CreationDate,
                UpdateDate = driver.UpdateDate
            } : null;

            dto.Plan = plan != null ? new PlanDto
            {
                Id = plan.Id,
                DayCount = plan.DayCount,
                DailyRate = plan.DailyRate,
                PenaltyPercentage = plan.PenaltyPercentage,
                CreationDate = plan.CreationDate,
                UpdateDate = plan.UpdateDate
            } : null;
        }

        return dto;
    }
}