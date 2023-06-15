using System.Buffers.Binary;

namespace telesto;

public struct Decoder
{
    private readonly Stream _stream;
    private byte _nextCode;
    private int _spanSize;

    public Decoder(Stream stream)
    {
        _stream = stream;
        _nextCode = 0x00;
        _spanSize = 0;
    }

    /// <summary>
    /// Must call this before reading a value, and must call it
    /// once at the creation of the decoder.
    /// </summary>
    public void Read()
    {
        _nextCode = (byte)_stream.ReadByte();
        _spanSize = _nextCode switch
        {
            <= (byte)Bytecode.PackedUIntEnd => 0,
            (byte)Bytecode.Int1Byte => 1,
            (byte)Bytecode.Int2Byte => 2,
            (byte)Bytecode.Int4Byte => 4,
            (byte)Bytecode.Int8Byte => 8,
            (byte)Bytecode.UInt1Byte => 1,
            (byte)Bytecode.UInt2Byte => 2,
            (byte)Bytecode.UInt4Byte => 4,
            (byte)Bytecode.UInt8Byte => 8,
            _ => _spanSize
        };
    }

    public TokenTypes GetTokenType()
    {
        switch (_nextCode)
        {
            case (byte)Bytecode.Null:
                return TokenTypes.Null;
            case (byte)Bytecode.True:
            case (byte)Bytecode.False:
                return TokenTypes.Boolean;
            case >= (byte)Bytecode.PackedUIntStart 
                and <= (byte)Bytecode.PackedUIntEnd:
                return TokenTypes.UInt1Byte;
            case (byte)Bytecode.Int2Byte:
                return TokenTypes.Int2Byte;
            case (byte)Bytecode.Int4Byte:
                return TokenTypes.Int4Byte;
            case (byte)Bytecode.Int8Byte:
                return TokenTypes.Int8Byte;
            case (byte)Bytecode.UInt1Byte:
                return TokenTypes.UInt1Byte;
            case (byte)Bytecode.UInt2Byte:
                return TokenTypes.UInt2Byte;
            case (byte)Bytecode.UInt4Byte:
                return TokenTypes.UInt4Byte;
            case (byte)Bytecode.UInt8Byte:
                return TokenTypes.UInt8Byte;
        }

        throw new NotImplementedException();
    }

    public bool ReadBoolean()
    {
        return _nextCode switch
        {
            (byte)Bytecode.True => true,
            (byte)Bytecode.False => false,
            _ => DecoderException.Throw<bool>(_nextCode)
        };
    }

    public byte ReadByte()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and 
                <= (byte)Bytecode.PackedUIntEnd => (byte)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => (byte)_stream.ReadByte(),
            _ => DecoderException.Throw<byte>(_nextCode)
        };
    }

    public ushort ReadUShort()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd:
                return (ushort)(_nextCode - (byte)Bytecode.PackedUIntStart);
            case (byte)Bytecode.UInt1Byte:
                return (ushort)_stream.ReadByte();
            case (byte)Bytecode.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<ushort>(_nextCode);
        }
    }

    public short ReadShort()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd:
                return (short)(_nextCode - (byte)Bytecode.PackedUIntStart);
            case (byte)Bytecode.UInt1Byte:
                return (short)_stream.ReadByte();
            case (byte)Bytecode.Int2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadInt16LittleEndian(buf);
            }
            case (byte)Bytecode.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return (short)BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }                
            default:
                return DecoderException.Throw<short>(_nextCode);
        }
    }

    public uint ReadUInt()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd:
                return (uint)(_nextCode - (byte)Bytecode.PackedUIntStart);
            case (byte)Bytecode.UInt1Byte:
                return (uint)_stream.ReadByte();
            case (byte)Bytecode.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            case (byte)Bytecode.UInt4Byte:
            {
                Span<byte> buf = stackalloc byte[4];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt32LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<uint>(_nextCode);
        }
    }
    
    public ulong ReadULong()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd:
                return (ulong)(_nextCode - (byte)Bytecode.PackedUIntStart);
            case (byte)Bytecode.UInt1Byte:
                return (ulong)_stream.ReadByte();
            case (byte)Bytecode.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            case (byte)Bytecode.UInt4Byte:
            {
                Span<byte> buf = stackalloc byte[4];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt32LittleEndian(buf);
            }            
            case (byte)Bytecode.UInt8Byte:
            {
                Span<byte> buf = stackalloc byte[8];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt64LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<ulong>(_nextCode);
        }
    }
    
    public int ReadInt()
    {
        switch (_nextCode)
        {
            case >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd:
                return _nextCode - (byte)Bytecode.PackedUIntStart;
            case (byte)Bytecode.UInt1Byte:
                return _stream.ReadByte();
            case (byte)Bytecode.UInt2Byte:
            {
                Span<byte> buf = stackalloc byte[2];
                _stream.Read(buf);
                return BinaryPrimitives.ReadUInt16LittleEndian(buf);
            }
            case (byte)Bytecode.UInt4Byte:
            {
                Span<byte> buf = stackalloc byte[4];
                _stream.Read(buf);
                return (int)BinaryPrimitives.ReadUInt32LittleEndian(buf);
            }
            case (byte)Bytecode.Int4Byte:
            {
                Span<byte> buf = stackalloc byte[4];
                _stream.Read(buf);
                return BinaryPrimitives.ReadInt32LittleEndian(buf);
            }
            default:
                return DecoderException.Throw<int>(_nextCode);
        }
    }
}