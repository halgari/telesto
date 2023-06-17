using FluentAssertions;

namespace telesto.Tests;

public class CollectionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MaxValue+1)]
    [InlineData(ushort.MaxValue)]
    [InlineData(ushort.MaxValue+1)]
    public void ListTests(int len)
    {
        var items = Enumerable.Range(0, len).ToList();
        var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        encoder.WriteStartList(items.Count);
        foreach (var item in items)
        {
            encoder.Write(item);
        }
        
        ms.Position = 0;
        var decoder = new Decoder(ms.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.StartList);
        var count = decoder.GetLength();
        count.Should().Be((uint)items.Count);
        foreach (var item in items)
        {
            decoder.Read();
            decoder.ReadInt().Should().Be(item);
        }
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MaxValue+1)]
    [InlineData(ushort.MaxValue)]
    [InlineData(ushort.MaxValue+1)]
    public void DictionaryTests(int len)
    {
        var items = Enumerable.Range(0, len).ToDictionary(x => x, x => x.ToString());
        var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        encoder.WriteStartDictionary(items.Count);
        foreach (var (key, value) in items)
        {
            encoder.Write(key);
            encoder.Write(value);
        }
        
        ms.Position = 0;
        var decoder = new Decoder(ms.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.StartDictionary);
        var count = decoder.GetLength();
        count.Should().Be((uint)items.Count);
        foreach (var (key, value) in items)
        {
            decoder.Read();
            decoder.ReadInt().Should().Be(key);
            decoder.Read();
            decoder.ReadString().Should().Be(value);
        }
    }
}