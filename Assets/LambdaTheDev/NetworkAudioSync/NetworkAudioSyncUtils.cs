using System;

namespace LambdaTheDev.NetworkAudioSync
{
    // Utilities for NetworkAudioSync
    public static class NetworkAudioSyncUtils
    {
        // Code from: https://andrewlock.net/why-is-string-gethashcode-different-each-time-i-run-my-program-in-net-core/
        // Modified a small thing to improve performance
        public static int GetPlatformStableHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;
                
                int length = str.Length;
                int endingCondition = length - 1;

                for (int i = 0; i < length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == endingCondition)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        // Same as above, but wrapped to short
        public static short GetPlatformStableHashCodeShort(string str)
        {
            int intHashCode = GetPlatformStableHashCode(str);
            return (short)(intHashCode & 0xFFFF);
        }
        
        // Just a default callback method to avoid null check in packet processor
        private static void EmptyCallbackMethod(ArraySegment<byte> _) { }
        
        public static readonly Action<ArraySegment<byte>> EmptyCallback = EmptyCallbackMethod;
    }
}