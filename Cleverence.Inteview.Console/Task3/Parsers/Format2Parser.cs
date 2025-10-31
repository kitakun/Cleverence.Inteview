using Cleverence.Inteview.Console.Task3.Models;

namespace Cleverence.Inteview.Console.Task3.Parsers;

/// <summary>
/// Парсер для формата 2: 2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'
/// </summary>
public class Format2Parser : ILogParser
{
    public bool CouldBeParsed(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return false;

        var span = line.AsSpan();

        if (span.Length < 34)
            return false;

        if (span.Length < 10 || span[4] != '-' || span[7] != '-')
            return false;

        if (!IsDigit(span[0]) || !IsDigit(span[1]) || !IsDigit(span[2]) || !IsDigit(span[3]) ||
            !IsDigit(span[5]) || !IsDigit(span[6]) ||
            !IsDigit(span[8]) || !IsDigit(span[9]))
            return false;

        var pipeIndex = span.IndexOf('|');
        if (pipeIndex is -1 or < 20)
            return false;

        var pipeCount = 0;
        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == '|')
                pipeCount++;
        }

        return pipeCount >= 4;
    }

    public LogRecord Parse(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            throw new FormatException("Строка не может быть пустой");

        var span = line.AsSpan();
        var pos = 0;

        if (span.Length < 10)
            throw new FormatException($"Строка слишком короткая: {line}");

        var year = ParseFourDigits(span, ref pos);
        if (span[pos] != '-')
            throw new FormatException($"Ожидается '-' после года: {line}");
        pos++;

        var month = ParseTwoDigits(span, ref pos);
        if (span[pos] != '-')
            throw new FormatException($"Ожидается '-' после месяца: {line}");
        pos++;

        var day = ParseTwoDigits(span, ref pos);

        DateTime date;
        try
        {
            date = new DateTime(year, month, day);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw new FormatException($"Невалидная дата: {year}-{month:D2}-{day:D2}");
        }

        SkipSpaces(span, ref pos);

        var timeStart = pos;
        while (pos < span.Length && span[pos] != '|')
            pos++;

        if (pos >= span.Length)
            throw new FormatException($"Отсутствует разделитель '|': {line}");

        var time = span.Slice(timeStart, pos - timeStart).ToString().Trim();
        pos++;

        SkipSpaces(span, ref pos);

        var levelStart = pos;
        while (pos < span.Length && span[pos] != '|')
            pos++;

        if (pos >= span.Length)
            throw new FormatException($"Отсутствует второй разделитель '|': {line}");

        var levelStr = span.Slice(levelStart, pos - levelStart).ToString().Trim();
        pos++;

        if (!TryMapLogLevel(levelStr, out var logLevel))
            throw new FormatException($"Неизвестный уровень логирования: {levelStr}");

        while (pos < span.Length && span[pos] != '|')
            pos++;

        if (pos >= span.Length)
            throw new FormatException($"Отсутствует третий разделитель '|': {line}");
        pos++;

        var methodStart = pos;
        while (pos < span.Length && span[pos] != '|')
            pos++;

        if (pos >= span.Length)
            throw new FormatException($"Отсутствует четвёртый разделитель '|': {line}");

        var method = span.Slice(methodStart, pos - methodStart).ToString().Trim();
        pos++;

        SkipSpaces(span, ref pos);

        var message = pos < span.Length ? span[pos..].ToString() : string.Empty;

        return new LogRecord(date, time, logLevel, method, message);
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
            throw new FormatException("Ожидается четыре цифры");

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
            pos++;
    }

    private static bool IsDigit(char c) => c is >= '0' and <= '9';

    /// <summary>
    /// Маппинг строкового представления уровня логирования в enum
    /// INFORMATION -> INFO, WARNING -> WARN
    /// </summary>
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