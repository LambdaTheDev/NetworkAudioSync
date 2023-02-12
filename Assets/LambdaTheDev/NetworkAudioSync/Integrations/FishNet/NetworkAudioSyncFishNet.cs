#if FISHNET

using System;
using FishNet.Object;

namespace LambdaTheDev.NetworkAudioSync.Integrations.FishNet
{
    // Fish Networking integration for NetworkAudioSync
    public sealed class NetworkAudioSyncFishNet : NetworkBehaviour, INetworkAudioSyncIntegration
    {
        bool INetworkAudioSyncIntegration.IsReady => !IsOffline;
        bool INetworkAudioSyncIntegration.IsServer => IsServer;
        float INetworkAudioSyncIntegration.ClientLatency => TimeManager.RoundTripTime / 1000f;

        private Action<ArraySegment<byte>> _callback = NetworkAudioSyncUtils.EmptyCallback;
        
        
        public void BindPacketCallback(Action<ArraySegment<byte>> callback) { _callback = callback; }
        public void ResetPacketCallback() => BindPacketCallback(NetworkAudioSyncUtils.EmptyCallback);
        

        [ObserversRpc]
        public void ServerExecuteAndBroadcastPacket(ArraySegment<byte> packet)
        {
            _callback.Invoke(packet);
        }
    }
}

#endif