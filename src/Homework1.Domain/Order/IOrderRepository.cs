using Homework1.Domain.Common;

namespace Homework1.Domain.Order;

public interface IOrderRepository : IRepository<Order, OrderId>
{
    IReadOnlyCollection<Order> GetByClientId(ClientId clientId);
}
