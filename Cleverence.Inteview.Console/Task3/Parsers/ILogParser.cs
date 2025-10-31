using Cleverence.Inteview.Console.Task3.Models;

namespace Cleverence.Inteview.Console.Task3.Parsers;

public interface ILogParser
{
    /// <summary>
    /// Быстрая проверка, может ли строка быть распарсена этим парсером
    /// </summary>
    /// <param name="line">Строка для проверки</param>
    /// <returns>true если строка соответствует формату, иначе false</returns>
    bool CouldBeParsed(string line);

    /// <summary>
    /// Парсит строку лога
    /// </summary>
    /// <param name="line">Строка для парсинга</param>
    /// <returns>LogRecord</returns>
    /// <exception cref="FormatException">Если строка не может быть распарсена</exception>
    LogRecord Parse(string line);
}

