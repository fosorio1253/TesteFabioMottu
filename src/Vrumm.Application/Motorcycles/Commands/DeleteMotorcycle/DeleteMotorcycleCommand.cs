using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Motorcycles.Commands.DeleteMotorcycle;
public class DeleteMotorcycleCommand : ICommand
{
    public Guid Id { get; set; }
}