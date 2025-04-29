using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Domain.Common.Enums;

namespace Vrumm.Application.Motorcycles.Queries.GetMotorcycles;
public class GetMotorcyclesQuery : IQuery<PaginatedList<MotorcycleDto>>
{
    public Guid? MotorcycleId { get; set; }
    public MotorcycleStatus? Status { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public string? Model { get; set; }
    public string? LicensePlate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "CreationDate";
    public bool SortDescending { get; set; } = true;
}