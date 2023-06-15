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
        Bytecode? result = (Bytecode)code;
        if (Enum.IsDefined((Bytecode)code))
            throw new DecoderException(code, typeof(T), result);
        else
            throw new DecoderException(code, typeof(T), null);
    }
}