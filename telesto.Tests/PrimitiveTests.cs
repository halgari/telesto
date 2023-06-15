using System.Runtime.InteropServices;
using FluentAssertions;

namespace telesto.Tests;

public class PrimitiveTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MinValue)]
    [InlineData(byte.MaxValue -1)]
    public void TestByte(byte value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream);
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.UInt1Byte);
        var result = decoder.ReadByte();
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(Encoder.PackedIntSize - 1)]
    [InlineData(Encoder.PackedIntSize)]
    [InlineData(ushort.MaxValue)]
    [InlineData(ushort.MinValue)]
    [InlineData(ushort.MaxValue -1)]
    [InlineData(byte.MaxValue + 1)]
    [InlineData(byte.MaxValue - 1)]
    [InlineData(byte.MaxValue)]
    public void TestUShort(ushort value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream);
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, TokenTypes.UInt2Byte);
        var result = decoder.ReadUShort();
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(Encoder.PackedIntSize - 1)]
    [InlineData(Encoder.PackedIntSize)]
    [InlineData(Encoder.PackedIntSize + 1)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MaxValue + 1)]
    [InlineData(uint.MaxValue)]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue - 1)]
    [InlineData(ushort.MaxValue + 1)]
    [InlineData(ushort.MaxValue - 1)]
    [InlineData(ushort.MaxValue)]
    public void TestUInt(uint value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream);
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
            TokenTypes.UInt2Byte, TokenTypes.UInt4Byte);
        var result = decoder.ReadUInt();
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(0, 8, 12)]
    [InlineData(0, 20, 36)]
    [InlineData(0, 10 * 1024, 30460)]
    [InlineData(0, 10 * 1024 * 1024, 52297468)]
    public void TestWritingLotsOfUInts(uint min, uint max, uint len)
    {
        using var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        for (uint i = min; i < max; i++)
        {
            encoder.Write(i);
        }

        ms.Position.Should().Be(len);
        ms.Position = 0;
        var decoder = new Decoder(ms);
        for (uint i = min; i < max; i++)
        {
            decoder.Read();
            decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
                TokenTypes.UInt2Byte, TokenTypes.UInt4Byte);
            var result = decoder.ReadUInt();
            result.Should().Be(i);
        }
    }
}