namespace Vrumm.Application.Common.Interfaces;
public interface ICommandBus
{
    Task<TResult> DispatchCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
    Task<TResult> DispatchQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken);
}