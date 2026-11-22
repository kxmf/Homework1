using Homework1.Application;
using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Infrastructure;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services
    .AddApplication()
    .AddInfrastructure();

var provider = services.BuildServiceProvider();

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: <console|file> <client_id> [client_id ...]");
    return 1;
}

if (!Enum.TryParse<OutputMode>(args[0], ignoreCase: true, out var outputMode))
{
    Console.Error.WriteLine($"Invalid output mode: '{args[0]}'. Allowed values: console, file.");
    return 1;
}

var clientIds = new List<long>();
foreach (var arg in args.Skip(1))
{
    if (!long.TryParse(arg, out var id))
    {
        Console.Error.WriteLine($"Invalid client ID: '{arg}'. Expected an integer.");
        return 1;
    }
    clientIds.Add(id);
}

try
{
    var useCase = provider.GetRequiredService<IGenerateReportUseCase>();
    useCase.Execute(clientIds, outputMode);
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}