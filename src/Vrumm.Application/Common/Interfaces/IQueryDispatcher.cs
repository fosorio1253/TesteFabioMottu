namespace Vrumm.Application.Common.Interfaces;
public interface IQueryDispatcher
{
    Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery command, CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>;
}