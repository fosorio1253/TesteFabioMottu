using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Dispatching;
public class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueryDispatcher> _logger;

    public QueryDispatcher(IServiceProvider serviceProvider, ILogger<QueryDispatcher> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> DispatchAsync<TQuery, TResponse>(TQuery command, CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        _logger.LogInformation("Executando comando: {CommandType}", typeof(TQuery).Name);

        var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>();
        if (handler == null)
            throw new InvalidOperationException($"Nenhum handler encontrado para o comando {typeof(TQuery).Name}");

        return await handler.Handle(command, cancellationToken);
    }
}