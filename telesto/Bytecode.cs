namespace telesto;

public enum Bytecode : byte
{
    Null = 0x00,
    True = 0x01,
    False = 0x02,
    PackedUIntStart = 0x03,
    PackedUIntEnd = 0x07,
    Int2Byte = 0x09,
    Int4Byte = 0x0A,
    Int8Byte = 0x0B,
    UInt1Byte = 0x0C,
    UInt2Byte = 0x0D,
    UInt4Byte = 0x0E,
    UInt8Byte = 0x0F,
    String1ByteSize = 0x10,
    String2ByteSize = 0x11,
    String4ByteSize = 0x12,
    Guid = 0x13,
    Bytes1ByteSize = 0x14,
    Bytes2ByteSize = 0x15,
    Bytes4ByteSize = 0x16,
}