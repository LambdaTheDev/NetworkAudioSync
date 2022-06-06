using System;
using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.Integrations;
using LambdaTheDev.NetworkAudioSync.InternalNetworking.Unions;

namespace LambdaTheDev.NetworkAudioSync.InternalNetworking
{
    // Helper struct used to fill out packet contents
    internal sealed class AudioPacketBuilder : IDisposable
    {
        private INetworkAudioSyncIntegration _integration;
        private byte[] _buffer;
        private int _count;


        public void SetBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _count = 0;
        }

        public void SetIntegration(INetworkAudioSyncIntegration integration)
        {
            _integration = integration;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioPacketBuilder WriteByte(byte value)
        {
            _buffer[_count++] = value;
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioPacketBuilder WriteBool(bool value)
        {
            _buffer[_count++] = (byte)(value ? 1 : 0);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioPacketBuilder WriteShort(short value)
        {
            _buffer[_count++] = (byte)(value);
            _buffer[_count++] = (byte)(value >> 8);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioPacketBuilder WriteInt(int value)
        {
            _buffer[_count++] = (byte)(value);
            _buffer[_count++] = (byte)(value >> 8);
            _buffer[_count++] = (byte)(value >> 16);
            _buffer[_count++] = (byte)(value >> 24);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public AudioPacketBuilder WriteFloat(float value)
        {
            IntFloatUnion union = new IntFloatUnion { floatValue = value };
            WriteInt(union.intValue);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send()
        {
            var data = new ArraySegment<byte>(_buffer, 0, _count);
            _integration.SendPacketIfServer(data);
        }

        public void Dispose()
        {
            NetworkAudioSyncPools.SmallBuffersPool.Return(_buffer);
            NetworkAudioSyncPools.PacketBuildersPool.Return(this);
            _integration = null;
            _buffer = null;
            _count = 0;
        }
    }
}