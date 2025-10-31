using BenchmarkDotNet.Attributes;

namespace Cleverence.Inteview.Console.Task3;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class LogStandardizerBenchmark
{
    private string _smallFile = null!;
    private string _mediumFile = null!;
    private string _largeFile = null!;
    private string _mixedFile = null!;
    private string _format1File = null!;
    private string _format2File = null!;
    private string _highInvalidFile = null!;

    private LogStandardizer _standardizer = null!;
    private string _outputPath = null!;

    [GlobalSetup]
    public void Setup()
    {
        _standardizer = new LogStandardizer();
        _outputPath = Path.GetTempFileName();

        _smallFile = CreateTestFile(100, 0.5, 0.0);
        _mediumFile = CreateTestFile(10000, 0.5, 0.0);
        _largeFile = CreateTestFile(100000, 0.5, 0.0);
        _mixedFile = CreateTestFile(10000, 0.5, 0.0);
        _format1File = CreateTestFile(10000, 1.0, 0.0);
        _format2File = CreateTestFile(10000, 0.0, 0.0);
        _highInvalidFile = CreateTestFile(10000, 0.5, 0.2);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        DeleteFileIfExists(_smallFile);
        DeleteFileIfExists(_mediumFile);
        DeleteFileIfExists(_largeFile);
        DeleteFileIfExists(_mixedFile);
        DeleteFileIfExists(_format1File);
        DeleteFileIfExists(_format2File);
        DeleteFileIfExists(_highInvalidFile);
        DeleteFileIfExists(_outputPath);
        DeleteFileIfExists(Path.Combine(Path.GetDirectoryName(_outputPath) ?? ".", "problems.txt"));
    }

    [IterationSetup]
    public void IterationSetup()
    {
        // Очистка выходных файлов между итерациями
        DeleteFileIfExists(_outputPath);
        DeleteFileIfExists(Path.Combine(Path.GetDirectoryName(_outputPath) ?? ".", "problems.txt"));
        _outputPath = Path.GetTempFileName();
    }

    [Benchmark(Description = "Small file: 100 строк (50/50 форматы)")]
    public async Task ProcessSmallFile()
    {
        await _standardizer.ProcessLogFileAsync(_smallFile, _outputPath);
    }

    [Benchmark(Description = "Medium file: 10,000 строк (50/50 форматы)")]
    public async Task ProcessMediumFile()
    {
        await _standardizer.ProcessLogFileAsync(_mediumFile, _outputPath);
    }

    [Benchmark(Description = "Large file: 100,000 строк (50/50 форматы)")]
    public async Task ProcessLargeFile()
    {
        await _standardizer.ProcessLogFileAsync(_largeFile, _outputPath);
    }

    [Benchmark(Description = "Mixed formats: 10,000 строк (50/50)")]
    public async Task ProcessMixedFormats()
    {
        await _standardizer.ProcessLogFileAsync(_mixedFile, _outputPath);
    }

    [Benchmark(Description = "Format1 only: 10,000 строк")]
    public async Task ProcessFormat1Only()
    {
        await _standardizer.ProcessLogFileAsync(_format1File, _outputPath);
    }

    [Benchmark(Description = "Format2 only: 10,000 строк")]
    public async Task ProcessFormat2Only()
    {
        await _standardizer.ProcessLogFileAsync(_format2File, _outputPath);
    }

    [Benchmark(Description = "High invalid rate: 10,000 строк (20% невалидных)")]
    public async Task ProcessHighInvalidRate()
    {
        await _standardizer.ProcessLogFileAsync(_highInvalidFile, _outputPath);
    }

    private string CreateTestFile(int lineCount, double format1Ratio, double invalidRatio)
    {
        var tempFile = Path.GetTempFileName();
        var random = new Random(42);

        using var writer = new StreamWriter(tempFile);

        for (var i = 0; i < lineCount; i++)
        {
            var rand = random.NextDouble();

            if (rand < invalidRatio)
            {
                writer.WriteLine($"Invalid log line {i} with random data");
            }
            else if (rand < invalidRatio + format1Ratio * (1 - invalidRatio))
            {
                // Format 1
                var date = new DateTime(2025, 3, 10 + (i % 20));
                var level = GetRandomLogLevel(random);
                writer.WriteLine($"{date:dd.MM.yyyy} 15:14:{i % 60:D2}.{random.Next(1000):D3} {level} Сообщение {i}");
            }
            else
            {
                // Format 2
                var date = new DateTime(2025, 3, 10 + (i % 20));
                var level = GetRandomLogLevel(random);
                var threadId = random.Next(1, 100);
                writer.WriteLine($"{date:yyyy-MM-dd} 15:14:{i % 60:D2}.{random.Next(10000):D4}| {level}|{threadId}|TestClass.Method{i % 10}| Сообщение {i}");
            }
        }

        return tempFile;
    }

    private string GetRandomLogLevel(Random random)
    {
        var levels = new[] { "INFO", "INFORMATION", "WARN", "WARNING", "ERROR", "DEBUG" };
        return levels[random.Next(levels.Length)];
    }

    private void DeleteFileIfExists(string path)
    {
        if (!File.Exists(path))
            return;

        try
        {
            File.Delete(path);
        }
        catch
        {
            // Игнорируем ошибки удаления
        }
    }
}