## Intro
Telesto is a bytecode based serialization format for .NET
that aims to be allocation free and fast. Based on the internal
structure of the .NET JSON serializers, in that it uses refs,
structs and streaming readers for the smallest amount of allocations.


## Format

| Code        | Description       |
|-------------|-------------------|
| 0x00        | Null              |
| 0x01        | True              |
| 0x02        | False             |
| 0x03 - 0x07 | Packed UInt       |
| 0x08        | 1 byte Int        |
| 0x09        | 2 byte Int        |
| 0x0A        | 4 byte Int        |
| 0x0B        | 8 byte Int        |
| 0x0C        | 1 byte UInt       |
| 0x0D        | 2 byte UInt       |
| 0x0E        | 4 byte UInt       |
| 0x0F        | 8 byte UInt       |
| 0x10 - 0x20 | Packed byte Array |

