using Homework1.Domain.Common;

namespace Homework1.Domain.Order;

public class ClientNotFoundException : DomainException
{
    public ClientNotFoundException(string message) : base(message)
    {
    }

    public ClientNotFoundException(Exception innerException) : base(innerException)
    {
    }

    public ClientNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
