using Homework1.Domain.Common;

namespace Homework1.Domain.Order;

public class OrderDomainException : DomainException
{
    public OrderDomainException(string message) : base(message)
    {
    }

    public OrderDomainException(Exception innerException) : base(innerException)
    {
    }

    public OrderDomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
