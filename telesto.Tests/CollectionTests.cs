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
        var count = decoder.GetLength();
        count.Should().Be((uint)items.Count);
        foreach (var item in items)
        {
            decoder.Read();
            decoder.ReadInt().Should().Be(item);
        }
    }
}