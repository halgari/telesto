namespace telesto;

public enum TokenTypes
{
    Null = 0x00,
    Boolean,
    Int2Byte,
    Int4Byte,
    Int8Byte,
    UInt1Byte,
    UInt2Byte,
    UInt4Byte,
    UInt8Byte,
    String,
    Guid,
    ByteArray,
    StartDictionary,
    StartList,
    StartObject,
}