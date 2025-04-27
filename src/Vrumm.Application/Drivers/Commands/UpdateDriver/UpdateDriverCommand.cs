using Vrumm.Domain.Common.Enums;

namespace Vrumm.Application.Drivers.Commands.UpdateDriver;
public class UpdateDriverCommand
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string TaxId { get; set; }
    public DateTime BirthDate { get; set; }
    public string LicenseNumber { get; set; }
    public LicenseType LicenseType { get; set; }
}