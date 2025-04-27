using Vrumm.Application.Common.Interfaces;
using Vrumm.Application.Common.Models;
using Vrumm.Application.Drivers.Dtos;

namespace Vrumm.Application.Drivers.Queries.GetDrivers;
public class GetDriversQuery : IQuery<PaginatedList<DriverDto>>
{
    public DriverFilter Filter { get; init; } = new DriverFilter();
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "CreationDate";
    public bool SortDescending { get; init; } = true;
}