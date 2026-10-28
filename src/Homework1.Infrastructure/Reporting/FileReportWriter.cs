using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using System.Globalization;
using System.Text;

namespace Homework1.Infrastructure.Reporting;

public class FileReportWriter : IReportWriter
{
    public OutputMode Mode => OutputMode.File;

    public void Write(IReadOnlyCollection<ClientReportDto> reports)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
        var fileName = $"report_{timestamp}.csv";

        using var writer = new StreamWriter(fileName, append: false, Encoding.UTF8);

        writer.WriteLine("client_id,orders_sum,favorite_item_name");

        foreach (var report in reports)
        {
            writer.WriteLine($"{report.ClientId},{report.OrdersSum.ToString("F2", CultureInfo.InvariantCulture)},{report.FavoriteItemName}");
        }

        Console.WriteLine($"Report saved to file: {Path.GetFullPath(fileName)}");
    }
}
