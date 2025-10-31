using Cleverence.Inteview.Console.Task3.Models;
using Cleverence.Inteview.Console.Task3.Parsers;

namespace Cleverence.Inteview.Console.Task3;

/// <summary>
/// Парсер для лог-файлов, поддерживающий несколько форматов
/// </summary>
public class LogParser
{
    private readonly ILogParser[] _parsers =
    [
        new Format1Parser(),
        new Format2Parser()
    ];

    /// <summary>
    /// Проверяет, может ли строка быть распарсена
    /// </summary>
    /// <param name="line">Строка для проверки</param>
    /// <returns>true если строка может быть распарсена, иначе false</returns>
    public bool CouldBeParsed(string line)
    {
        return _parsers.Any(parser => parser.CouldBeParsed(line));
    }

    /// <summary>
    /// Парсит строку лога, пробуя все доступные форматы
    /// </summary>
    /// <param name="line">Строка для парсинга</param>
    /// <returns>LogRecord</returns>
    /// <exception cref="FormatException">Если строка не может быть распарсена ни одним парсером</exception>
    public LogRecord Parse(string line)
    {
        foreach (var parser in _parsers)
        {
            if (parser.CouldBeParsed(line))
            {
                return parser.Parse(line);
            }
        }

        throw new FormatException($"Строка не соответствует ни одному известному формату: {line}");
    }
}

