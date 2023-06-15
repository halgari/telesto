using System.Buffers.Binary;

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
    /// Writes an int value to the stream.
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
}