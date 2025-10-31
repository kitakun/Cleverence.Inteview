namespace Cleverence.Inteview.Console.Task3.Models;

public record LogRecord(
    DateTime Date,
    string Time,
    ParsedLogLevel ParsedLogLevel,
    string CallingMethod,
    string Message
);

