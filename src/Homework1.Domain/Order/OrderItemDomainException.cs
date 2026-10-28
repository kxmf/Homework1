using Homework1.Domain.Common;

namespace Homework1.Domain.Order;

public class OrderItemDomainException : DomainException
{
    public OrderItemDomainException(string message) : base(message)
    {
    }

    public OrderItemDomainException(Exception innerException) : base(innerException)
    {
    }

    public OrderItemDomainException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
