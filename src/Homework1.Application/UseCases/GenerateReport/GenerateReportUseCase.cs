using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Domain;

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

        foreach (var clientId in clientIds)
        {
            if (clientId <= 0)
            {
                throw new InvalidIdException($"Invalid client ID '{clientId}'. Client ID must be a positive number greater than zero.");
            }

            var orders = _orderRepository.GetByClientId(clientId);
            if (orders.Count == 0)
            {
                throw new ClientNotFoundException($"Client with ID '{clientId}' not found.");
            }

            var ordersSum = orders.Sum(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));

            var favoriteItem = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.Name)
                .Select(g => new { Name = g.Key, TotalQuantity = g.Sum(i => i.Quantity) })
                .OrderByDescending(x => x.TotalQuantity)
                .FirstOrDefault();

            reports.Add(new ClientReportDto(clientId, ordersSum, favoriteItem?.Name ?? string.Empty));
        }

        var writer = _writers.FirstOrDefault(w => w.Mode == outputMode)
            ?? throw new ReportWriterNotFoundException($"No report writer registered for output mode '{outputMode}'.");

        writer.Write(reports);
    }
}
