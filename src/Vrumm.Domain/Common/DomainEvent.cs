namespace Vrumm.Domain.Common;
public abstract class DomainEvent
{
    public Guid Id { get; }
    public DateTime Timestamp { get; }

    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}