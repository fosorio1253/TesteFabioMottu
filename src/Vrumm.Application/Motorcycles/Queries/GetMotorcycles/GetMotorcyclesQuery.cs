using Vrumm.Domain.Common.Enums;

namespace Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
public class GetMotorcyclesQuery
{
    public MotorcycleStatus? Status { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public string Model { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreationDate";
    public bool SortDescending { get; set; } = true;
}