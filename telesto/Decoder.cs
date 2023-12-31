﻿using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace telesto;

public ref struct Decoder
{
    public readonly ReadOnlySpan<byte> Buffer;
    private byte _nextCode;
    private uint _spanSize;
    private int _offset;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetOffset() => _offset;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetOffset(int offset) => _offset = offset;

    public Decoder(ReadOnlySpan<byte> stream)
    {
        Buffer = stream;
        _offset = 0x00;
        _nextCode = 0x00;
        _spanSize = 0;
    }

    #region Stream-like functions

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte NextByte()
    {
        var value = Buffer[_offset];
        _offset++;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ushort NextUShort()
    {
        var value = BinaryPrimitives.ReadUInt16LittleEndian(Buffer[_offset..]);
        _offset += 2;
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private uint NextUInt()
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(Buffer[_offset..]);
        _offset += 4;
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong NextULong()
    {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(Buffer[_offset..]);
        _offset += 8;
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private short NextShort()
    {
        var value = BinaryPrimitives.ReadInt16LittleEndian(Buffer[_offset..]);
        _offset += 2;
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int NextInt()
    {
        var value = BinaryPrimitives.ReadInt32LittleEndian(Buffer[_offset..]);
        _offset += 4;
        return value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private long NextLong()
    {
        var value = BinaryPrimitives.ReadInt64LittleEndian(Buffer[_offset..]);
        _offset += 8;
        return value;
    }
    
    

    #endregion

    /// <summary>
    /// Must call this before reading a value, and must call it
    /// once at the creation of the decoder.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Read()
    {
        _nextCode = NextByte();
        _spanSize = _nextCode switch
        {
            <= (byte)Bytecode.PackedUIntEnd => 0,
            (byte)Bytecode.Int2Byte => 2,
            (byte)Bytecode.Int4Byte => 4,
            (byte)Bytecode.Int8Byte => 8,
            (byte)Bytecode.UInt1Byte => 1,
            (byte)Bytecode.UInt2Byte => 2,
            (byte)Bytecode.UInt4Byte => 4,
            (byte)Bytecode.UInt8Byte => 8,
            (byte)Bytecode.Guid => 16,
            (byte)Bytecode.String1ByteSize => NextByte(),
            (byte)Bytecode.String2ByteSize => NextUShort(),
            (byte)Bytecode.String4ByteSize => NextUInt(),
            (byte)Bytecode.Bytes1ByteSize => NextByte(),
            (byte)Bytecode.Bytes2ByteSize => NextUShort(),
            (byte)Bytecode.Bytes4ByteSize => NextUInt(),
            (byte)Bytecode.StartList1ByteLength => NextByte(),
            (byte)Bytecode.StartList2ByteLength => NextUShort(),
            (byte)Bytecode.StartList4ByteLength => NextUInt(),
            (byte)Bytecode.StartDictionary1ByteLength => NextByte(),
            (byte)Bytecode.StartDictionary2ByteLength => NextUShort(),
            (byte)Bytecode.StartDictionary4ByteLength => NextUInt(),
            (byte)Bytecode.StartObject1ByteLength => NextByte(),
            (byte)Bytecode.StartObject2ByteLength => NextUShort(),
            (byte)Bytecode.StartObject4ByteLength => NextUInt(),
            _ => throw new NotImplementedException($"Unknown byte code {_nextCode:X}")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            case (byte)Bytecode.String1ByteSize:
            case (byte)Bytecode.String2ByteSize:
            case (byte)Bytecode.String4ByteSize:
                return TokenTypes.String;
            case (byte)Bytecode.Guid:
                return TokenTypes.Guid;
            case (byte)Bytecode.Bytes1ByteSize:
            case (byte)Bytecode.Bytes2ByteSize:
            case (byte)Bytecode.Bytes4ByteSize:
                return TokenTypes.ByteArray;
            case (byte)Bytecode.StartList1ByteLength:
            case (byte)Bytecode.StartList2ByteLength:
            case (byte)Bytecode.StartList4ByteLength:
                return TokenTypes.StartList;
            case (byte)Bytecode.StartDictionary1ByteLength:
            case (byte)Bytecode.StartDictionary2ByteLength:
            case (byte)Bytecode.StartDictionary4ByteLength:
                return TokenTypes.StartDictionary;
            case (byte)Bytecode.StartObject1ByteLength:
            case (byte)Bytecode.StartObject2ByteLength:
            case (byte)Bytecode.StartObject4ByteLength:
                return TokenTypes.StartObject;
        }

        throw new NotImplementedException();
    }
    
    /// <summary>
    /// Returns the current token data as a span, really only useful for
    /// byte arrays and strings.
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ValueSpan()
    {
        return Buffer[_offset..(_offset + (int)_spanSize)];
    }

    /// <summary>
    /// Returns the length of the current token data, really only useful
    /// for variable length data like strings and byte arrays.
    /// </summary>
    /// <returns></returns>
    public uint GetLength()
    {
        return _spanSize;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        return _nextCode switch
        {
            (byte)Bytecode.True => true,
            (byte)Bytecode.False => false,
            _ => DecoderException.Throw<bool>(_nextCode)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadByte()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and 
                <= (byte)Bytecode.PackedUIntEnd => (byte)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => NextByte(),
            _ => DecoderException.Throw<byte>(_nextCode)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUShort()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => (ushort)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.UInt2Byte => NextUShort(),
            _ => DecoderException.Throw<ushort>(_nextCode)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadShort()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => (short)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.Int2Byte => NextShort(),
            (byte)Bytecode.UInt2Byte => (short)NextUShort(),
            _ => DecoderException.Throw<short>(_nextCode)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => (uint)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.UInt2Byte => NextUShort(),
            (byte)Bytecode.UInt4Byte => NextUInt(),
            _ => DecoderException.Throw<uint>(_nextCode)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => (ulong)(_nextCode -
                (byte)Bytecode.PackedUIntStart),
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.UInt2Byte => NextUShort(),
            (byte)Bytecode.UInt4Byte => NextUInt(),
            (byte)Bytecode.UInt8Byte => NextULong(),
            _ => DecoderException.Throw<ulong>(_nextCode)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => _nextCode -
                (byte)Bytecode.PackedUIntStart,
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.UInt2Byte => NextUShort(),
            (byte)Bytecode.UInt4Byte => (int)NextUInt(),
            (byte)Bytecode.Int4Byte => NextInt(),
            _ => DecoderException.Throw<int>(_nextCode)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong()
    {
        return _nextCode switch
        {
            >= (byte)Bytecode.PackedUIntStart and <= (byte)Bytecode.PackedUIntEnd => _nextCode -
                (byte)Bytecode.PackedUIntStart,
            (byte)Bytecode.UInt1Byte => NextByte(),
            (byte)Bytecode.UInt2Byte => NextUShort(),
            (byte)Bytecode.UInt4Byte => NextUInt(),
            (byte)Bytecode.Int4Byte => NextInt(),
            (byte)Bytecode.Int8Byte => NextLong(),
            _ => DecoderException.Throw<int>(_nextCode)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {

        var str = Encoding.UTF8.GetString(Buffer[_offset..(_offset + (int)_spanSize)]);
        _offset += (int)_spanSize;
        return str;
    }
    
    /// <summary>
    /// Reads a guid
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Guid ReadGuid()
    {
        if (_nextCode != (byte)Bytecode.Guid)
            return DecoderException.Throw<Guid>(_nextCode);
        
        var guid = new Guid(Buffer[_offset..(_offset + 16)]);
        _offset += 16;
        return guid;
    }
    
    /// <summary>
    /// Reads the current value as a byte array
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ReadByteArray()
    {
        if (GetTokenType() != TokenTypes.ByteArray)
            return DecoderException.Throw<byte[]>(_nextCode);
        
        var data = ValueSpan().ToArray();
        _offset += (int)_spanSize;
        return data;
    }
    
}