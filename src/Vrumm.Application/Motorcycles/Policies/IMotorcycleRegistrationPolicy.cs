using Vrumm.Domain.Events;

namespace Vrumm.Application.Motorcycles.Policies;
public interface IMotorcycleRegistrationPolicy
{
    bool ShouldRegisterEvent(MotorcycleRegistered notification);
}