using System.Buffers.Binary;

namespace telesto;

public struct Decoder
{
    private readonly Stream _stream;
    private byte _nextCode;

    public Decoder(Stream stream)
    {
        _stream = stream;
        _nextCode = 0x00;
    }

    /// <summary>
    /// Must call this before reading a value, and must call it
    /// once at the creation of the decoder.
    /// </summary>
    public void Read()
    {
        _nextCode = (byte)_stream.ReadByte();
    }

    public TokenTypes GetTokenType()
    {
        switch (_nextCode)
        {
            case (byte)Bytecodes.Null:
                return TokenTypes.Null;
            case (byte)Bytecodes.True:
            case (byte)Bytecodes.False:
                return TokenTypes.Boolean;
            case >= (byte)Bytecodes.PackedUIntStart 
                and <= (byte)Bytecodes.PackedUIntEnd:
                return TokenTypes.UInt1Byte;
            case (byte)Bytecodes.Int1Byte:
                return TokenTypes.Int1Byte;
            case (byte)Bytecodes.Int2Byte:
                return TokenTypes.Int2Byte;
            case (byte)Bytecodes.Int4Byte:
                return TokenTypes.Int4Byte;
            case (byte)Bytecodes.Int8Byte:
                return TokenTypes.Int8Byte;
            case (byte)Bytecodes.UInt1Byte:
                return TokenTypes.UInt1Byte;
            case (byte)Bytecodes.UInt2Byte:
                return TokenTypes.UInt2Byte;
            case (byte)Bytecodes.UInt4Byte:
                return TokenTypes.UInt4Byte;
            case (byte)Bytecodes.UInt8Byte:
                return TokenTypes.UInt8Byte;
        }

        throw new NotImplementedException();
    }

    public bool ReadBoolean()
    {
        return _nextCode switch
        {
            (byte)Bytecodes.True => true,
            (byte)Bytecodes.False => false,
            _ => DecoderException.Throw<bool>(_nextCode)
        };
    }

    public byte ReadByte()
    {
        return _nextCode switch
        {
            >= (byte)Bytecodes.PackedUIntStart and 
                <= (byte)Bytecodes.PackedUIntEnd => (byte)(_nextCode -
                (byte)Bytecodes.PackedUIntStart),
            (byte)Bytecodes.UInt1Byte => (byte)_stream.ReadByte(),
            _ => DecoderException.Throw<byte>(_nextCode)
        };
    }

    public ushort ReadUShort()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecodes.PackedUIntStart and <= (byte)Bytecodes.PackedUIntEnd:
                return (ushort)(_nextCode - (byte)Bytecodes.PackedUIntStart);
            case (byte)Bytecodes.UInt1Byte:
                return (ushort)_stream.ReadByte();
            case (byte)Bytecodes.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<ushort>(_nextCode);
        }
    }

    public uint ReadUInt()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecodes.PackedUIntStart and <= (byte)Bytecodes.PackedUIntEnd:
                return (uint)(_nextCode - (byte)Bytecodes.PackedUIntStart);
            case (byte)Bytecodes.UInt1Byte:
                return (uint)_stream.ReadByte();
            case (byte)Bytecodes.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            case (byte)Bytecodes.UInt4Byte:
            {
                Span<byte> buf = stackalloc byte[4];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt32LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<uint>(_nextCode);
        }
    }
}