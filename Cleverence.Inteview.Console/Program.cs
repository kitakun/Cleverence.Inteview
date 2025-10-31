using BenchmarkDotNet.Running;
using Cleverence.Inteview.Console;
using Cleverence.Inteview.Console.Task1;

Console.WriteLine("Cleverence Interview Console");
Console.WriteLine("============================");
Console.WriteLine();
Console.WriteLine("1. Запустить бенчмарки Compressor");
Console.WriteLine("2. Выход");
Console.WriteLine();
Console.Write("Выберите действие: ");

var choice = Console.ReadLine();

switch (choice)
{
    case "1":
        BenchmarkRunner.Run<CompressorBenchmark>();
        break;
    case "2":
        Console.WriteLine("Выход...");
        break;
    default:
        Console.WriteLine("Неизвестная команда");
        break;
}