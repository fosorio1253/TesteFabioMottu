using FluentValidation;
using Vrumm.Application.Common.Interfaces;

namespace Vrumm.Application.Common.Behaviors;
public class ValidationBehavior<TRequest, TResult> : ICommandPipelineBehavior<TRequest, TResult>
        where TRequest : ICommand<TResult>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    }

    public async Task<TResult> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        CommandHandlerDelegate<TResult> next)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}