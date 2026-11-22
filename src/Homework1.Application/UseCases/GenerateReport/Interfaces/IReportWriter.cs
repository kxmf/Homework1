namespace Homework1.Application.UseCases.GenerateReport.Interfaces;

public interface IReportWriter
{
    OutputMode Mode { get; }

    void Write(IReadOnlyCollection<ClientReportDto> reports);
}