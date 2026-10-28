using Homework1.Domain.Common;
using System.Diagnostics.CodeAnalysis;

namespace Homework1.Domain.Order;

public class Order : AggregateRoot<OrderId>
{
    public ClientId ClientId { get; }
    private readonly List<OrderItem> _items = [];

    private Order(OrderId orderId, ClientId clientId)
    {
        ClientId = clientId;
        Id = orderId;
    }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public float TotalAmount => _items.Sum(item => item.TotalPrice);

    public static Order Create(OrderId orderId, ClientId clientId)
    {
        if (!TryCreate(orderId, clientId, out var order))
            ThrowOrderCreateException(orderId, clientId);

        return order;
    }

    public static bool TryCreate(OrderId orderId, ClientId clientId, [NotNullWhen(true)] out Order? order)
    {
        if (orderId.IsEmpty || clientId.IsEmpty)
        {
            order = null;
            return false;
        }

        order = new Order(orderId, clientId);
        return true;
    }

    [DoesNotReturn]
    private static void ThrowOrderCreateException(OrderId orderId, ClientId clientId)
    {
        if (orderId.IsEmpty)
            throw new OrderDomainException("Cannot create Order: OrderId must not be empty.");
        if (clientId.IsEmpty)
            throw new OrderDomainException("Cannot create Order: ClientId must not be empty.");

        throw new OrderDomainException($"Cannot create Order: Invalid parameters: orderId={orderId}, clientId={clientId}");
    }

    public void AddItem(string name, float unitPrice, int quantity)
    {
        var newItem = OrderItem.Create(name, unitPrice, quantity);
        _items.Add(newItem);
    }
}