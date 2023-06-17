using System.Runtime.InteropServices;
using System.Text;
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
        var decoder = new Decoder(stream.GetBuffer());
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
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, TokenTypes.UInt2Byte);
        var result = decoder.ReadUShort();
        result.Should().Be(value);
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(Encoder.PackedIntSize - 1)]
    [InlineData(Encoder.PackedIntSize)]
    [InlineData(short.MaxValue)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue -1)]
    [InlineData(byte.MaxValue + 1)]
    [InlineData(byte.MaxValue - 1)]
    [InlineData(byte.MaxValue)]
    [InlineData(-short.MaxValue -1)]
    [InlineData(-byte.MaxValue + 1)]
    [InlineData(-byte.MaxValue - 1)]
    [InlineData(-byte.MaxValue)]
    public void TestShort(short value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, TokenTypes.UInt2Byte, TokenTypes.Int2Byte);
        var result = decoder.ReadShort();
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
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
            TokenTypes.UInt2Byte, TokenTypes.UInt4Byte);
        var result = decoder.ReadUInt();
        result.Should().Be(value);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(Encoder.PackedIntSize - 1)]
    [InlineData(Encoder.PackedIntSize)]
    [InlineData(Encoder.PackedIntSize + 1)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MaxValue + 1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue - 1)]
    [InlineData(-byte.MaxValue)]
    [InlineData(-byte.MaxValue + 1)]
    [InlineData(ushort.MaxValue + 1)]
    [InlineData(ushort.MaxValue - 1)]
    [InlineData(ushort.MaxValue)]
    [InlineData(-ushort.MaxValue + 1)]
    [InlineData(-ushort.MaxValue - 1)]
    [InlineData(-ushort.MaxValue)]
    public void TestInt(int value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
            TokenTypes.UInt2Byte, TokenTypes.UInt4Byte,
            TokenTypes.Int2Byte, TokenTypes.Int4Byte);
        var result = decoder.ReadInt();
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
    [InlineData(ulong.MaxValue)]
    [InlineData(ulong.MaxValue - 1)]
    public void TestULong(ulong value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
            TokenTypes.UInt2Byte, TokenTypes.UInt4Byte, TokenTypes.UInt8Byte);
        var result = decoder.ReadULong();
        result.Should().Be(value);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(Encoder.PackedIntSize - 1)]
    [InlineData(Encoder.PackedIntSize)]
    [InlineData(Encoder.PackedIntSize + 1)]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MaxValue + 1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue - 1)]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue - 1)]
    [InlineData(-byte.MaxValue)]
    [InlineData(-byte.MaxValue + 1)]
    [InlineData(ushort.MaxValue + 1)]
    [InlineData(ushort.MaxValue - 1)]
    [InlineData(ushort.MaxValue)]
    [InlineData(-ushort.MaxValue + 1)]
    [InlineData(-ushort.MaxValue - 1)]
    [InlineData(-ushort.MaxValue)]
    public void TestLong(long value)
    {
        var stream = new MemoryStream();
        var encoder = new Encoder(stream);
        encoder.Write(value);
        stream.Position = 0;
        var decoder = new Decoder(stream.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
            TokenTypes.UInt2Byte, TokenTypes.UInt4Byte,
            TokenTypes.Int2Byte, TokenTypes.Int4Byte,
            TokenTypes.Int8Byte);
        var result = decoder.ReadLong();
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
        var decoder = new Decoder(ms.GetBuffer());
        for (uint i = min; i < max; i++)
        {
            decoder.Read();
            decoder.GetTokenType().Should().BeOneOf(TokenTypes.UInt1Byte, 
                TokenTypes.UInt2Byte, TokenTypes.UInt4Byte);
            var result = decoder.ReadUInt();
            result.Should().Be(i);
        }
    }

    [Theory]
    [InlineData("85D44B22-33DD-423F-AA07-B4415BABE248")]
    [InlineData("86484847-BD0F-471D-9504-8AD3DEF2ED4D")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")]
    public void TestGuids(string stringGuid)
    {
        var guid = Guid.Parse(stringGuid);
        using var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        encoder.Write(guid);
        ms.Position = 0;
        var decoder = new Decoder(ms.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.Guid);
        var result = decoder.ReadGuid();
        result.Should().Be(guid);
    }

    [Theory]
    [MemberData(nameof(RandomStrings))]
    public void TestString(string data, int length)
    {
        using var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        encoder.Write(data);
        ms.Position = 0;
        var decoder = new Decoder(ms.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.String);
        var result = decoder.ReadString();
        result.Should().Be(data);
    }
    
    [Theory]
    [MemberData(nameof(RandomStrings))]
    public void TestByteArray(string data, int length)
    {
        var bytes = Encoding.UTF8.GetBytes(data);
        using var ms = new MemoryStream();
        var encoder = new Encoder(ms);
        encoder.Write(bytes);
        ms.Position = 0;
        var decoder = new Decoder(ms.GetBuffer());
        decoder.Read();
        decoder.GetTokenType().Should().Be(TokenTypes.ByteArray);
        var outSpan = new byte[decoder.GetLength()];
        decoder.ReadByteArray(outSpan).Should().Be((uint)bytes.Length);
        outSpan.Should().BeEquivalentTo(bytes);
    }

    public static IEnumerable<object[]> RandomStrings()
    {
        var rnd = new Random(0);

        var lengths = new List<int>()
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            10,
            100,
            1000,
            10000,
            100000,
            1000000,
            10000000
        };
        
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        

        List<string> items = new List<string>();
        foreach (var length in lengths)
        {
            var bytes = new char[length];
            for (var idx = 0; idx < bytes.Length; idx ++)
            {
                bytes[idx] = chars[rnd.Next(0, chars.Length)];
            }
            items.Add(new String(bytes));
        }

        return items.Select(x => new object[]
        {
            x, Encoding.UTF8.GetByteCount(x)
        });
    }
}