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

        private Action<ArraySegment<byte>> _callback;
        
        
        public void BindPacketCallback(Action<ArraySegment<byte>> callback)
        {
            _callback = callback;
        }

        [ObserversRpc]
        public void SendPacketIfServer(ArraySegment<byte> packet)
        {
            _callback?.Invoke(packet);
        }
    }
}

#endif