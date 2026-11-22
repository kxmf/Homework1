namespace Homework1.Domain;

public class Order
{
    public long Id { get; init; }
    public long ClientId { get; init; }
    public List<OrderItem> Items { get; init; } = [];
}
