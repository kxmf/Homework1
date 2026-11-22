using Homework1.Application.UseCases.GenerateReport;
using Homework1.Application.UseCases.GenerateReport.Interfaces;

using Microsoft.Extensions.DependencyInjection;

namespace Homework1.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IGenerateReportUseCase, GenerateReportUseCase>();

        return services;
    }
}