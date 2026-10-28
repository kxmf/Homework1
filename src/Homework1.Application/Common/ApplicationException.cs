namespace Homework1.Application.Common;

public abstract class ApplicationException : Exception
{
    public ApplicationException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public ApplicationException(string message)
        : base(message, null)
    {
    }

    public ApplicationException(Exception innerException)
        : base(null, innerException)
    {
    }
}
