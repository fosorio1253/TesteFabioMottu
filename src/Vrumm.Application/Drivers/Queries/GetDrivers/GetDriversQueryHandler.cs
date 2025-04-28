using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Drivers.Dtos;
using Vrumm.Application.Drivers.Mappings;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.DriverCompose;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Drivers.Queries.GetDrivers;
public class GetDriversQueryHandler : IQueryHandler<GetDriversQuery, PaginatedList<DriverDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDriversQueryHandler> _logger;

    public GetDriversQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetDriversQueryHandler> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaginatedList<DriverDto>> Handle(GetDriversQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving drivers with page {PageNumber}, size {PageSize}", query.PageNumber, query.PageSize);

        var queryable = _unitOfWork.Drivers.GetAll();

        if (!string.IsNullOrWhiteSpace(query.Filter.Name))
            queryable = queryable.Where(d => d.Name.Contains(query.Filter.Name));

        if (!string.IsNullOrWhiteSpace(query.Filter.TaxId))
            queryable = queryable.Where(d => d.Cnpj.Value == query.Filter.TaxId);

        if (!string.IsNullOrWhiteSpace(query.Filter.LicenseType))
            queryable = queryable.Where(d => d.LicenseType.Value
            == Enum.Parse<LicenseType>(query.Filter.LicenseType));

        queryable = ApplySorting(queryable, query.SortBy, query.SortDescending);

        var drivers = await queryable
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await queryable.CountAsync(cancellationToken);
        var driverDtos = drivers.Select(DriverMapper.ToDto).ToList().AsReadOnly();

        var paginatedList = new PaginatedList<DriverDto>(driverDtos, totalCount, query.PageNumber, query.PageSize);

        _logger.LogInformation("Retrieved {DriverCount} drivers for page {PageNumber}", driverDtos.Count, query.PageNumber.ToString());

        return paginatedList;
    }

    private IQueryable<Driver> ApplySorting(IQueryable<Driver> query, string sortBy, bool descending)
    {
        Expression<Func<Driver, object>> keySelector = sortBy?.ToLower() switch
        {
            "name" => d => d.Name,
            "taxid" => d => d.Cnpj.Value,
            "birthdate" => d => d.BirthDate,
            "licensenumber" => d => d.LicenseNumber,
            "licensetype" => d => d.LicenseType,
            "updatedate" => d => d.UpdateDate,
            _ => d => d.CreationDate // default
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}