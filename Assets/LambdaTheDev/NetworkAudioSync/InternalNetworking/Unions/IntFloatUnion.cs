using System.Runtime.InteropServices;

namespace LambdaTheDev.NetworkAudioSync.InternalNetworking.Unions
{
    // C++ union equivalent for converting float to int
    [StructLayout(LayoutKind.Explicit)]
    internal struct IntFloatUnion
    {
        [FieldOffset(0)] public float floatValue;
        [FieldOffset(0)] public int intValue;
    }
}