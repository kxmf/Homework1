using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using System.Globalization;

namespace Homework1.Infrastructure.Reporting;

public class ConsoleReportWriter : IReportWriter
{
    public OutputMode Mode => OutputMode.Console;

    public void Write(IReadOnlyCollection<ClientReportDto> reports)
    {
        foreach (var report in reports)
        {
            Console.WriteLine($"{report.ClientId},{report.OrdersSum.ToString("F2", CultureInfo.InvariantCulture)},{report.FavoriteItemName}");
        }
    }
}
