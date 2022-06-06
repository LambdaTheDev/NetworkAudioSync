using System;
using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.InternalNetworking.Unions;

namespace LambdaTheDev.NetworkAudioSync.InternalNetworking
{
    // Helper struct used to read packet content
    internal sealed class AudioPacketReader : IDisposable
    {
        private ArraySegment<byte> _buffer;
        private int _offset;


        public void SetBuffer(ArraySegment<byte> buffer)
        {
            _buffer = buffer;
            _offset = buffer.Offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte()
        {
            return _buffer.Array![_offset++];
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBool()
        {
            return _buffer.Array![_offset++] == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort()
        {
#pragma warning disable CS0675
            short value = 0;
            value |= _buffer.Array![_offset++];
            value |= (short)(_buffer.Array![_offset++] << 8);
            return value;
#pragma warning restore CS0675
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt()
        {
#pragma warning disable CS0675
            int value = 0;
            value |= _buffer.Array![_offset++];
            value |= (_buffer.Array![_offset++] << 8);
            value |= (_buffer.Array![_offset++] << 16);
            value |= (_buffer.Array![_offset++] << 24);
            return value;
#pragma warning restore CS0675
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadFloat()
        {
            int intValue = ReadInt();
            IntFloatUnion union = new IntFloatUnion { intValue = intValue };
            return union.floatValue;
        }

        public void Dispose()
        {
            NetworkAudioSyncPools.PacketReaderPool.Return(this);
            _offset = 0;
            _buffer = default;
        }
    }
}