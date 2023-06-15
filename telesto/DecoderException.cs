using System.Security.Cryptography.X509Certificates;

namespace telesto;

public class DecoderException : Exception
{
    public DecoderException(byte byteCode, Type type, Bytecode? bytecodeEnum)
    : base($"Cannot convert byte code {bytecodeEnum}({byteCode}) to type {type.Name}.")
    {
        
    }

    public static T Throw<T>(byte code)
    {
        var e = code as Bytecode?;
        throw new DecoderException(code, typeof(T), e);
    }
}