using Cleverence.Inteview.Console.Task1;

namespace Cleverence.Interview.UnitTests;

public class Task1DecompressorTests
{
    private Compressor _compressor;

    [SetUp]
    public void Setup()
    {
        _compressor = new Compressor();
    }

    [TestCase("a3b2c3d2e", "aaabbcccdde", TestName = "Пример из описания задачи")]
    [TestCase("", "", TestName = "Пустая строка")]
    [TestCase("a", "a", TestName = "Один символ")]
    [TestCase("a2", "aa", TestName = "Два одинаковых символа")]
    [TestCase("abcdef", "abcdef", TestName = "Нет сжатых символов")]
    [TestCase("a7", "aaaaaaa", TestName = "Все символы одинаковые")]
    [TestCase("ababab", "ababab", TestName = "Чередующийся паттерн")]
    [TestCase("a2bcd2e3", "aabcddeee", TestName = "Смешанные одиночные и сжатые символы")]
    [TestCase("a3b3a3", "aaabbbaaa", TestName = "Один и тот же символ в разных группах")]
    [TestCase("a4b4c4d4e4", "aaaabbbbccccddddeeee", TestName = "Длинная строка с различными группами")]
    [TestCase("a3b3z", "aaabbbz", TestName = "Строка заканчивается одиночным символом")]
    [TestCase("a3b3a", "aaabbba", TestName = "Строка начинается и заканчивается одним и тем же символом")]
    [TestCase("xyz", "xyz", TestName = "Три разных одиночных символа")]
    [TestCase("a2b2c2", "aabbcc", TestName = "Несколько пар")]
    [TestCase("a3b3c3", "aaabbbccc", TestName = "Несколько троек")]
    [TestCase("a5b6c9", "aaaaabbbbbbccccccccc", TestName = "Группы разного размера")]
    [TestCase("z10", "zzzzzzzzzz", TestName = "Десять последовательных символов")]
    [TestCase("abcdefghijklmnop", "abcdefghijklmnop", TestName = "Длинная строка без сжатия")]
    [TestCase("a2b2c2d2e2", "aabbccddee", TestName = "Все пары")]
    [TestCase("a3bc2d2e3", "aaabccddeee", TestName = "Смешанный паттерн")]
    public void Decompress_VariousInputs_ReturnsExpectedDecompression(string input, string expected)
    {
        // Act
        string result = _compressor.Decompress(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase(50, TestName = "50 последовательных символов")]
    [TestCase(100, TestName = "100 последовательных символов")]
    [TestCase(999, TestName = "999 последовательных символов")]
    [TestCase(1000, TestName = "1000 последовательных символов")]
    public void Decompress_LargeCountOfConsecutiveCharacters_ReturnsCorrectString(int count)
    {
        // Arrange
        string input = $"a{count}";
        string expected = new string('a', count);

        // Act
        string result = _compressor.Decompress(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase("aaabbcccdde", TestName = "Пример из описания задачи - круговой тест")]
    [TestCase("aaabbcccdde", TestName = "Несжатая строка - круговой тест")]
    [TestCase("abcdefghijk", TestName = "Без повторений - круговой тест")]
    [TestCase("aabbccddeeff", TestName = "Только пары - круговой тест")]
    public void CompressDecompress_RoundTrip_ReturnsOriginalString(string input)
    {
        // Act
        string compressed = _compressor.Compress(input);
        string decompressed = _compressor.Decompress(compressed);

        // Assert
        Assert.That(decompressed, Is.EqualTo(input));
    }

    [TestCase("a3", "aaa", TestName = "Простая декомпрессия числа")]
    [TestCase("a10b20", "aaaaaaaaaabbbbbbbbbbbbbbbbbbbb", TestName = "Двузначные числа")]
    [TestCase("a100", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", TestName = "Трёхзначное число")]
    [TestCase("x5y5z5", "xxxxxyyyyyzzzzz", TestName = "Несколько групп с одинаковым счетчиком")]
    public void Decompress_MultiDigitNumbers_HandlesCorrectly(string input, string expected)
    {
        // Act
        string result = _compressor.Decompress(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}

