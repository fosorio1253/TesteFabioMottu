using System.Linq.Expressions;
using Vrumm.Domain.Common.Enums;
using Vrumm.Domain.Entities;

namespace Vrumm.Application.Rentals.Queries.GetRentals;
internal static class RentalFilter
{
    public static IQueryable<Rental> ApplyFilters(
        IQueryable<Rental> query,
        Guid? rentalId,
        Guid? motorcycleId,
        Guid? driverId,
        RentalStatus? status,
        DateTime? startDateFrom,
        DateTime? startDateTo)
    {
        if (rentalId.HasValue)
            query = query.Where(r => r.Id == rentalId.Value);

        if (motorcycleId.HasValue)
            query = query.Where(r => r.MotorcycleId == motorcycleId.Value);

        if (driverId.HasValue)
            query = query.Where(r => r.DriverId == driverId.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (startDateFrom.HasValue)
            query = query.Where(r => r.StartDate >= startDateFrom.Value);

        if (startDateTo.HasValue)
            query = query.Where(r => r.StartDate <= startDateTo.Value);

        return query;
    }

    public static IQueryable<Rental> ApplySorting(IQueryable<Rental> query, string sortBy, bool descending)
    {
        Expression<Func<Rental, object>> keySelector = sortBy?.ToLower() switch
        {
            "startdate" => r => r.StartDate,
            "expectedenddate" => r => r.ExpectedEndDate,
            "enddate" => r => r.EndDate,
            "totalvalue" => r => r.TotalValue,
            "status" => r => r.Status,
            "updatedate" => r => r.UpdateDate,
            _ => r => r.CreationDate
        };

        return descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}