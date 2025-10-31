using System.Buffers;

namespace Cleverence.Inteview.Console.Extensions;

internal static class RentedArrayHelper
{
    public static RentedArray Rent(int size)
    {
        return new RentedArray(size);
    }
}

internal readonly ref struct RentedArray
{
    private readonly char[] _rentedArray;
    private readonly int _size;

    public RentedArray(int size)
    {
        _size = size;
        _rentedArray = ArrayPool<char>.Shared.Rent(size);
    }

    public Span<char> AsSpan() => _rentedArray.AsSpan(0, _size);

    public void Dispose()
    {
        ArrayPool<char>.Shared.Return(_rentedArray);
    }
}