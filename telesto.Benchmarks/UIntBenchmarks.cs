using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using BenchmarkDotNet.Attributes;

namespace telesto.Benchmarks;

[MemoryDiagnoser]
public class PrimitiveBenchmarks
{
    private readonly MemoryStream _telestoWrite;
    private readonly MemoryStream _binaryWrite;
    private readonly MemoryStream _jsonWrite;


    [Params(16, 1024)] 
    public int Size { get; set; } = 0;

    public uint[] Data { get; set; } = Array.Empty<uint>();

    public PrimitiveBenchmarks()
    {
        _telestoWrite = new MemoryStream();
        _binaryWrite = new MemoryStream();
        _jsonWrite = new MemoryStream();
    }

    [GlobalSetup]
    public void SetupData()
    {
        Data = new uint[Size];
        for (var x = 0; x < Size; x++)
        {
            Data[x] = (uint)Random.Shared.Next(0, int.MaxValue);
        }
        WriteTelesto(_telestoWrite);
        WriteNative(_binaryWrite);
        WriteJson(_jsonWrite);
    }

    [Benchmark]
    public void Read_Native()
    {
        _binaryWrite.Position = 0;
        using var reader = new BinaryReader(_binaryWrite, Encoding.UTF8, true);
        var data = Data;
        foreach (var value in data)
        {
            reader.ReadUInt32();
        }
    }
    
    [Benchmark]
    public void Read_Telesto()
    {
        _telestoWrite.Position = 0;
        var reader = new Decoder(_telestoWrite);
        var data = Data;
        foreach (var value in data)
        {
            reader.Read();
            reader.ReadUInt();
        }
    }

    [Benchmark]
    public void Read_Json()
    {
        _jsonWrite.Position = 0;
        var reader = new Utf8JsonReader(_jsonWrite.GetBuffer());
        reader.Read();
        var data = Data;
        foreach (var value in data)
        {
            reader.Read();
            reader.GetUInt32();
        }
    }

    [Benchmark]
    public void Write_Native()
    {
        WriteNative(_binaryWrite);
    }
    
    [Benchmark]
    public void Write_Telesto()
    {
        WriteTelesto(_telestoWrite);
    }
    
    [Benchmark]
    public void Write_Json()
    {
        WriteJson(_jsonWrite);
    }

    private void WriteJson(Stream stream)
    {
        stream.Position = 0;
        using var writer = new Utf8JsonWriter(stream);
        writer.WriteStartArray();
        var data = Data;
        foreach (var value in data)
        {
            writer.WriteNumberValue(value);
        }
        writer.WriteEndArray();
    }

    private void WriteNative(Stream stream)
    {
        stream.Position = 0;
        using var writer = new BinaryWriter(stream, Encoding.UTF8, true);
        var data = Data;
        foreach (var value in data)
        {
            writer.Write(value);
        }
    }

    private void WriteTelesto(Stream stream)
    {
        stream.Position = 0;
        var writer = new Encoder(stream);
        var data = Data;
        foreach (var value in data)
        {
            writer.Write(value);
        }
    }
}