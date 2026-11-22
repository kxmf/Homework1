using System.Text;

using FluentAssertions;

using Homework1.Application;
using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Domain;
using Homework1.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace Homework1.Tests.Integration;

public class DependencyInjectionTests : IDisposable
{
    private readonly string _tempCsvPath;

    public DependencyInjectionTests()
    {
        _tempCsvPath = Path.Combine(Path.GetTempPath(), $"di_test_{Guid.NewGuid():N}.csv");
    }

    public void Dispose()
    {
        if (File.Exists(_tempCsvPath))
        {
            File.Delete(_tempCsvPath);
        }
    }

    [Fact]
    public void ServiceProvider_ResolvesAllDependenciesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure(_tempCsvPath);

        var provider = services.BuildServiceProvider();

        var useCase = provider.GetService<IGenerateReportUseCase>();
        var repo = provider.GetService<IOrderRepository>();
        var writers = provider.GetServices<IReportWriter>().ToList();

        useCase.Should().NotBeNull();
        repo.Should().NotBeNull();
        writers.Should().HaveCount(2);
        writers.Should().Contain(w => w.Mode == OutputMode.Console);
        writers.Should().Contain(w => w.Mode == OutputMode.File);
    }

    [Fact]
    public void EndToEnd_UsingRegisteredServices_ExecutesAndOutputsExpectedResult()
    {
        var csvContent = """
            client_id,order_id,item_name,item_quantity,item_price
            1010,1,Огурчики маринованные,1,100
            1010,1,Чипсеки,4,99.99
            """;
        File.WriteAllText(_tempCsvPath, csvContent, Encoding.UTF8);

        var services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure(_tempCsvPath);
        var provider = services.BuildServiceProvider();

        var useCase = provider.GetRequiredService<IGenerateReportUseCase>();

        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            useCase.Execute([1010], OutputMode.Console);

            var output = stringWriter.ToString().TrimEnd();
            output.Should().Be("1010,499.96,Чипсеки");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}