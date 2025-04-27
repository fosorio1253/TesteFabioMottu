using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Behaviors;
public interface ICommandPipelineBehavior<in TRequest, TResult> where TRequest : ICommand<TResult>
{
    Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, CommandHandlerDelegate<TResult> next);
}

public delegate Task<TResult> CommandHandlerDelegate<TResult>();