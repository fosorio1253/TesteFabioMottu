using System.Linq.Expressions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

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
            query = query.Where(m => m.Status == status.Value);

        if (yearFrom.HasValue)
            query = query.Where(m => m.Year >= yearFrom.Value);

        if (yearTo.HasValue)
            query = query.Where(m => m.Year <= yearTo.Value);

        if (!string.IsNullOrWhiteSpace(model))
            query = query.Where(m => m.Model.Contains(model));

        if (!string.IsNullOrWhiteSpace(licensePlate))
            query = query.Where(m => m.LicensePlate.Contains(licensePlate));

        return query;
    }

    public static IQueryable<Motorcycle> ApplySorting(IQueryable<Motorcycle> query, string sortBy, bool descending)
    {
        Expression<Func<Motorcycle, object>> keySelector = sortBy?.ToLower() switch
        {
            "year" => m => m.Year,
            "model" => m => m.Model,
            "licenseplate" => m => m.LicensePlate,
            "status" => m => m.Status,
            "updatedate" => m => m.UpdateDate,
            _ => m => m.CreationDate // Default
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}