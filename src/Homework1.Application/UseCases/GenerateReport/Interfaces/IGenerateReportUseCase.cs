namespace Homework1.Application.UseCases.GenerateReport.Interfaces;

public interface IGenerateReportUseCase
{
    void Execute(IReadOnlyCollection<long> clientIds, OutputMode outputMode);
}