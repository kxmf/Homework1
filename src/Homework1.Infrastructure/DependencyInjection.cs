using Homework1.Application.UseCases.GenerateReport.Interfaces;
using Homework1.Domain;
using Homework1.Infrastructure.Persistence;
using Homework1.Infrastructure.Reporting;

using Microsoft.Extensions.DependencyInjection;

namespace Homework1.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string csvFilePath = "orders_log.csv")
    {
        services.AddSingleton<IOrderRepository>(_ => new CsvOrderRepository(csvFilePath));

        services.AddSingleton<IReportWriter, ConsoleReportWriter>();
        services.AddSingleton<IReportWriter, FileReportWriter>();

        return services;
    }
}
