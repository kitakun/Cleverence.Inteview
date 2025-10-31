using Cleverence.Inteview.Console.Task3.Models;

namespace Cleverence.Inteview.Console.Task3;

public static class LogFormatter
{
    /// <summary>
    /// Форматирует запись лога в стандартный выходной формат
    /// Формат: Дата\tВремя\tУровень\tМетод\tСообщение
    /// Дата в формате DD-MM-YYYY
    /// </summary>
    public static string FormatRecord(LogRecord record)
    {
        var formattedDate = record.Date.ToString("dd-MM-yyyy");
        var level = record.ParsedLogLevel.ToString();
        
        return $"{formattedDate}\t{record.Time}\t{level}\t{record.CallingMethod}\t{record.Message}";
    }
}

