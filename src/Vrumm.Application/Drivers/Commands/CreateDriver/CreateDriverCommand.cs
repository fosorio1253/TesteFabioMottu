using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Drivers.Commands.CreateDriver;
public class CreateDriverCommand : ICommand<Guid>
{
    public string Name { get; init; }
    public string TaxId { get; init; }
    public DateTime BirthDate { get; init; }
    public string LicenseNumber { get; init; }
    public string LicenseType { get; init; }
}