using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities.MotorcycleCompose;

namespace Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
internal static class MotorcycleFilter
{
    public static IQueryable<Motorcycle> ApplyFilters(
        IQueryable<Motorcycle> query,
        MotorcycleStatus? status,
        int? yearFrom,
        int? yearTo,
        string model,
        string licensePlate)
    {
        if (status.HasValue)
            query = query.Where(m => m.Status().ToStatus() == status.Value);

        if (yearFrom.HasValue)
            query = query.Where(m => m.Details().Year().ToInt() >= yearFrom.Value);

        if (yearTo.HasValue)
            query = query.Where(m => m.Details().Year().ToInt() <= yearTo.Value);

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(m => m.Details().Model().ToStringRepresentation().Contains(model));

        if (!string.IsNullOrWhiteSpace(licensePlate))
            query = query.Where(m => m.Details().LicensePlate().ToStringRepresentation().Contains(licensePlate));

        return query;
    }

    public static IQueryable<Motorcycle> ApplySorting(IQueryable<Motorcycle> query, string sortBy, bool descending)
    {
        Expression<Func<Motorcycle, object>> keySelector = sortBy?.ToLower() switch
        {
            "year" => m => EF.Property<int>(m, "Year"),
            "model" => m => EF.Property<string>(m, "Model"),
            "licenseplate" => m => EF.Property<string>(m, "LicensePlate"),
            "status" => m => EF.Property<MotorcycleStatus>(m, "Status"),
            "updatedate" => m => EF.Property<DateTime>(m, "ModifiedAt"),
            _ => m => EF.Property<DateTime>(m, "CreatedAt") // Default
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}