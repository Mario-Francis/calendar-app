namespace CalendarApp.WebApi.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<string>? ErrorItems { get; }

    public ValidationException(string message) : base(message) { }
    public ValidationException(string message, IEnumerable<string> errorItems) : base(message)
    {
        ErrorItems = errorItems;
    }
}
