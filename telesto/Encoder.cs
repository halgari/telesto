﻿using System.Buffers.Binary;

namespace telesto;

public struct Encoder
{
    /// <summary>
    /// The number of numbers that can be packed into a single byte.
    /// </summary>
    public const int PackedIntSize = Bytecodes.PackedUIntEnd - Bytecodes.PackedUIntStart;
    
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
        _stream.WriteByte((byte)Bytecodes.Null);
    }
    
    /// <summary>
    /// Writes a boolean value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(bool value)
    {
        _stream.WriteByte(value ? (byte)Bytecodes.True : (byte)Bytecodes.False);
    }

    /// <summary>
    /// Writes a byte value to the stream.
    /// </summary>
    /// <param name="value"></param>
    public void Write(byte value)
    {
        if (value < PackedIntSize)
        {
            _stream.WriteByte((byte)(Bytecodes.PackedUIntStart + value));
        }
        else
        {
            _stream.WriteByte((byte)Bytecodes.UInt1Byte);
            _stream.WriteByte(value);
        }
    }

    public void Write(ushort value)
    {
        if (value <= byte.MaxValue)
        {
            Write((byte)value);
            return;
        }
        _stream.WriteByte((byte)Bytecodes.UInt2Byte);
        Span<byte> buffer = stackalloc byte[2];
        BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    public void Write(uint value)
    {
        if (value <= ushort.MaxValue)
        {
            Write((ushort)value);
            return;
        }
        _stream.WriteByte((byte)Bytecodes.UInt4Byte);
        Span<byte> buffer = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        _stream.Write(buffer);
    }

    public void Write(ulong value)
    {
        if (value <= uint.MaxValue)
        {
            Write((uint)value);
            return;
        }
        _stream.WriteByte((byte)Bytecodes.UInt8Byte);
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        _stream.Write(buffer);
    }
    
}