using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Dispatching;
public class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandDispatcher> _logger;

    public CommandDispatcher(IServiceProvider serviceProvider, ILogger<CommandDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> DispatchAsync<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        _logger.LogInformation("Executando comando: {CommandType}", typeof(TCommand).Name);

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>();
        if (handler == null)
            throw new InvalidOperationException($"Nenhum handler encontrado para o comando {typeof(TCommand).Name}");

        return await handler.Handle(command, cancellationToken);
    }

    public async Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        _logger.LogInformation("Executando comando: {CommandType}", typeof(TCommand).Name);

        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
        if (handler == null)
            throw new InvalidOperationException($"Nenhum handler encontrado para o comando {typeof(TCommand).Name}");

        await handler.Handle(command, cancellationToken);
    }
}