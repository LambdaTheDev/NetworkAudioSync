using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.InternalNetworking;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // NetworkAudioSource synced properties implementation
    // Those properties do exactly the same thing as Unity's AudioSource, but additionally they
    //  send those actions over network.
    public partial class NetworkAudioSource
    {
        public bool BypassEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassEffects;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.BypassEffects)
                    .WriteBool(value).Send();
            }
        }
        
        public bool BypassListenerEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassListenerEffects;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.BypassListenerEffects)
                    .WriteBool(value).Send();
            }
        }

        public bool BypassReverbZones
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassReverbZones;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.BypassReverbZones)
                    .WriteBool(value).Send();
            }
        }

        public float DopplerLevel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.dopplerLevel;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.DopplerLevel)
                    .WriteFloat(value).Send();
            }
        }

#if UNITY_EDITOR || UNITY_PS4 || UNITY_PS5
        public GamepadSpeakerOutputType GamepadSpeakerOutputType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.gamepadSpeakerOutputType;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.GamepadSpeakerOutputType)
                    .WriteByte((byte)value).Send();
            }
        }
#endif

        public bool IgnoreListenerPause
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.ignoreListenerPause;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.IgnoreListenerPause)
                    .WriteBool(value).Send();
            }
        }

        public bool IgnoreListenerVolume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.ignoreListenerVolume;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.IgnoreListenerVolume)
                    .WriteBool(value).Send();
            }
        }

        public bool Loop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.loop;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Loop)
                    .WriteBool(value).Send();
            }
        }

        public float MaxDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.maxDistance;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.MaxDistance)
                    .WriteFloat(value).Send();
            }
        }

        public float MinDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.minDistance;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.MinDistance)
                    .WriteFloat(value).Send();
            }
        }

        public bool Mute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.mute;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Mute)
                    .WriteBool(value).Send();
            }
        }
        
        public float PanStereo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.panStereo;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.PanStereo)
                    .WriteFloat(value).Send();
            }
        }

        public float Pitch
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.pitch;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Pitch)
                    .WriteFloat(value).Send();
            }
        }

        public int Priority
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.priority;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Priority)
                    .WriteInt(value).Send();
            }
        }

        public float ReverbZoneMix
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.reverbZoneMix;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.ReverbZoneMix)
                    .WriteFloat(value).Send();
            }
        }

        public AudioRolloffMode RolloffMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.rolloffMode;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.RolloffMode)
                    .WriteByte((byte)value).Send();
            }
        }

        public float SpatialBlend
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatialBlend;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.SpatialBlend)
                    .WriteFloat(value).Send();
            }
        }

        public bool Spatialize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatialize;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Spatialize)
                    .WriteBool(value).Send();
            }
        }

        public bool SpatializePostEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatializePostEffects;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.SpatializePostEffects)
                    .WriteBool(value).Send();
            }
        }

        public float Spread
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spread;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Spread)
                    .WriteFloat(value).Send();
            }
        }

        public float Time
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.time;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Time)
                    .WriteFloat(value).Send();
            }
        }

        public int TimeSamples
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.timeSamples;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.TimeSamples)
                    .WriteInt(value).Send();
            }
        }

        public AudioVelocityUpdateMode VelocityUpdateMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.velocityUpdateMode;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.VelocityUpdateMode)
                    .WriteByte((byte)value).Send();
            }
        }

        public float Volume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.volume;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Volume)
                    .WriteFloat(value).Send();
            }
        }

        public bool Paused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetState().IsPaused;
            set
            {
                using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
                builder.WriteByte(AudioSourceActionId.Pause)
                    .WriteBool(value).Send();
            }
        }
    }
}