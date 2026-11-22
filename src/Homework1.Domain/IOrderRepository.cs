namespace Homework1.Domain;

public interface IOrderRepository
{
    IReadOnlyCollection<Order> GetByClientId(long clientId);
}
