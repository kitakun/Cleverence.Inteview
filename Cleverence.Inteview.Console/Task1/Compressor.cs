using Cleverence.Inteview.Console.Extensions;

namespace Cleverence.Inteview.Console.Task1;

/// <summary>
/// Задача 1
/// Дана строка, содержащая n маленьких букв латинского алфавита. Требуется реализовать
/// алгоритм компрессии этой строки, замещающий группы последовательно идущих
///     одинаковых букв формой "sc" (где "s" – символ, "с" – количество букв в группе), а также
/// алгоритм декомпрессии, возвращающий исходную строку по сжатой.
///     Если буква в группе всего одна – количество в сжатой строке не указываем, а пишем её
/// как есть.
///     Пример:
/// Исходная строка: aaabbcccdde
///     Сжатая строка: a3b2c3d2e
/// </summary>
public class Compressor
{
    public void Run()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Временная сложность: O(n), где n - длина входной строки
    /// Использование памяти: O(n) для выходного буфера (худший случай - когда сжатие не происходит)
    /// </summary>
    public string Compress(string input)
    {
        if (input.Length == 1) return input;
        if (string.IsNullOrEmpty(input)) return input;

        var writerIndex = 0;
        var requiredSize = input.Length * 2;
        using var buffer = RentedArrayHelper.Rent(requiredSize);
        var resultString = buffer.AsSpan();

        for (var i = 0; i < input.Length;)
        {
            var currentChar = input[i];
            
            if (!char.IsLetter(currentChar))
            {
                resultString[writerIndex++] = currentChar;
                i++;
                continue;
            }

            var charsCount = 1;
            while (i + charsCount < input.Length && input[i + charsCount] == currentChar)
            {
                charsCount++;
            }

            resultString[writerIndex] = currentChar;
            if (charsCount > 1)
            {
                var writtenDigits = IntegerToCharConverter(charsCount, resultString[(writerIndex + 1)..]);
                writerIndex += writtenDigits + 1;
            }
            else
            {
                writerIndex++;
            }

            i += charsCount;
        }

        return new string(resultString[..writerIndex]);
    }

    /// <summary>
    /// Временная сложность: O(m), где m - длина распакованного результата
    /// Использование памяти: O(m) для выходного буфера
    /// </summary>
    public string Decompress(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        if (input.Length == 1) return input;

        var estimatedLength = CalculateDecompressedLength(input);
        using var buffer = RentedArrayHelper.Rent(estimatedLength);
        var resultString = buffer.AsSpan();

        var writerIndex = 0;
        
        for (var i = 0; i < input.Length;)
        {
            var currentChar = input[i];
            i++;
            
            if (char.IsLetter(currentChar) && i < input.Length && char.IsDigit(input[i]))
            {
                var digitStart = i;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    i++;
                }
                
                var countStr = input.AsSpan(digitStart, i - digitStart);
                if (int.TryParse(countStr, out var count))
                {
                    for (var j = 0; j < count; j++)
                    {
                        resultString[writerIndex++] = currentChar;
                    }
                }
            }
            else
            {
                resultString[writerIndex++] = currentChar;
            }
        }
        
        return new string(resultString[..writerIndex]);
    }

    /// <summary>
    /// Временная сложность: O(n), где n - длина сжатой входной строки
    /// Использование памяти: O(1) константное пространство
    /// </summary>
    private static int CalculateDecompressedLength(string input)
    {
        var totalLength = 0;
        
        for (var i = 0; i < input.Length;)
        {
            var currentChar = input[i];
            i++;
            
            if (char.IsLetter(currentChar) && i < input.Length && char.IsDigit(input[i]))
            {
                var digitStart = i;
                while (i < input.Length && char.IsDigit(input[i]))
                {
                    i++;
                }
                
                var countStr = input.AsSpan(digitStart, i - digitStart);
                if (int.TryParse(countStr, out var count))
                {
                    totalLength += count;
                }
            }
            else
            {
                totalLength++;
            }
        }
        
        return totalLength;
    }

    private static int IntegerToCharConverter(int input, Span<char> resultString)
    {
        input.TryFormat(resultString, out int charsWritten);
        return charsWritten;
    }
}
