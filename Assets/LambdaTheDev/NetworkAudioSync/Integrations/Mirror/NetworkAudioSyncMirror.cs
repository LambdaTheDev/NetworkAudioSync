#if MIRROR

using System;
using Mirror;

namespace LambdaTheDev.NetworkAudioSync.Integrations.Mirror
{
    // Mirror Networking integration for NetworkAudioSync
    public sealed class NetworkAudioSyncMirror : NetworkBehaviour, INetworkAudioSyncIntegration
    {
        public bool IsReady => gameObject.activeInHierarchy;
        public bool IsServer => isServer;
        public float ClientLatency => (float)(NetworkTime.rtt / 2f);

        private Action<ArraySegment<byte>> _callback = NetworkAudioSyncUtils.EmptyCallback;

        
        public void BindPacketCallback(Action<ArraySegment<byte>> callback)
        {
            _callback = callback;
        }
        
        public void ResetPacketCallback() => BindPacketCallback(NetworkAudioSyncUtils.EmptyCallback);

        public void ServerExecuteAndBroadcastPacket(ArraySegment<byte> packet)
        {
            _callback.Invoke(packet);
            ClientRpcBroadcastPacket(packet);
        }

        [ClientRpc]
        private void ClientRpcBroadcastPacket(ArraySegment<byte> packet)
        {
            _callback.Invoke(packet);
        }
    }
}

#endif