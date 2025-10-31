using BenchmarkDotNet.Attributes;

namespace Cleverence.Inteview.Console.Task1;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CompressorBenchmark
{
    private Compressor _compressor = null!;
    private string _shortString = null!;
    private string _mediumString = null!;
    private string _longString = null!;
    private string _shortCompressed = null!;
    private string _mediumCompressed = null!;
    private string _longCompressed = null!;

    [GlobalSetup]
    public void Setup()
    {
        _compressor = new Compressor();
        
        // Короткая строка: 20 символов
        _shortString = "aaabbcccddeeffgghhii";
        
        // Средняя строка: 200 символов
        _mediumString = new string('a', 50) + new string('b', 50) + 
                        new string('c', 50) + new string('d', 50);
        
        // Длинная строка: 10000 символов
        _longString = new string('a', 2500) + new string('b', 2500) + 
                      new string('c', 2500) + new string('d', 2500);

        _shortCompressed = _compressor.Compress(_shortString);
        _mediumCompressed = _compressor.Compress(_mediumString);
        _longCompressed = _compressor.Compress(_longString);
    }

    [Benchmark(Description = "Compress короткая строка (20 символов)")]
    public string CompressShort() => _compressor.Compress(_shortString);

    [Benchmark(Description = "Compress средняя строка (200 символов)")]
    public string CompressMedium() => _compressor.Compress(_mediumString);

    [Benchmark(Description = "Compress длинная строка (10000 символов)")]
    public string CompressLong() => _compressor.Compress(_longString);

    [Benchmark(Description = "Decompress короткая строка")]
    public string DecompressShort() => _compressor.Decompress(_shortCompressed);

    [Benchmark(Description = "Decompress средняя строка")]
    public string DecompressMedium() => _compressor.Decompress(_mediumCompressed);

    [Benchmark(Description = "Decompress длинная строка")]
    public string DecompressLong() => _compressor.Decompress(_longCompressed);

    [Benchmark(Description = "Round-trip короткая строка")]
    public string RoundTripShort() => _compressor.Decompress(_compressor.Compress(_shortString));

    [Benchmark(Description = "Round-trip средняя строка")]
    public string RoundTripMedium() => _compressor.Decompress(_compressor.Compress(_mediumString));
}

