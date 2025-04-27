namespace Vrumm.Application.Common.Interfaces;
public interface INotificationHandler<in TNotification>
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}