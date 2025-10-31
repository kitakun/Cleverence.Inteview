using Cleverence.Inteview.Console.Task3.Models;

namespace Cleverence.Inteview.Console.Task3.Parsers;

/// <summary>
/// Парсер для формата 1: 10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'
/// </summary>
public class Format1Parser : ILogParser
{
    public bool CouldBeParsed(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return false;

        var span = line.AsSpan();

        if (span.Length < 26)
            return false;

        if (span.Length < 10 || span[2] != '.' || span[5] != '.')
            return false;

        if (!IsDigit(span[0]) || !IsDigit(span[1]) ||
            !IsDigit(span[3]) || !IsDigit(span[4]) ||
            !IsDigit(span[6]) || !IsDigit(span[7]) || !IsDigit(span[8]) || !IsDigit(span[9]))
            return false;

        var pos = 10;
        if (pos >= span.Length || span[pos] != ' ')
            return false;

        while (pos < span.Length && span[pos] == ' ')
        {
            pos++;
        }

        if (pos + 11 >= span.Length)
            return false;

        if (span[pos + 2] != ':' || span[pos + 5] != ':' || span[pos + 8] != '.')
            return false;

        return true;
    }

    public LogRecord Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new FormatException("Строка не может быть пустой");

        var span = line.AsSpan();
        var pos = 0;

        if (span.Length < 10)
            throw new FormatException($"Строка слишком короткая: {line}");

        var day = ParseTwoDigits(span, ref pos);
        if (span[pos] != '.')
            throw new FormatException($"Ожидается '.' после дня: {line}");
        pos++;

        var month = ParseTwoDigits(span, ref pos);
        if (span[pos] != '.')
            throw new FormatException($"Ожидается '.' после месяца: {line}");
        pos++;

        var year = ParseFourDigits(span, ref pos);

        DateTime date;
        try
        {
            date = new DateTime(year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new FormatException($"Невалидная дата: {day:D2}.{month:D2}.{year}");
        }

        SkipSpaces(span, ref pos);

        var timeStart = pos;
        while (pos < span.Length && span[pos] != ' ')
        {
            pos++;
        }

        var time = span.Slice(timeStart, pos - timeStart).ToString();

        SkipSpaces(span, ref pos);

        var levelStart = pos;
        while (pos < span.Length && span[pos] != ' ')
        {
            pos++;
        }

        if (levelStart == pos)
            throw new FormatException($"Уровень логирования отсутствует: {line}");

        var levelStr = span.Slice(levelStart, pos - levelStart).ToString();

        if (!TryMapLogLevel(levelStr, out var logLevel))
            throw new FormatException($"Неизвестный уровень логирования: {levelStr}");

        SkipSpaces(span, ref pos);

        if (pos >= span.Length)
            throw new FormatException($"Сообщение отсутствует: {line}");

        var message = span[pos..].ToString();

        return new LogRecord(date, time, logLevel, "DEFAULT", message);
    }

    private static int ParseTwoDigits(ReadOnlySpan<char> span, ref int pos)
    {
        if (pos + 1 >= span.Length || !IsDigit(span[pos]) || !IsDigit(span[pos + 1]))
            throw new FormatException("Ожидается две цифры");

        var result = (span[pos] - '0') * 10 + (span[pos + 1] - '0');
        pos += 2;
        return result;
    }

    private static int ParseFourDigits(ReadOnlySpan<char> span, ref int pos)
    {
        if (pos + 3 >= span.Length ||
            !IsDigit(span[pos]) || !IsDigit(span[pos + 1]) ||
            !IsDigit(span[pos + 2]) || !IsDigit(span[pos + 3]))
        {
            throw new FormatException("Ожидается четыре цифры");
        }

        var result = (span[pos] - '0') * 1000 +
                     (span[pos + 1] - '0') * 100 +
                     (span[pos + 2] - '0') * 10 +
                     (span[pos + 3] - '0');
        pos += 4;
        return result;
    }

    private static void SkipSpaces(ReadOnlySpan<char> span, ref int pos)
    {
        while (pos < span.Length && span[pos] == ' ')
        {
            pos++;
        }
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    private static bool TryMapLogLevel(string levelStr, out ParsedLogLevel parsedLogLevel)
    {
        var mappedLevel = levelStr.ToUpperInvariant() switch
        {
            "INFO" or "INFORMATION" => ParsedLogLevel.INFO,
            "WARN" or "WARNING" => ParsedLogLevel.WARN,
            "ERROR" => ParsedLogLevel.ERROR,
            "DEBUG" => (ParsedLogLevel?)ParsedLogLevel.DEBUG,
            _ => null
        };

        if (mappedLevel.HasValue)
        {
            parsedLogLevel = mappedLevel.Value;
            return true;
        }

        parsedLogLevel = default;
        return false;
    }
}