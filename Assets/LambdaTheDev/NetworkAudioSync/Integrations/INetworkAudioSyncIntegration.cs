using System;

namespace LambdaTheDev.NetworkAudioSync.Integrations
{
    // Interface for NetworkAudioSync networking integrations
    public interface INetworkAudioSyncIntegration
    {
        // True, if networking is ready to be used
        bool IsReady { get; }
        
        // True, if this peer has server permissions
        bool IsServer { get; }
        
        // Returns latency for local client
        float ClientLatency { get; }
        
        
        // Binds packet callback to this integration
        void BindPacketCallback(Action<ArraySegment<byte>> callback);

        // Resets the packet callback to this integration
        void ResetPacketCallback();
        
        // Server method used to execute the packet, and then broadcasts it to other clients
        void ServerExecuteAndBroadcastPacket(ArraySegment<byte> packet);
    }
}