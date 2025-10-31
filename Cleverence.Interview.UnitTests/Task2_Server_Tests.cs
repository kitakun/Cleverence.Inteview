using Cleverence.Inteview.Console.Task2;

namespace Cleverence.Interview.UnitTests;

public class Task2ServerTests
{
    [SetUp]
    public void Setup()
    {
        Server.Reset();
    }

    [TearDown]
    public void TearDown()
    {
        Server.Reset();
    }

    [Test]
    public async Task GetCount_WithEmptyQueue_ReturnsZero()
    {
        // Act
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCount_WithSingleItem_ReturnsCorrectCount()
    {
        // Arrange
        Server.AddToCount(5);

        // Act
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(5));
    }

    [Test]
    public async Task GetCount_WithMultipleItems_ReturnsSum()
    {
        // Arrange
        Server.AddToCount();
        Server.AddToCount(2);
        Server.AddToCount(3);
        Server.AddToCount(4);

        // Act
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(10));
    }

    [Test]
    public async Task GetCount_CalledMultipleTimes_AccumulatesValues()
    {
        // Arrange & Act
        Server.AddToCount(5);
        var firstResult = await Server.GetCountAsync();

        Server.AddToCount(3);
        var secondResult = await Server.GetCountAsync();

        Server.AddToCount(2);
        var thirdResult = await Server.GetCountAsync();

        // Assert
        Assert.That(firstResult, Is.EqualTo(5));
        Assert.That(secondResult, Is.EqualTo(8));
        Assert.That(thirdResult, Is.EqualTo(10));
    }

    [Test]
    public async Task GetCount_WithDefaultParameter_UsesDefaultValue()
    {
        // Arrange
        Server.AddToCount();
        Server.AddToCount();
        Server.AddToCount();

        // Act
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(3));
    }

    [Test]
    public async Task GetCount_WaitsForAllQueueItems_BeforeReturning()
    {
        // Arrange
        Server.AddToCount();
        Server.AddToCount(2);
        Server.AddToCount(3);

        // Act
        var getTask = Server.GetCountAsync();

        var result = await getTask;

        // Assert
        Assert.That(result, Is.EqualTo(6));

        var secondResult = await Server.GetCountAsync();
        Assert.That(secondResult, Is.EqualTo(6), "Счётчик должен остаться прежним, когда очередь пуста");
    }

    [Test]
    public void GetCount_WithCancellationToken_CanBeCancelled()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        
        for (var i = 0; i < 100; i++)
        {
            Server.AddToCount();
        }

        // Act & Assert
        cts.Cancel();
        Assert.ThrowsAsync<TaskCanceledException>(async () => 
            await Server.GetCountAsync(cts.Token));
    }

    [Test]
    public async Task GetCount_ParallelCalls_AreThreadSafe()
    {
        // Arrange
        const int numberOfTasks = 10;
        const int itemsPerTask = 5;

        var addTasks = Enumerable.Range(0, numberOfTasks)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < itemsPerTask; i++)
                {
                    Server.AddToCount();
                }
            }))
            .ToArray();

        await Task.WhenAll(addTasks);

        // Act
        var getTasks = Enumerable.Range(0, 5)
            .Select(_ => Server.GetCountAsync())
            .ToArray();

        var results = await Task.WhenAll(getTasks);

        // Assert
        Assert.That(results.All(r => r == numberOfTasks * itemsPerTask), Is.True);
        Assert.That(results[0], Is.EqualTo(50));
    }

    [Test]
    public async Task GetCount_ConcurrentAddAndGet_MaintainsConsistency()
    {
        // Arrange & Act
        var addTask = Task.Run(async () =>
        {
            for (var i = 0; i < 100; i++)
            {
                Server.AddToCount();
                await Task.Delay(1);
            }
        });

        var getTask = Task.Run(async () =>
        {
            await Task.Delay(50);
            return await Server.GetCountAsync();
        });

        await addTask;
        var result = await getTask;

        var finalCount = await Server.GetCountAsync();

        // Assert
        Assert.That(finalCount, Is.EqualTo(100));
        Assert.That(result, Is.LessThanOrEqualTo(100));
        Assert.That(result, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task AddToCount_WithNegativeValue_SubtractsFromCount()
    {
        // Arrange
        Server.AddToCount(10);
        await Server.GetCountAsync();

        // Act
        Server.AddToCount(-3);
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(7));
    }

    [Test]
    public async Task GetCount_WithLargeValues_HandlesCorrectly()
    {
        // Arrange
        Server.AddToCount(1000000);
        Server.AddToCount(2000000);
        Server.AddToCount(3000000);

        // Act
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(6000000));
    }

    [Test]
    public async Task GetCount_MultipleThreadsWithSemaphore_OnlyOneProcessesAtTime()
    {
        // Arrange
        const int numberOfThreads = 10;
        for (var i = 0; i < 100; i++)
        {
            Server.AddToCount();
        }

        // Act
        var tasks = Enumerable.Range(0, numberOfThreads)
            .Select(_ => Task.Run(async () => await Server.GetCountAsync()))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.That(results.Distinct().Count(), Is.EqualTo(1), 
            "Все параллельные вызовы GetCount должны вернуть одинаковое значение");
        Assert.That(results[0], Is.EqualTo(100));
    }

    [Test]
    public async Task GetCount_WithItemsAddedDuringProcessing_ProcessesAll()
    {
        // Arrange
        Server.AddToCount();
        Server.AddToCount(2);

        // Act
        var getCountTask = Server.GetCountAsync();

        await Task.Delay(10);
        Server.AddToCount(3);

        var firstResult = await getCountTask;
        var secondResult = await Server.GetCountAsync();

        // Assert
        Assert.That(firstResult, Is.EqualTo(3), "Первый вызов должен обработать элементы 1 и 2");
        Assert.That(secondResult, Is.EqualTo(6), "Второй вызов должен обработать элемент 3 и вернуть итог");
    }

    [Test]
    public async Task GetCount_CalledSequentially_MaintainsAccumulatedState()
    {
        // Arrange & Act
        Server.AddToCount(5);
        var count1 = await Server.GetCountAsync();

        Server.AddToCount(10);
        var count2 = await Server.GetCountAsync();

        Server.AddToCount(15);
        var count3 = await Server.GetCountAsync();

        var count4 = await Server.GetCountAsync();

        // Assert
        Assert.That(count1, Is.EqualTo(5));
        Assert.That(count2, Is.EqualTo(15));
        Assert.That(count3, Is.EqualTo(30));
        Assert.That(count4, Is.EqualTo(30), "Счётчик должен остаться без изменений, когда новые элементы не добавлены");
    }

    [Test]
    public async Task GetCount_StressTest_HandlesHighConcurrency()
    {
        // Arrange
        const int numberOfAddThreads = 20;
        const int numberOfGetThreads = 10;
        const int itemsPerThread = 50;
        var expectedTotal = numberOfAddThreads * itemsPerThread;

        // Act
        var addTasks = Enumerable.Range(0, numberOfAddThreads)
            .Select(_ => Task.Run(() =>
            {
                for (var i = 0; i < itemsPerThread; i++)
                {
                    Server.AddToCount();
                }
            }))
            .ToArray();

        await Task.WhenAll(addTasks);

        var getTasks = Enumerable.Range(0, numberOfGetThreads)
            .Select(_ => Task.Run(async () => await Server.GetCountAsync()))
            .ToArray();

        var results = await Task.WhenAll(getTasks);

        // Assert
        Assert.That(results.All(r => r == expectedTotal), Is.True);
        Assert.That(results[0], Is.EqualTo(1000));
    }

    [Test]
    public void Reset_ClearsCountAndQueue()
    {
        // Arrange
        Server.AddToCount(10);
        Server.AddToCount(20);

        // Act
        Server.Reset();

        // Assert
        var result = Server.GetCountAsync().Result;
        Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public async Task GetCount_WithZeroValue_AddsZero()
    {
        // Arrange
        Server.AddToCount(10);
        await Server.GetCountAsync();

        // Act
        Server.AddToCount(0);
        var result = await Server.GetCountAsync();

        // Assert
        Assert.That(result, Is.EqualTo(10), "Добавление нуля не должно изменять счётчик");
    }
}

