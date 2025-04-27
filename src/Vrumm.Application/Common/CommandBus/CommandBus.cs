using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.CommandBus;
public class CommandBus : ICommandBus
{
    private readonly IServiceProvider _serviceProvider;

    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task<TResult> DispatchCommand<TResult>(ICommand<TResult> command, CancellationToken cancellationToken)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for command {command.GetType().Name}");

        var method = handlerType.GetMethod(nameof(ICommandHandler<ICommand<TResult>, TResult>.Handle));
        var task = (Task<TResult>)method!.Invoke(handler, new object[] { command, cancellationToken })!;
        return await task;
    }

    public async Task<TResult> DispatchQuery<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for query {query.GetType().Name}");

        var method = handlerType.GetMethod(nameof(IQueryHandler<IQuery<TResult>, TResult>.Handle));
        var task = (Task<TResult>)method!.Invoke(handler, new object[] { query, cancellationToken })!;
        return await task;
    }
}