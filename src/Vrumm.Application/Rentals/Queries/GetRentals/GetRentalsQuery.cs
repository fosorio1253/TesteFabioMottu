using Vrumm.Domain.Common.Enums;

namespace Vrumm.Application.Rentals.Queries.GetRentals;
public class GetRentalsQuery
{
    public Guid? MotorcycleId { get; set; }
    public Guid? DriverId { get; set; }
    public RentalStatus? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreationDate";
    public bool SortDescending { get; set; } = true;
    public bool IncludeRelatedData { get; set; }
}