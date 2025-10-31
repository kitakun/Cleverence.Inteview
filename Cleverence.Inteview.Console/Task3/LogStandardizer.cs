namespace Cleverence.Inteview.Console.Task3;

/// <summary>
/// Основной класс для стандартизации лог-файлов
/// </summary>
public class LogStandardizer
{
    private readonly LogParser _parser = new();

    /// <summary>
    /// Обрабатывает лог-файл, преобразуя его в стандартный формат
    /// Невалидные записи записываются в problems.txt
    /// </summary>
    /// <param name="inputPath">Путь к входному файлу</param>
    /// <param name="outputPath">Путь к выходному файлу</param>
    /// <param name="cancellationToken">Токен отмены</param>
    public async Task ProcessLogFileAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(inputPath))
            throw new FileNotFoundException($"Входной файл не найден: {inputPath}");

        var problemsPath = Path.Combine(Path.GetDirectoryName(outputPath) ?? ".", "problems.txt");
        
        var processedCount = 0;
        var invalidCount = 0;

        await using var outputWriter = new StreamWriter(outputPath, false);
        await using var problemsWriter = new StreamWriter(problemsPath, false);
        using var inputReader = new StreamReader(inputPath);

        while (await inputReader.ReadLineAsync(cancellationToken) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (!_parser.CouldBeParsed(line))
            {
                // invalid record
                await problemsWriter.WriteLineAsync(line);
                invalidCount++;
                continue;
            }

            var record = _parser.Parse(line);

            var formattedLine = LogFormatter.FormatRecord(record);
            await outputWriter.WriteLineAsync(formattedLine);
            processedCount++;
        }

        System.Console.WriteLine($"Обработано записей: {processedCount}");
        System.Console.WriteLine($"Невалидных записей: {invalidCount}");
    }
}

