using System.Collections.Concurrent;

namespace Cleverence.Inteview.Console.Task2;

public static class Server
{
    private static int _count;
    private static readonly ConcurrentQueue<int> _queue = new();
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public static async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            while (!_queue.IsEmpty)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                if (_queue.TryDequeue(out int value))
                {
                    Interlocked.Add(ref _count, value);
                }
                
                // Не блокируем другие потоки пока считаем актуальные значения
                await Task.Yield();
            }
            
            return _count;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public static void AddToCount(int count = 1)
    {
        _queue.Enqueue(count);
    }

    // For testing purposes
    internal static void Reset()
    {
        _count = 0;
        while (_queue.TryDequeue(out _)) { }
    }
}