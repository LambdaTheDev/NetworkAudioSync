using Mirror;

namespace LambdaTheDev.NetworkAudioSync.PreciseSync
{
    public struct PreciseSyncRequest : NetworkMessage
    {
        public byte Clip;
        public double RequestTime;
    }
}