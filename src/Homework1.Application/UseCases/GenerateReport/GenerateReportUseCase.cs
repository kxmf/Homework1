using Homework1.Application.Common;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Domain.Order;

namespace Homework1.Application.UseCases.GenerateReport;

public class GenerateReportUseCase : IGenerateReportUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEnumerable<IReportWriter> _writers;

    public GenerateReportUseCase(IOrderRepository orderRepository, IEnumerable<IReportWriter> writers)
    {
        _orderRepository = orderRepository;
        _writers = writers;
    }

    public void Execute(IReadOnlyCollection<long> clientIds, OutputMode outputMode)
    {
        ArgumentNullException.ThrowIfNull(clientIds);

        var reports = new List<ClientReportDto>();

        foreach (var clientIdLong in clientIds)
        {
            if (!ClientId.TryCreate(clientIdLong, out var clientId))
            {
                throw new InvalidIdException($"Invalid client ID '{clientIdLong}'. Client ID must be a positive number greater than zero.");
            }

            var orders = _orderRepository.GetByClientId(clientId);
            if (orders.Count == 0)
            {
                throw new ClientNotFoundException($"Client with ID '{clientIdLong}' not found.");
            }

            var ordersSum = orders.Sum(o => o.TotalAmount);

            var favoriteItem = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Name)
                .Select(g => new { Name = g.Key, TotalQuantity = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefault();

            reports.Add(new ClientReportDto(clientIdLong, ordersSum, favoriteItem?.Name ?? string.Empty));
        }

        var writer = _writers.FirstOrDefault(w => w.Mode == outputMode)
            ?? throw new ReportWriterNotFoundException($"No report writer registered for output mode '{outputMode}'.");

        writer.Write(reports);
    }
}
