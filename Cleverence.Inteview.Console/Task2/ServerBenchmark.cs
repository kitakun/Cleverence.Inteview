using BenchmarkDotNet.Attributes;

namespace Cleverence.Inteview.Console.Task2;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class ServerBenchmark
{
    [GlobalSetup]
    public void Setup()
    {
        Server.Reset();
    }

    [IterationSetup]
    public void IterationSetup()
    {
        Server.Reset();
    }

    [Benchmark(Description = "Sequential: 10 операций Add + GetCount")]
    public async Task<int> SequentialSmall()
    {
        for (var i = 0; i < 10; i++)
        {
            Server.AddToCount(1);
        }
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Sequential: 100 операций Add + GetCount")]
    public async Task<int> SequentialMedium()
    {
        for (var i = 0; i < 100; i++)
        {
            Server.AddToCount(1);
        }
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Sequential: 1000 операций Add + GetCount")]
    public async Task<int> SequentialLarge()
    {
        for (var i = 0; i < 1000; i++)
        {
            Server.AddToCount(1);
        }
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Parallel: 10 потоков x 10 операций Add")]
    public async Task<int> ParallelSmall()
    {
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < 10; i++)
                {
                    Server.AddToCount(1);
                }
            }))
            .ToArray();

        await Task.WhenAll(tasks);
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Parallel: 20 потоков x 50 операций Add")]
    public async Task<int> ParallelMedium()
    {
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < 50; i++)
                {
                    Server.AddToCount(1);
                }
            }))
            .ToArray();

        await Task.WhenAll(tasks);
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Parallel: 50 потоков x 100 операций Add")]
    public async Task<int> ParallelLarge()
    {
        var tasks = Enumerable.Range(0, 50)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < 100; i++)
                {
                    Server.AddToCount(1);
                }
            }))
            .ToArray();

        await Task.WhenAll(tasks);
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Concurrent GetCount: 5 параллельных вызовов")]
    public async Task<int[]> ConcurrentGetCountSmall()
    {
        // Подготовка данных
        for (var i = 0; i < 100; i++)
        {
            Server.AddToCount(1);
        }

        // Параллельные вызовы GetCount
        var tasks = Enumerable.Range(0, 5)
            .Select(_ => Server.GetCountAsync())
            .ToArray();

        return await Task.WhenAll(tasks);
    }

    [Benchmark(Description = "Concurrent GetCount: 10 параллельных вызовов")]
    public async Task<int[]> ConcurrentGetCountMedium()
    {
        // Подготовка данных
        for (var i = 0; i < 100; i++)
        {
            Server.AddToCount(1);
        }

        // Параллельные вызовы GetCount
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Server.GetCountAsync())
            .ToArray();

        return await Task.WhenAll(tasks);
    }

    [Benchmark(Description = "Mixed: Одновременные Add и GetCount")]
    public async Task<int> MixedOperations()
    {
        // Параллельные Add операции
        var addTask = Task.Run(() =>
        {
            for (var i = 0; i < 100; i++)
            {
                Server.AddToCount(1);
            }
        });

        // Подождем немного и вызовем GetCount
        await Task.Delay(5);
        var getTask = Server.GetCountAsync();

        await addTask;
        var result = await getTask;

        // Финальный GetCount для всех элементов
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "High contention: 100 потоков конкурируют")]
    public async Task<int> HighContention()
    {
        var tasks = new List<Task>();

        // 50 потоков добавляют данные
        for (var i = 0; i < 50; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (var j = 0; j < 10; j++)
                {
                    Server.AddToCount(1);
                }
            }));
        }

        // 50 потоков пытаются получить счетчик
        for (var i = 0; i < 50; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Server.GetCountAsync();
            }));
        }

        await Task.WhenAll(tasks);
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Burst: Быстрое добавление пачки элементов")]
    public async Task<int> BurstOperations()
    {
        // Быстрое добавление 1000 элементов
        for (var i = 0; i < 1000; i++)
        {
            Server.AddToCount(1);
        }

        // Один GetCount для обработки всей очереди
        return await Server.GetCountAsync();
    }

    [Benchmark(Description = "Multiple GetCount: 10 последовательных вызовов")]
    public async Task<int> MultipleSequentialGetCount()
    {
        var result = 0;
        
        for (var i = 0; i < 10; i++)
        {
            Server.AddToCount(10);
            result = await Server.GetCountAsync();
        }

        return result;
    }
}

