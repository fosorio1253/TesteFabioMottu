namespace Vrumm.Application.Common.Models;
public class Result
{
    public bool Succeeded { get; }
    public IReadOnlyCollection<string> Errors { get; }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        var listErrors = errors?.ToList().AsReadOnly();
        if (listErrors == null || listErrors.Count == 0)
            Array.Empty<string>();

        Succeeded = succeeded;
        Errors = listErrors == null || listErrors.Count == 0
            ? Array.Empty<string>()
            : listErrors;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(string error) => new(false, new[] { error });
}

public class Result<T> : Result
{
    public T Data { get; }

    protected Result(bool succeeded, T data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, null);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public static Result<T> Failure(string error) => new(false, default, new[] { error });
}