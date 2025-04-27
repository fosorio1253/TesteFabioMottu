using FluentValidation.Results;

namespace Vrumm.Application.Common.Exceptions;
public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation failures have occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray())
            .AsReadOnly();
    }

    public ValidationException(string propertyName, string error)
        : base("Validation failure occurred.")
    {
        Errors = new Dictionary<string, string[]> { { propertyName, new[] { error } } }.AsReadOnly();
    }
}