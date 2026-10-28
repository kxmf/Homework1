using Homework1.Domain.Order;
using System.Globalization;
using System.Text;

namespace Homework1.Infrastructure.Persistence;

public class CsvOrderRepository : IOrderRepository
{
    private readonly string _filePath;

    public CsvOrderRepository(string filePath = "orders_log.csv")
    {
        _filePath = filePath;
    }

    public IReadOnlyCollection<Order> GetByClientId(ClientId clientId)
    {
        var ordersById = new Dictionary<long, Order>();

        using var reader = new StreamReader(_filePath, Encoding.UTF8);

        // Skip the header line
        reader.ReadLine();

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = line.Split(',');
            if (parts.Length < 5)
                continue;

            if (!long.TryParse(parts[0].Trim(), out var rowClientId))
                continue;

            if (rowClientId != clientId.Value)
                continue;

            if (!long.TryParse(parts[1].Trim(), out var orderId))
                continue;

            var itemName = parts[2].Trim();

            if (!int.TryParse(parts[3].Trim(), out var quantity))
                continue;

            if (!float.TryParse(parts[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var price))
                continue;

            if (!ordersById.TryGetValue(orderId, out var order))
            {
                if (!OrderId.TryCreate(orderId, out var orderIdVO))
                    continue;

                order = Order.Create(orderIdVO, clientId);
                ordersById[orderId] = order;
            }

            order.AddItem(itemName, price, quantity);
        }

        return ordersById.Values.ToList().AsReadOnly();
    }
}
