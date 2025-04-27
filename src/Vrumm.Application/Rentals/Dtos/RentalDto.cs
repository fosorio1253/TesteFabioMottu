using Vrumm.Application.Drivers.Dtos;
using Vrumm.Application.Motorcycles.Dtos;
using Vrumm.Application.Plans.Dtos;

namespace Vrumm.Application.Rentals.Dtos;
public class RentalDto
{
    public Guid Id { get; set; }
    public Guid MotorcycleId { get; set; }
    public Guid DriverId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ExpectedEndDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime UpdateDate { get; set; }
    public MotorcycleDto? Motorcycle { get; set; }
    public DriverDto? Driver { get; set; }
    public PlanDto? Plan { get; set; }
}
