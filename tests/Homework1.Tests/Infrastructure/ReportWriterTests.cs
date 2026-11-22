using FluentAssertions;

using Homework1.Application.UseCases.GenerateReport;
using Homework1.Infrastructure.Reporting;

using Xunit;

namespace Homework1.Tests.Infrastructure;

public class ConsoleReportWriterTests
{
    [Fact]
    public void Write_OutputsFormattedReportsToConsole()
    {
        var writer = new ConsoleReportWriter();
        var reports = new[]
        {
            new ClientReportDto(1010, 499.96m, "Чипсеки"),
            new ClientReportDto(1020, 660.00m, "Сырок со сгущенкой")
        };

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            writer.Write(reports);

            var output = stringWriter.ToString().TrimEnd();
            var lines = output.Split(Environment.NewLine);

            lines.Should().HaveCount(2);
            lines[0].Should().Be("1010,499.96,Чипсеки");
            lines[1].Should().Be("1020,660.00,Сырок со сгущенкой");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}

[Collection("ConsoleTests")]
public class FileReportWriterTests : IDisposable
{
    private readonly List<string> _createdFiles = [];

    public void Dispose()
    {
        foreach (var file in _createdFiles)
        {
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }

    [Fact]
    public void Write_CreatesCsvFileWithHeaderAndData()
    {
        var writer = new FileReportWriter();
        var reports = new[]
        {
            new ClientReportDto(1010, 499.96m, "Чипсеки")
        };

        var beforeFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "report_*.csv");

        writer.Write(reports);

        var afterFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "report_*.csv");
        var newFiles = afterFiles.Except(beforeFiles).ToList();
        _createdFiles.AddRange(newFiles);

        newFiles.Should().HaveCount(1);
        var createdFile = newFiles.Single();

        var lines = File.ReadAllLines(createdFile);
        lines.Should().HaveCount(2);
        lines[0].Should().Be("client_id,orders_sum,favorite_item_name");
        lines[1].Should().Be("1010,499.96,Чипсеки");
    }
}