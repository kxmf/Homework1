using Homework1.Domain.Common;

namespace Homework1.Domain.Order;

public class InvalidIdException : DomainException
{
    public InvalidIdException(string message) : base(message)
    {
    }

    public InvalidIdException(Exception innerException) : base(innerException)
    {
    }

    public InvalidIdException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
