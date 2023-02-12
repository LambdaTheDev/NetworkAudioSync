using System;
using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.Integrations;
using LambdaTheDev.NetworkAudioSync.InternalNetworking.Unions;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.InternalNetworking
{
    // Helper struct used to fill out packet contents
    internal sealed class AudioPacketBuilder : IDisposable
    {
        private INetworkAudioSyncIntegration _integration;
        private byte[] _buffer;
        private int _count;
        private bool _isServer;


        public void SetBuffer(byte[] buffer)
        {
            _buffer = buffer;
            _count = 0;
        }

        public void SetIntegration(INetworkAudioSyncIntegration integration)
        {
            _integration = integration;
            _isServer = integration.IsServer;
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

        public AudioPacketBuilder WriteVector3(Vector3 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
            return this;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Send()
        {
            if(!_isServer) ThrowSendingAsNotServer();
            
            var data = new ArraySegment<byte>(_buffer, 0, _count);
            _integration.ServerExecuteAndBroadcastPacket(data);
        }

        public void Dispose()
        {
            NetworkAudioSyncPools.SmallBuffersPool.Return(_buffer);
            NetworkAudioSyncPools.PacketBuildersPool.Return(this);
            _integration = null;
            _buffer = null;
            _count = 0;
        }

        private static void ThrowSendingAsNotServer()
        {
            throw new Exception("Cannot send synchronized audio over network as non-server!");
        }
    }
}