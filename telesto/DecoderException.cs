using System.Security.Cryptography.X509Certificates;

namespace telesto;

public class DecoderException : Exception
{
    public DecoderException(byte byteCode, Type type)
    : base($"Cannot convert byte code {byteCode} to type {type.Name}.")
    {
        
    }

    public static T Throw<T>(byte code)
    {
        throw new DecoderException(code, typeof(T));
    }
}