using Cleverence.Inteview.Console.Task3;
using Cleverence.Inteview.Console.Task3.Models;
using Cleverence.Inteview.Console.Task3.Parsers;

namespace Cleverence.Interview.UnitTests;

public class Task3LogStandardizerTests
{
    private LogParser _parser = null!;
    private Format1Parser _format1Parser = null!;
    private Format2Parser _format2Parser = null!;

    [SetUp]
    public void Setup()
    {
        _parser = new LogParser();
        _format1Parser = new Format1Parser();
        _format2Parser = new Format2Parser();
    }

    #region LogParser Format1 Tests

    [Test]
    public void ParseFormat1_ValidLine_ReturnsLogRecord()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";

        // Act
        var result = _format1Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Date.Day, Is.EqualTo(10));
        Assert.That(result.Date.Month, Is.EqualTo(3));
        Assert.That(result.Date.Year, Is.EqualTo(2025));
        Assert.That(result.Time, Is.EqualTo("15:14:49.523"));
        Assert.That(result.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.INFO));
        Assert.That(result.CallingMethod, Is.EqualTo("DEFAULT"));
        Assert.That(result.Message, Is.EqualTo("Версия программы: '3.4.0.48729'"));
    }

    [Test]
    public void ParseFormat1_InformationLevel_MapsToInfo()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 INFORMATION Test message";

        // Act
        var result = _format1Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.INFO), "INFORMATION должен маппиться в INFO");
    }

    [Test]
    public void ParseFormat1_WarningLevel_MapsToWarn()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 WARNING Test warning";

        // Act
        var result = _format1Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.WARN), "WARNING должен маппиться в WARN");
    }

    [Test]
    public void ParseFormat1_ErrorLevel_ReturnsError()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 ERROR Test error";

        // Act
        var result = _format1Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.ERROR));
    }

    [Test]
    public void ParseFormat1_DebugLevel_ReturnsDebug()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 DEBUG Test debug";

        // Act
        var result = _format1Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.DEBUG));
    }

    [Test]
    public void ParseFormat1_InvalidFormat_ThrowsException()
    {
        // Arrange
        var line = "Invalid log line without proper format";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format1Parser.Parse(line), 
            "Невалидный формат должен выбросить FormatException");
    }

    [Test]
    public void ParseFormat1_EmptyLine_ThrowsException()
    {
        // Arrange
        var line = "";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format1Parser.Parse(line), 
            "Пустая строка должна выбросить FormatException");
    }

    [Test]
    public void ParseFormat1_InvalidDate_ThrowsException()
    {
        // Arrange
        var line = "32.13.2025 15:14:49.523 INFO Test";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format1Parser.Parse(line), 
            "Невалидная дата должна выбросить FormatException");
    }

    [Test]
    public void ParseFormat1_UnknownLogLevel_ThrowsException()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 UNKNOWN Test";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format1Parser.Parse(line), 
            "Неизвестный уровень логирования должен выбросить FormatException");
    }

    #endregion

    #region LogParser Format2 Tests

    [Test]
    public void ParseFormat2_ValidLine_ReturnsLogRecord()
    {
        // Arrange
        var line = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        // Act
        var result = _format2Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Date.Day, Is.EqualTo(10));
        Assert.That(result.Date.Month, Is.EqualTo(3));
        Assert.That(result.Date.Year, Is.EqualTo(2025));
        Assert.That(result.Time, Is.EqualTo("15:14:51.5882"));
        Assert.That(result.ParsedLogLevel, Is.EqualTo(ParsedLogLevel.INFO));
        Assert.That(result.CallingMethod, Is.EqualTo("MobileComputer.GetDeviceId"));
        Assert.That(result.Message, Is.EqualTo("Код устройства: '@MINDEO-M40-D-410244015546'"));
    }

    [Test]
    public void ParseFormat2_ExtractsCallingMethod()
    {
        // Arrange
        var line = "2025-03-10 15:14:51.5882| INFO|11|TestClass.TestMethod| Message";

        // Act
        var result = _format2Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CallingMethod, Is.EqualTo("TestClass.TestMethod"), "Метод должен быть извлечен");
    }

    [Test]
    public void ParseFormat2_IgnoresThreadId()
    {
        // Arrange
        var line = "2025-03-10 15:14:51.5882| INFO|999|TestMethod| Message";

        // Act
        var result = _format2Parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null, "ThreadID должен игнорироваться");
    }

    [Test]
    public void ParseFormat2_InvalidFormat_ThrowsException()
    {
        // Arrange
        var line = "Invalid format without pipes";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format2Parser.Parse(line), 
            "Невалидный формат должен выбросить FormatException");
    }

    [Test]
    public void ParseFormat2_InvalidDate_ThrowsException()
    {
        // Arrange
        var line = "2025-13-32 15:14:51.5882| INFO|11|Method| Message";

        // Act & Assert
        Assert.Throws<FormatException>(() => _format2Parser.Parse(line), 
            "Невалидная дата должна выбросить FormatException");
    }

    #endregion

    #region LogParser Parse Tests

    [Test]
    public void Parse_Format1_ReturnsRecord()
    {
        // Arrange
        var line = "10.03.2025 15:14:49.523 INFO Test";

        // Act
        var result = _parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CallingMethod, Is.EqualTo("DEFAULT"), "Формат 1 должен использовать DEFAULT");
    }

    [Test]
    public void Parse_Format2_ReturnsRecord()
    {
        // Arrange
        var line = "2025-03-10 15:14:51.5882| INFO|11|TestMethod| Message";

        // Act
        var result = _parser.Parse(line);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.CallingMethod, Is.EqualTo("TestMethod"), "Формат 2 должен извлекать метод");
    }

    [Test]
    public void Parse_InvalidFormat_ThrowsException()
    {
        // Arrange
        var line = "Completely invalid log line";

        // Act & Assert
        Assert.Throws<FormatException>(() => _parser.Parse(line), 
            "Невалидная строка должна выбросить FormatException");
    }

    [Test]
    public void Parse_UsesCouldBeParsedToSelectParser()
    {
        // Arrange
        var validLine = "10.03.2025 15:14:49.523 INFO Test";
        var invalidLine = "Invalid line";

        // Act
        var validResult = _parser.Parse(validLine);

        // Assert
        Assert.That(validResult, Is.Not.Null, "Валидная строка должна быть распарсена");
        Assert.That(_parser.CouldBeParsed(validLine), Is.True, "CouldBeParsed должен вернуть true для валидной строки");
        Assert.That(_parser.CouldBeParsed(invalidLine), Is.False, "CouldBeParsed должен вернуть false для невалидной строки");
        Assert.Throws<FormatException>(() => _parser.Parse(invalidLine), "Parse должен выбросить исключение для невалидной строки");
    }

    #endregion

    #region LogFormatter Tests

    [Test]
    public void FormatRecord_CorrectFormat_WithTabs()
    {
        // Arrange
        var record = new LogRecord(
            new DateTime(2025, 3, 10),
            "15:14:49.523",
            ParsedLogLevel.INFO,
            "TestMethod",
            "Test message"
        );

        // Act
        var result = LogFormatter.FormatRecord(record);

        // Assert
        Assert.That(result, Is.EqualTo("10-03-2025\t15:14:49.523\tINFO\tTestMethod\tTest message"));
    }

    [Test]
    public void FormatRecord_DateFormat_IsDDMMYYYY()
    {
        // Arrange
        var record = new LogRecord(
            new DateTime(2025, 12, 31),
            "23:59:59.999",
            ParsedLogLevel.ERROR,
            "DEFAULT",
            "End of year"
        );

        // Act
        var result = LogFormatter.FormatRecord(record);

        // Assert
        Assert.That(result, Does.StartWith("31-12-2025"), "Дата должна быть в формате DD-MM-YYYY");
    }

    [Test]
    public void FormatRecord_PreservesTime()
    {
        // Arrange
        var record = new LogRecord(
            new DateTime(2025, 1, 1),
            "12:34:56.789",
            ParsedLogLevel.DEBUG,
            "DEFAULT",
            "Test"
        );

        // Act
        var result = LogFormatter.FormatRecord(record);

        // Assert
        Assert.That(result, Does.Contain("12:34:56.789"), "Время должно сохраняться в исходном формате");
    }

    [Test]
    public void FormatRecord_AllLogLevels_FormatCorrectly()
    {
        // Arrange & Act & Assert
        var levels = new[] { ParsedLogLevel.INFO, ParsedLogLevel.WARN, ParsedLogLevel.ERROR, ParsedLogLevel.DEBUG };
        
        foreach (var level in levels)
        {
            var record = new LogRecord(
                new DateTime(2025, 1, 1),
                "00:00:00.000",
                level,
                "DEFAULT",
                "Test"
            );
            
            var result = LogFormatter.FormatRecord(record);
            Assert.That(result, Does.Contain(level.ToString()), $"Уровень {level} должен присутствовать в выводе");
        }
    }

    #endregion

    #region LogStandardizer Integration Tests

    [Test]
    public async Task ProcessLogFile_MixedFormats_ProcessesCorrectly()
    {
        // Arrange
        var inputPath = Path.GetTempFileName();
        var outputPath = Path.GetTempFileName();
        var problemsPath = Path.Combine(Path.GetDirectoryName(outputPath)!, "problems.txt");

        var inputLines = new[]
        {
            "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
            "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'",
            "Invalid log line",
            "11.03.2025 16:20:30.100 WARNING Предупреждение",
            "2025-03-11 16:25:00.000| ERROR|22|ErrorHandler.Handle| Ошибка обработки"
        };

        await File.WriteAllLinesAsync(inputPath, inputLines);

        try
        {
            var standardizer = new LogStandardizer();

            // Act
            await standardizer.ProcessLogFileAsync(inputPath, outputPath);

            // Assert
            var outputLines = await File.ReadAllLinesAsync(outputPath);
            Assert.That(outputLines.Length, Is.EqualTo(4), "Должно быть обработано 4 валидных записи");

            var problemLines = await File.ReadAllLinesAsync(problemsPath);
            Assert.That(problemLines.Length, Is.EqualTo(1), "Должна быть 1 невалидная запись");
            Assert.That(problemLines[0], Is.EqualTo("Invalid log line"), "Невалидная запись должна быть сохранена в исходном виде");

            // Проверяем формат первой строки
            var firstLine = outputLines[0];
            Assert.That(firstLine, Does.StartWith("10-03-2025\t"), "Первая строка должна начинаться с даты в формате DD-MM-YYYY");
            Assert.That(firstLine, Does.Contain("\tINFO\t"), "INFORMATION должен быть преобразован в INFO");
            Assert.That(firstLine, Does.Contain("\tDEFAULT\t"), "Формат 1 должен использовать DEFAULT");
        }
        finally
        {
            // Cleanup
            if (File.Exists(inputPath)) File.Delete(inputPath);
            if (File.Exists(outputPath)) File.Delete(outputPath);
            if (File.Exists(problemsPath)) File.Delete(problemsPath);
        }
    }

    [Test]
    public async Task ProcessLogFile_EmptyLines_AreIgnored()
    {
        // Arrange
        var inputPath = Path.GetTempFileName();
        var outputPath = Path.GetTempFileName();

        var inputLines = new[]
        {
            "10.03.2025 15:14:49.523 INFO Test",
            "",
            "   ",
            "11.03.2025 15:14:49.523 INFO Test2"
        };

        await File.WriteAllLinesAsync(inputPath, inputLines);

        try
        {
            var standardizer = new LogStandardizer();

            // Act
            await standardizer.ProcessLogFileAsync(inputPath, outputPath);

            // Assert
            var outputLines = await File.ReadAllLinesAsync(outputPath);
            Assert.That(outputLines.Length, Is.EqualTo(2), "Пустые строки должны игнорироваться");
        }
        finally
        {
            if (File.Exists(inputPath)) File.Delete(inputPath);
            if (File.Exists(outputPath)) File.Delete(outputPath);
            if (File.Exists(Path.Combine(Path.GetDirectoryName(outputPath)!, "problems.txt")))
                File.Delete(Path.Combine(Path.GetDirectoryName(outputPath)!, "problems.txt"));
        }
    }

    [Test]
    public void ProcessLogFile_NonExistentFile_ThrowsException()
    {
        // Arrange
        var standardizer = new LogStandardizer();
        var nonExistentPath = "non_existent_file_12345.log";
        var outputPath = Path.GetTempFileName();

        // Act & Assert
        Assert.ThrowsAsync<FileNotFoundException>(
            async () => await standardizer.ProcessLogFileAsync(nonExistentPath, outputPath),
            "Должно быть выброшено исключение для несуществующего файла"
        );
    }

    [Test]
    public async Task ProcessLogFile_AllValidRecords_NoProblemsFile()
    {
        // Arrange
        var inputPath = Path.GetTempFileName();
        var outputPath = Path.GetTempFileName();
        var problemsPath = Path.Combine(Path.GetDirectoryName(outputPath)!, "problems.txt");

        var inputLines = new[]
        {
            "10.03.2025 15:14:49.523 INFO Test1",
            "11.03.2025 15:14:49.523 WARN Test2",
            "2025-03-12 15:14:51.5882| ERROR|11|Method| Test3"
        };

        await File.WriteAllLinesAsync(inputPath, inputLines);

        try
        {
            var standardizer = new LogStandardizer();

            // Act
            await standardizer.ProcessLogFileAsync(inputPath, outputPath);

            // Assert
            var outputLines = await File.ReadAllLinesAsync(outputPath);
            Assert.That(outputLines.Length, Is.EqualTo(3), "Все записи должны быть обработаны");

            // Problems file should exist but be empty or have no content
            if (File.Exists(problemsPath))
            {
                var problemLines = await File.ReadAllLinesAsync(problemsPath);
                Assert.That(problemLines.Length, Is.EqualTo(0), "Файл проблем должен быть пустым");
            }
        }
        finally
        {
            if (File.Exists(inputPath)) File.Delete(inputPath);
            if (File.Exists(outputPath)) File.Delete(outputPath);
            if (File.Exists(problemsPath)) File.Delete(problemsPath);
        }
    }

    #endregion
}

