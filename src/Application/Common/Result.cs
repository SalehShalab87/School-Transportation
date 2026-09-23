namespace Application.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; init; }
    public T? Data { get; init; }
    public Error? Error { get; init; }
    public IReadOnlyCollection<Error> Errors { get; init; } = Array.Empty<Error>();

    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    public static Result<T> Failure(Error error) => new()
    {
        IsSuccess = false,
        Error = error,
        Errors = [error]
    };

    public static Result<T> Failure(IEnumerable<Error> errors)
    {
        var list = errors.ToArray();
        return new Result<T>
        {
            IsSuccess = false,
            Error = list.FirstOrDefault(),
            Errors = list
        };
    }
}
