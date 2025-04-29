using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Domain.Entities.MotorcycleCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
public class GetMotorcyclesQueryHandler : IQueryHandler<GetMotorcyclesQuery, PaginatedList<MotorcycleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetMotorcyclesQueryHandler> _logger;

    public GetMotorcyclesQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetMotorcyclesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<MotorcycleDto>> Handle(GetMotorcyclesQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving motorcycles with filters: Status={Status}, YearFrom={YearFrom}, YearTo={YearTo}, Model={Model}, LicensePlate={LicensePlate}",
            query.Status, query.YearFrom, query.YearTo, query.Model, query.LicensePlate);

        var motorcyclesQuery = await _unitOfWork.Motorcycles.GetQueryAsync(cancellationToken);

        motorcyclesQuery = MotorcycleFilter.ApplyFilters(
            motorcyclesQuery,
            query.MotorcycleId,
            query.Status,
            query.YearFrom,
            query.YearTo,
            query.Model,
            query.LicensePlate);

        motorcyclesQuery = MotorcycleFilter.ApplySorting(motorcyclesQuery, query.SortBy, query.SortDescending);

        var motorcycles = await motorcyclesQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await motorcyclesQuery.CountAsync(cancellationToken);

        var dtos = motorcycles.Select(MapToDto).ToList();

        _logger.LogInformation("Retrieved {Count} motorcycles", dtos.Count);

        return new PaginatedList<MotorcycleDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }

    private MotorcycleDto MapToDto(Motorcycle motorcycle)
    {
        return new MotorcycleDto
        {
            Id = motorcycle.Id,
            Year = motorcycle.Details().Year().ToInt(),
            Model = motorcycle.Details().Model().ToStringRepresentation(),
            LicensePlate = motorcycle.Details().LicensePlate().ToStringRepresentation(),
            Status = motorcycle.Status().ToStatus().ToString(),
            CreationDate = motorcycle.CreationDate,
            UpdateDate = motorcycle.UpdateDate
        };
    }
}