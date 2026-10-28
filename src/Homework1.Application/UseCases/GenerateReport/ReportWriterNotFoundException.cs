namespace Homework1.Application.UseCases.GenerateReport;

public class ReportWriterNotFoundException : Common.ApplicationException
{
    public ReportWriterNotFoundException(string message) : base(message)
    {
    }

    public ReportWriterNotFoundException(Exception innerException) : base(innerException)
    {
    }

    public ReportWriterNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
