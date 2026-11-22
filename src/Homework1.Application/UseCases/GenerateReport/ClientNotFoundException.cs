namespace Homework1.Application.UseCases.GenerateReport;

public class ClientNotFoundException : Common.ApplicationException
{
    public ClientNotFoundException(string message) : base(message)
    {
    }

    public ClientNotFoundException(Exception innerException) : base(innerException)
    {
    }

    public ClientNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
