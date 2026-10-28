namespace Homework1.Domain.Common;

public abstract class DomainException : Exception
{
    public DomainException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    public DomainException(string message)
        : base(message, null)
    {
    }

    public DomainException(Exception innerException)
        : base(null, innerException)
    {
    }
}
