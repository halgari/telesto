namespace telesto;

public enum Bytecodes : byte
{
    Null = 0x00,
    True = 0x01,
    False = 0x02,
    PackedUIntStart = 0x03,
    PackedUIntEnd = 0x07,
    Int1Byte = 0x08,
    Int2Byte = 0x09,
    Int4Byte = 0x0A,
    Int8Byte = 0x0B,
    UInt1Byte = 0x0C,
    UInt2Byte = 0x0D,
    UInt4Byte = 0x0E,
    UInt8Byte = 0x0F,
    PackedArrayStart = 0x0C,
    PackedArrayEnd = 0x0F,
}