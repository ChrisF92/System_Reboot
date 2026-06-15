namespace Game.Backend.Shared.Errors;

public class AppException : Exception
{
    public AppException(string code, string message, int statusCode)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }

    public string Code { get; }

    public int StatusCode { get; }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string code, string message)
        : base(code, message, StatusCodes.Status404NotFound)
    {
    }
}

public sealed class ValidationException : AppException
{
    public ValidationException(string code, string message)
        : base(code, message, StatusCodes.Status400BadRequest)
    {
    }
}

public sealed record ErrorEnvelope(ErrorDetails Error);

public sealed record ErrorDetails(string Code, string Message);
