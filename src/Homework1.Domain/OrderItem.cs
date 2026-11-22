namespace Homework1.Domain;

public class OrderItem
{
    public string Name { get; init; } = string.Empty;

    public decimal UnitPrice { get; init; }

    public int Quantity { get; init; }
}