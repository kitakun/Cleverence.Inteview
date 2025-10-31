using Cleverence.Inteview.Console.Task1;

namespace Cleverence.Interview.UnitTests;

public class Task1CompressorTests
{
    private Compressor _compressor;

    [SetUp]
    public void Setup()
    {
        _compressor = new Compressor();
    }

    [TestCase("aaabbcccdde", "a3b2c3d2e", TestName = "Пример из описания задачи")]
    [TestCase("", "", TestName = "Пустая строка")]
    [TestCase("a", "a", TestName = "Один символ")]
    [TestCase("aa", "a2", TestName = "Два одинаковых символа подряд")]
    [TestCase("abcdef", "abcdef", TestName = "Нет повторяющихся символов")]
    [TestCase("aaaaaaa", "a7", TestName = "Все символы одинаковые")]
    [TestCase("ababab", "ababab", TestName = "Чередующийся паттерн")]
    [TestCase("aabcddeee", "a2bcd2e3", TestName = "Смешанные одиночные и множественные вхождения")]
    [TestCase("aaabbbaaa", "a3b3a3", TestName = "Один и тот же символ в разных группах")]
    [TestCase("aaaabbbbccccddddeeee", "a4b4c4d4e4", TestName = "Длинная строка с различными паттернами")]
    [TestCase("aaabbbz", "a3b3z", TestName = "Строка заканчивается одиночным символом")]
    [TestCase("aaabbba", "a3b3a", TestName = "Строка начинается и заканчивается одним и тем же символом")]
    [TestCase("xyz", "xyz", TestName = "Три разных одиночных символа")]
    [TestCase("aabbcc", "a2b2c2", TestName = "Несколько пар")]
    [TestCase("aaabbbccc", "a3b3c3", TestName = "Несколько троек")]
    [TestCase("aaaaabbbbbbccccccccc", "a5b6c9", TestName = "Группы разного размера")]
    [TestCase("zzzzzzzzzz", "z10", TestName = "Десять последовательных символов")]
    [TestCase("abcdefghijklmnop", "abcdefghijklmnop", TestName = "Длинная строка без повторений")]
    [TestCase("aabbccddee", "a2b2c2d2e2", TestName = "Все пары")]
    [TestCase("aaabccddeee", "a3bc2d2e3", TestName = "Смешанный паттерн сжатия")]
    public void Compress_VariousInputs_ReturnsExpectedCompression(string input, string expected)
    {
        // Act
        string result = _compressor.Compress(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    [TestCase(50, "a50", TestName = "50 последовательных символов")]
    [TestCase(100, "a100", TestName = "100 последовательных символов")]
    [TestCase(999, "a999", TestName = "999 последовательных символов")]
    [TestCase(1000, "a1000", TestName = "1000 последовательных символов")]
    public void Compress_LargeCountOfConsecutiveCharacters_ReturnsCorrectCount(int count, string expected)
    {
        // Arrange
        string input = new string('a', count);

        // Act
        string result = _compressor.Compress(input);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }
}