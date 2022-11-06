using System;
using System.Collections.Generic;
using LambdaTheDev.NetworkAudioSync.Integrations;

namespace LambdaTheDev.NetworkAudioSync.InternalNetworking
{
    // Pools used within NetworkAudioSync
    internal static class NetworkAudioSyncPools
    {
        private const int SmallBufferLength = 64;
        public static readonly Pool<byte[]> SmallBuffersPool = new Pool<byte[]>(32, 128, () => new byte[SmallBufferLength]);
        public static readonly Pool<AudioPacketBuilder> PacketBuildersPool = new Pool<AudioPacketBuilder>(8, 32, () => new AudioPacketBuilder());
        public static readonly Pool<AudioPacketReader> PacketReaderPool = new Pool<AudioPacketReader>(8, 32, () => new AudioPacketReader());


        // Helper method that rents packet builder & initializes it
        public static AudioPacketBuilder RentBuilder(INetworkAudioSyncIntegration integration, bool throwIfNotServer = true)
        {
            if(throwIfNotServer && !integration.IsServer) ThrowIfNotServer();
            byte[] buffer = SmallBuffersPool.Rent();
            AudioPacketBuilder builder = PacketBuildersPool.Rent();
            builder.SetBuffer(buffer);
            builder.SetIntegration(integration);
            return builder;
        }

        private static void ThrowIfNotServer()
        {
            throw new Exception("Only server can change NetworkAudioSource state!");
        }

        // Simple Pool implementation
        public sealed class Pool<T>
        {
            private readonly Stack<T> _container;
            private readonly Func<T> _generator;
            private readonly int _maxCapacity;


            public Pool(int initialCapacity, int maxCapacity, Func<T> generator)
            {
                _container = new Stack<T>(initialCapacity);
                for (int i = 0; i < initialCapacity; i++)
                {
                    _container.Push(generator.Invoke());
                }

                _maxCapacity = maxCapacity;
                _generator = generator;
            }

            public T Rent()
            {
                if (_container.Count == 0) return _generator.Invoke();
                return _container.Pop();
            }

            public void Return(T item)
            {
                if (_container.Count >= _maxCapacity) return;
                _container.Push(item);
            }
        }
    }
}