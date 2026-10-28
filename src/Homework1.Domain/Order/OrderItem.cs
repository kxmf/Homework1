using System.Diagnostics.CodeAnalysis;

namespace Homework1.Domain.Order;

public sealed record OrderItem
{
    public string Name { get; }
    public float UnitPrice { get; }
    public int Quantity { get; }

    public float TotalPrice => UnitPrice * Quantity;

    private OrderItem(string name, float unitPrice, int quantity)
    {
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public static bool TryCreate(string name, float unitPrice, int quantity, [NotNullWhen(true)] out OrderItem? orderItem)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            orderItem = null;
            return false;
        }
        if (unitPrice < 0)
        {
            orderItem = null;
            return false;
        }
        if (quantity <= 0)
        {
            orderItem = null;
            return false;
        }

        orderItem = new OrderItem(name, unitPrice, quantity);
        return true;
    }

    public static OrderItem Create(string name, float unitPrice, int quantity)
    {
        if (!TryCreate(name, unitPrice, quantity, out var orderItem))
            ThrowOrderItemCreateException(name, unitPrice, quantity);

        return orderItem;
    }

    [DoesNotReturn]
    private static void ThrowOrderItemCreateException(string name, float unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new OrderItemDomainException("Item name cannot be empty.");

        if (unitPrice < 0)
            throw new OrderItemDomainException("Price cannot be negative.");

        if (quantity <= 0)
            throw new OrderItemDomainException("Quantity must be greater than zero.");

        throw new OrderItemDomainException($"Invalid order item parameters: name={name}, unitPrice={unitPrice}, quantity={quantity}");
    }
}
