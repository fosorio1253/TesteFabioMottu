using Microsoft.Extensions.Logging;
using Vrumm.Application.Common.Interfaces;
using Vrumm.Infrastructure.Data.UnitOfWork;

namespace Vrumm.Application.Common.Behaviors;
public class TransactionBehavior<TRequest, TResult> : ICommandPipelineBehavior<TRequest, TResult>
        where TRequest : ICommand<TResult>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResult>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TransactionBehavior(
        ILogger<TransactionBehavior<TRequest, TResult>> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<TResult> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        CommandHandlerDelegate<TResult> next)
    {
        var requestName = typeof(TRequest).Name;

        try
        {
            _logger.LogInformation("Beginning transaction for command {RequestName}", requestName);
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var result = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("Committed transaction for command {RequestName}", requestName);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Rolling back transaction for command {RequestName}", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}