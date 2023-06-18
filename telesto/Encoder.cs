using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace telesto;

public struct Encoder
{
    /// <summary>
    /// The number of numbers that can be packed into a single byte.
    /// </summary>
    public const int PackedIntSize = Bytecode.PackedUIntEnd - Bytecode.PackedUIntStart;
    
    private readonly Stream _stream;

    public Encoder(Stream stream)
    {
        _stream = stream;
    }

    #region Stream-like functions

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteNative(Bytecode code)
    {
        WriteNative((byte)code);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteNative(byte value)
    {
        _stream.WriteByte(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteNative(ushort value)
    {
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        _stream.Write(buffer);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteNative(uint value)
    {
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    #endregion
    
    /// <summary>
    /// Writes a null value to the stream.
    /// </summary>
    public void WriteNull()
    {
        _stream.WriteByte((byte)Bytecode.Null);
    }
    
    /// <summary>
    /// Writes a boolean value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(bool value)
    {
        _stream.WriteByte(value ? (byte)Bytecode.True : (byte)Bytecode.False);
    }

    /// <summary>
    /// Writes a byte value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(byte value)
    {
        if (value < PackedIntSize)
        {
            _stream.WriteByte((byte)(Bytecode.PackedUIntStart + value));
        }
        else
        {
            _stream.WriteByte((byte)Bytecode.UInt1Byte);
            _stream.WriteByte(value);
        }
    }

    /// <summary>
    /// Writes a short value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(ushort value)
    {
        if (value <= byte.MaxValue)
        {
            Write((byte)value);
            return;
        }
        _stream.WriteByte((byte)Bytecode.UInt2Byte);
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        _stream.Write(buffer);
    }
    
    /// <summary>
    /// Writes a long value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(uint value)
    {
        if (value <= ushort.MaxValue)
        {
            Write((ushort)value);
            return;
        }
        _stream.WriteByte((byte)Bytecode.UInt4Byte);
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    /// <summary>
    /// Writes an int value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(int value)
    {
        if (value is > 0 and <= short.MaxValue)
        {
            Write((ushort)value);
            return;
        }
        _stream.WriteByte((byte)Bytecode.Int4Byte);
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    /// <summary>
    /// Writes a long value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(ulong value)
    {
        if (value <= uint.MaxValue)
        {
            Write((uint)value);
            return;
        }
        _stream.WriteByte((byte)Bytecode.UInt8Byte);
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        _stream.Write(buffer);
    }
    
    /// <summary>
    /// Writes a long value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(long value)
    {
        if (value is >= 0 and <= int.MaxValue)
        {
            Write((uint)value);
            return;
        }
        _stream.WriteByte((byte)Bytecode.Int8Byte);
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    public void Write(short value)
    {
        switch (value)
        {
            case >= 0 and <= byte.MaxValue:
                Write((byte)value);
                return;
            case > 0:
                Write((ushort)value);
                return;
            default:
                _stream.WriteByte((byte)Bytecode.Int2Byte);
                Span<byte> buffer = stackalloc byte[2];
                BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
                _stream.Write(buffer);
                return;
        }
    }
    
    public void Write(string value)
    {
        Span<byte> buffer = stackalloc byte[128];
        var encodedLength = Encoding.UTF8.GetByteCount(value);
        
        switch (encodedLength)
        {
            case <= byte.MaxValue:
                _stream.WriteByte((byte)Bytecode.String1ByteSize);
                _stream.WriteByte((byte)encodedLength);
                break;
            case <= ushort.MaxValue:
                _stream.WriteByte((byte)Bytecode.String2ByteSize);
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)encodedLength);
                _stream.Write(buffer[..2]);
                break;
            default:
                _stream.WriteByte((byte)Bytecode.String4ByteSize);
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)encodedLength);
                _stream.Write(buffer[..4]);
                break;
        }
        
        var bytes = Encoding.UTF8.GetBytes(value);
        Debug.Assert(bytes.Length == encodedLength);
        _stream.Write(bytes);
    }

    /// <summary>
    /// Writes a guid value to the stream.
    /// </summary>
    /// <param name="guid"></param>
    public void Write(Guid guid)
    {
        _stream.WriteByte((byte)Bytecode.Guid);
        Span<byte> buffer = stackalloc byte[16];
        guid.TryWriteBytes(buffer);
        _stream.Write(buffer);
    }
    
    /// <summary>
    /// Writes a span as a byte array to the stream.
    /// </summary>
    /// <param name="bytes"></param>
    public void Write(ReadOnlySpan<byte> bytes)
    {
        var encodedLength = bytes.Length;
        
        switch (encodedLength)
        {
            case <= byte.MaxValue:
                WriteNative(Bytecode.Bytes1ByteSize);
                WriteNative((byte)encodedLength);
                break;
            case <= ushort.MaxValue:
                WriteNative(Bytecode.Bytes2ByteSize);
                WriteNative((ushort)encodedLength);
                break;
            default:
                WriteNative(Bytecode.Bytes4ByteSize);
                WriteNative((uint)encodedLength);
                break;
        }
        
        _stream.Write(bytes);
    }

    /// <summary>
    /// Writes a start list marker, should be matched by a call to <see cref="WriteEndContainer"/>.
    /// </summary>
    public void WriteStartList(int length)
    {
        switch (length)
        {
            case <= byte.MaxValue:
                WriteNative(Bytecode.StartList1ByteLength);
                WriteNative((byte)length);
                break;
            case <= ushort.MaxValue:
                WriteNative(Bytecode.StartList2ByteLength);
                WriteNative((ushort)length);
                break;
            default:
                WriteNative(Bytecode.StartList4ByteLength);
                WriteNative((uint)length);
                break;
        }
    }

    
    /// <summary>
    /// Writes a start dictionary marker, should be matched by a call to <see cref="WriteEndContainer"/>.
    /// Length is the number of key-value pairs in the dictionary.
    /// </summary>
    /// <param name="pairCount"></param>
    public void WriteStartDictionary(int pairCount)
    {
        switch (pairCount)
        {
            case <= byte.MaxValue:
                WriteNative(Bytecode.StartDictionary1ByteLength);
                WriteNative((byte)pairCount);
                break;
            case <= ushort.MaxValue:
                WriteNative(Bytecode.StartDictionary2ByteLength);
                WriteNative((ushort)pairCount);
                break;
            default:
                WriteNative(Bytecode.StartDictionary4ByteLength);
                WriteNative((uint)pairCount);
                break;
        }
    }
    
    public void WriteStartObject(ReadOnlySpan<byte> type, int memberCount)
    {
        switch (memberCount)
        {
            case <= byte.MaxValue:
                WriteNative(Bytecode.StartObject1ByteLength);
                WriteNative((byte)memberCount);
                break;
            case <= ushort.MaxValue:
                WriteNative(Bytecode.StartObject2ByteLength);
                WriteNative((ushort)memberCount);
                break;
            default:
                WriteNative(Bytecode.StartObject4ByteLength);
                WriteNative((uint)memberCount);
                break;
        }
        Write(type);
    }

}