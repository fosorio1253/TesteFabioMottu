using Microsoft.Extensions.Options;
using Vrumm.Domain.Events;
using Vrumm.Domain.Options;

namespace Vrumm.Application.Motorcycles.Policies;
public class MotorcycleRegistrationPolicy : IMotorcycleRegistrationPolicy
{
    private readonly int _targetYear;

    public MotorcycleRegistrationPolicy(IOptions<MotorcycleRegistrationOptions> registrationOptions)
    {
        _targetYear = registrationOptions.Value?.TargetYear ?? 2024;
    }

    public bool ShouldRegisterEvent(MotorcycleRegistered notification)
    {
        return notification.Year == _targetYear;
    }
}