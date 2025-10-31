using BenchmarkDotNet.Running;

using Cleverence.Inteview.Console.Task1;
using Cleverence.Inteview.Console.Task2;
using Cleverence.Inteview.Console.Task3;

Console.WriteLine("Cleverence Interview Console");
Console.WriteLine("============================");
Console.WriteLine();
Console.WriteLine("1. Запустить бенчмарки Compressor");
Console.WriteLine("2. Запустить бенчмарки Server");
Console.WriteLine("3. Стандартизировать лог-файл");
Console.WriteLine("4. Запустить бенчмарки LogStandardizer");
Console.WriteLine("5. Выход");
Console.WriteLine();
Console.Write("Выберите действие: ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        BenchmarkRunner.Run<CompressorBenchmark>();
        break;
    case "2":
        BenchmarkRunner.Run<ServerBenchmark>();
        break;
    case "3":
        Console.Write("Путь к входному файлу: ");
        var inputPath = Console.ReadLine();
        Console.Write("Путь к выходному файлу: ");
        var outputPath = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(inputPath) || string.IsNullOrWhiteSpace(outputPath))
        {
            Console.WriteLine("Ошибка: пути не могут быть пустыми");
            break;
        }
        
        try
        {
            var standardizer = new LogStandardizer();
            await standardizer.ProcessLogFileAsync(inputPath, outputPath);
            Console.WriteLine("Обработка завершена");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при обработке: {ex.Message}");
        }
        break;
    case "4":
        BenchmarkRunner.Run<LogStandardizerBenchmark>();
        break;
    case "5":
        Console.WriteLine("Выход...");
        break;
    default:
        Console.WriteLine("Неизвестная команда");
        break;
}