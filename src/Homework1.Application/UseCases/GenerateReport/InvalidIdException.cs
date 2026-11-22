namespace Homework1.Application.UseCases.GenerateReport;

public class InvalidIdException : Common.ApplicationException
{
    public InvalidIdException(string message) : base(message)
    {
    }

    public InvalidIdException(Exception innerException) : base(innerException)
    {
    }

    public InvalidIdException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
