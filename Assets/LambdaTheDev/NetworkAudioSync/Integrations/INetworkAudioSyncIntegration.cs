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
        
        // Send packet only if ready & server (if client calls this method, exception can be thrown)
        void SendPacketIfServer(ArraySegment<byte> packet);
    }
}