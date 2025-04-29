using Microsoft.Extensions.Configuration;
using Vrumm.Domain.Events;

namespace Vrumm.Application.Motorcycles.Policies;
public class MotorcycleRegistrationPolicy : IMotorcycleRegistrationPolicy
{
    private readonly int _targetYear;

    public MotorcycleRegistrationPolicy(IConfiguration configuration)
    {
        _targetYear = configuration.GetValue<int>("MotorcycleRegistration:TargetYear", 2024);
    }

    public bool ShouldRegisterEvent(MotorcycleRegistered notification)
    {
        return notification.Year == _targetYear;
    }
}