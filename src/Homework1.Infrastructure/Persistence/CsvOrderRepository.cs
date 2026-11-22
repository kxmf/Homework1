using System.Globalization;
using System.Text;

using Homework1.Domain;

namespace Homework1.Infrastructure.Persistence;

public class CsvOrderRepository : IOrderRepository
{
    private readonly string _filePath;

    public CsvOrderRepository(string filePath = "orders_log.csv")
    {
        _filePath = filePath;
    }

    public IReadOnlyCollection<Order> GetByClientId(long clientId)
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        var ordersById = new Dictionary<long, Order>();

        using var reader = new StreamReader(_filePath, Encoding.UTF8);

        reader.ReadLine();

        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var parts = line.Split(',');
            if (parts.Length < 5)
            {
                continue;
            }

            if (!long.TryParse(parts[0].Trim(), out var rowClientId) || rowClientId <= 0)
            {
                continue;
            }

            if (rowClientId != clientId)
            {
                continue;
            }

            if (!long.TryParse(parts[1].Trim(), out var orderId) || orderId <= 0)
            {
                continue;
            }

            var itemName = parts[2].Trim();
            if (string.IsNullOrWhiteSpace(itemName))
            {
                continue;
            }

            if (!int.TryParse(parts[3].Trim(), out var quantity) || quantity <= 0)
            {
                continue;
            }

            if (!decimal.TryParse(parts[4].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var price) || price < 0)
            {
                continue;
            }

            if (!ordersById.TryGetValue(orderId, out var order))
            {
                order = new Order
                {
                    Id = orderId,
                    ClientId = rowClientId
                };
                ordersById[orderId] = order;
            }

            order.Items.Add(new OrderItem
            {
                Name = itemName,
                UnitPrice = price,
                Quantity = quantity
            });
        }

        return ordersById.Values.ToList().AsReadOnly();
    }
}