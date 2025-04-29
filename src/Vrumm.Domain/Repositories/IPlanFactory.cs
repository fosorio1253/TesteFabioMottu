using Vrumm.Domain.Entities;

namespace Vrumm.Application.Plans;
public interface IPlanFactory
{
    Plan CreatePlan(int planId);
}