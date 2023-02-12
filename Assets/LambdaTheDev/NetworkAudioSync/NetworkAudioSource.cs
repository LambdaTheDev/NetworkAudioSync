using System.Runtime.CompilerServices;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Allows AudioSource settings synchronization over network
    // Base implementation with definitions & essentials
    public partial class NetworkAudioSource : BaseNetworkAudioSyncComponent
    {
        // Audio clips used by this NAS instance
        [SerializeField] private NetworkAudioClips clips;
        

        protected override void VirtualAwake()
        {
            if (clips == null)
                Debug.LogWarning("NetworkAudioClips instance has not been assigned for this NetworkAudioSource. This is not an error, but you will be unable to play other AudioClips than default one!");
            else
                clips.Initialize();

            // Initialize integration
            Integration.BindPacketCallback(OnNetworkPacket);
        }

        protected override void VirtualOnDestroy() { }
<<<<<<< Updated upstream

        #region Internal networking processor

        private void OnNetworkPacket(ArraySegment<byte> packet)
        {
            // Server has already applied settings. This shouldn't happen tbh
            if (Integration.IsServer) return;

            using AudioPacketReader reader = NetworkAudioSyncPools.PacketReaderPool.Rent();
            reader.SetBuffer(packet);
            
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                #region Methods impl
                    
                case AudioSourceActionId.Play:
                    byte playMode = reader.ReadByte();
                    switch (playMode)
                    {
                        case AudioSourceActionId.PlayModes.Normal:
                            AudioSource.Play();
                            break;
                            
                        case AudioSourceActionId.PlayModes.Delayed:
                            float volume = reader.ReadFloat();
                            if (volume > 0)
                            {
                                volume = Mathf.Clamp(volume, 0f, 1f);
                                AudioSource.volume = volume;
                            }
                            
                            float delay = reader.ReadFloat();
                            bool compensate = reader.ReadBool();

                            if (compensate)
                                delay = Mathf.Max(delay - Integration.ClientLatency, 0);

                            AudioSource.PlayDelayed(delay);
                            break;
                            
                        case AudioSourceActionId.PlayModes.OneShot:
                            int oneShotClipHash = reader.ReadInt();
                            float volumeScale = reader.ReadFloat();
                            EnsureNetworkAudioClipsIsSet();
                            AudioClip oneShotClip = clips.GetAudioClip(oneShotClipHash);
                            EnsureClipIsNotNull(oneShotClip);

                            AudioSource.volume = volumeScale;
                            AudioSource.PlayOneShot(oneShotClip);
                            break;
                            
                        default:
                            AudioSourceActionId.InvalidPacketThrow();
                            break;
                    }

                    break;
                    
                case AudioSourceActionId.Stop:
                    AudioSource.Stop();
                    break;
                    
                case AudioSourceActionId.SetClip:
                    int setClipHash = reader.ReadInt();
                    EnsureNetworkAudioClipsIsSet();
                    AudioClip setClip = clips.GetAudioClip(setClipHash);
                    EnsureClipIsNotNull(setClip);
                    AudioSource.clip = setClip;
                    break;
                    
                #endregion


                #region Properties impl

                case AudioSourceActionId.BypassEffects:
                    AudioSource.bypassEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.BypassListenerEffects:
                    AudioSource.bypassListenerEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.BypassReverbZones:
                    AudioSource.bypassReverbZones = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.DopplerLevel:
                    AudioSource.dopplerLevel = reader.ReadFloat();
                    break;

#if UNITY_EDITOR || UNITY_PS4 || UNITY_PS5
                case AudioSourceActionId.GamepadSpeakerOutputType:
                    AudioSource.gamepadSpeakerOutputType = (GamepadSpeakerOutputType)reader.ReadByte();
                    break;
#endif
                    
                case AudioSourceActionId.IgnoreListenerPause:
                    AudioSource.ignoreListenerPause = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.IgnoreListenerVolume:
                    AudioSource.ignoreListenerVolume = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.Loop:
                    AudioSource.loop = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.MaxDistance:
                    AudioSource.maxDistance = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.MinDistance:
                    AudioSource.minDistance = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Mute:
                    AudioSource.mute = reader.ReadBool();
                    break;
                    
                // TODO: Create OutputAudioMixerGroup support
                    
                case AudioSourceActionId.PanStereo:
                    AudioSource.panStereo = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Pitch:
                    AudioSource.pitch = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Priority:
                    AudioSource.priority = reader.ReadInt();
                    break;
                    
                case AudioSourceActionId.ReverbZoneMix:
                    AudioSource.reverbZoneMix = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.RolloffMode:
                    AudioSource.rolloffMode = (AudioRolloffMode)reader.ReadByte();
                    break;
                    
                case AudioSourceActionId.SpatialBlend:
                    AudioSource.spatialBlend = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Spatialize:
                    AudioSource.spatialize = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.SpatializePostEffects:
                    AudioSource.spatializePostEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.Spread:
                    AudioSource.spread = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Time:
                    AudioSource.time = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.TimeSamples:
                    AudioSource.timeSamples = reader.ReadInt();
                    break;
                    
                case AudioSourceActionId.VelocityUpdateMode:
                    AudioSource.velocityUpdateMode = (AudioVelocityUpdateMode)reader.ReadByte();
                    break;
                    
                case AudioSourceActionId.Volume:
                    AudioSource.volume = reader.ReadFloat();
                    break;

                #endregion
                    
                default:
                    AudioSourceActionId.InvalidPacketThrow();
                    break;
            }
        }

        #endregion

        
        #region Unity AudioSource wrapper - properties
        
        // METHODS IN HERE ARE DOING EXACTLY THE SAME THING AS UNITY AUDIO CLIP METHODS,
        //  BUT SET METHODS/PROPERTIES (AudioSource state changing methods) ARE SERVER-AUTHORITATIVE,
        //  AND TRIGGER NETWORK PACKET SEND!
        
        public bool BypassEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassEffects)
                        .WriteBool(value).Send();
                }
                AudioSource.bypassEffects = value;
            }
        }
        
        public bool BypassListenerEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassListenerEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassListenerEffects)
                        .WriteBool(value).Send();
                }
                AudioSource.bypassListenerEffects = value;
            }
        }

        public bool BypassReverbZones
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.bypassReverbZones;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassReverbZones)
                        .WriteBool(value).Send();
                }
                AudioSource.bypassReverbZones = value;
            }
        }

        public float DopplerLevel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.dopplerLevel;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.DopplerLevel)
                        .WriteFloat(value).Send();
                }
                AudioSource.dopplerLevel = value;
            }
        }

#if UNITY_EDITOR || UNITY_PS4 || UNITY_PS5
        public GamepadSpeakerOutputType GamepadSpeakerOutputType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.gamepadSpeakerOutputType;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.GamepadSpeakerOutputType)
                        .WriteByte((byte)value).Send();
                }
                AudioSource.gamepadSpeakerOutputType = value;
            }
        }
#endif

        public bool IgnoreListenerPause
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.ignoreListenerPause;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.IgnoreListenerPause)
                        .WriteBool(value).Send();
                }
                AudioSource.ignoreListenerPause = value;
            }
        }

        public bool IgnoreListenerVolume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.ignoreListenerVolume;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.IgnoreListenerVolume)
                        .WriteBool(value).Send();
                }
                AudioSource.ignoreListenerVolume = value;
            }
        }

        public bool Loop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.loop;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Loop)
                        .WriteBool(value).Send();
                }
                AudioSource.loop = value;
            }
        }

        public float MaxDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.maxDistance;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.MaxDistance)
                        .WriteFloat(value).Send();
                }
                AudioSource.maxDistance = value;
            }
        }

        public float MinDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.minDistance;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.MinDistance)
                        .WriteFloat(value).Send();
                }
                AudioSource.minDistance = value;
            }
        }

        public bool Mute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.mute;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Mute)
                        .WriteBool(value).Send();
                }
                AudioSource.mute = value;
            }
        }

        public AudioMixerGroup OutputAudioMixerGroup
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.outputAudioMixerGroup;
            set
            {
                throw new NotImplementedException("Right now I have no idea how to synchronize it over network, but I will find out!");
            }
        }

        public float PanStereo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.panStereo;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.PanStereo)
                        .WriteFloat(value).Send();
                }
                AudioSource.panStereo = value;
            }
        }

        public float Pitch
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.pitch;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Pitch)
                        .WriteFloat(value).Send();
                }
                AudioSource.pitch = value;
            }
        }

        public int Priority
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.priority;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Priority)
                        .WriteInt(value).Send();
                }
                AudioSource.priority = value;
            }
        }

        public float ReverbZoneMix
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.reverbZoneMix;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.ReverbZoneMix)
                        .WriteFloat(value).Send();
                }
                AudioSource.reverbZoneMix = value;
            }
        }

        public AudioRolloffMode RolloffMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.rolloffMode;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.RolloffMode)
                        .WriteByte((byte)value).Send();
                }
                AudioSource.rolloffMode = value;
            }
        }

        public float SpatialBlend
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatialBlend;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.SpatialBlend)
                        .WriteFloat(value).Send();
                }
                AudioSource.spatialBlend = value;
            }
        }

        public bool Spatialize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatialize;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Spatialize)
                        .WriteBool(value).Send();
                }
                AudioSource.spatialize = value;
            }
        }

        public bool SpatializePostEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spatializePostEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.SpatializePostEffects)
                        .WriteBool(value).Send();
                }
                AudioSource.spatializePostEffects = value;
            }
        }

        public float Spread
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.spread;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Spread)
                        .WriteFloat(value).Send();
                }
                AudioSource.spread = value;
            }
        }

        public float Time
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.time;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Time)
                        .WriteFloat(value).Send();
                }
                AudioSource.time = value;
            }
        }

        public int TimeSamples
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.timeSamples;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.TimeSamples)
                        .WriteInt(value).Send();
                }
                AudioSource.timeSamples = value;
            }
        }

        public AudioVelocityUpdateMode VelocityUpdateMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.velocityUpdateMode;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.VelocityUpdateMode)
                        .WriteByte((byte)value).Send();
                }
                AudioSource.velocityUpdateMode = value;
            }
        }

        public float Volume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AudioSource.volume;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Volume)
                        .WriteFloat(value).Send();
                }
                AudioSource.volume = value;
            }
        }

        public bool Paused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetState().IsPaused;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
                {
                    builder.WriteByte(AudioSourceActionId.Pause)
                        .WriteBool(value).Send();
                }
                if(value) AudioSource.Pause();
                else AudioSource.UnPause();
                
                
            }
        }
        
        #endregion


        #region Unity AudioSource wrapper - methods

        [Obsolete("Instead of Pause() and UnPause() method, use Paused property!")]
        public void Pause() { Paused = true; }
        [Obsolete("Instead of Pause() and UnPause() method, use Paused property!")]
        public void UnPause() { Paused = false; }

        public void Stop()
        {
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
            {
                builder.WriteByte(AudioSourceActionId.Stop).Send();
            }
            AudioSource.Stop();
        }

        public void Play()
        {
            EnsureClipIsSet();
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.Normal)
                    .Send();
            }

            AudioSource.Play();
        }

        public void PlayDelayed(float delayInSeconds, float volume = -1f, bool compensateLatency = true)
        {
            EnsureClipIsSet();
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.Delayed)
                    .WriteFloat(volume)
                    .WriteFloat(delayInSeconds)
                    .WriteBool(compensateLatency)
                    .Send();
            }

            if (volume > 0)
            {
                volume = Mathf.Clamp(volume, 0, 1);
                AudioSource.volume = volume;
            }
            
            AudioSource.PlayDelayed(delayInSeconds);
        }

        public void PlayOneShot(int clipHash, float volumeScale = 1.0f)
        {
            EnsureNetworkAudioClipsIsSet();
            AudioClip clip = clips.GetAudioClip(clipHash);
            EnsureClipIsNotNull(clip);
            InternalPlayOneShot(clip, clipHash, volumeScale);
        }

        public void PlayOneShot(string clipName, float volumeScale = 1.0f)
        {
            PlayOneShot(ComputeClipHashCode(clipName), volumeScale);
        }
        
        private void InternalPlayOneShot(AudioClip clip, int clipHash, float volumeScale)
        {
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.OneShot)
                    .WriteInt(clipHash)
                    .WriteFloat(volumeScale)
                    .Send();
            }

            AudioSource.volume = volumeScale;
            AudioSource.PlayOneShot(clip);
        }

        public void SetClip(int clipHash)
        {
            EnsureNetworkAudioClipsIsSet();
            AudioClip clip = clips.GetAudioClip(clipHash);
            EnsureClipIsNotNull(clip);
            
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration))
            {
                builder.WriteByte(AudioSourceActionId.SetClip)
                    .WriteInt(clipHash)
                    .Send();
            }

            AudioSource.clip = clip;
        }

        public void SetClip(string clipName) => SetClip(ComputeClipHashCode(clipName));

        private void EnsureClipIsSet()
        {
            if (AudioSource.clip == null)
                throw new MissingComponentException("You are trying to play AudioSource which has no clip chosen! Set default clip in the editor or call SetClip(...) method!");
        }

        private void EnsureNetworkAudioClipsIsSet()
        {
            if(clips == null)
                throw new MissingComponentException("You are trying to chose AudioClip without having NetworkAudioClips component attached!");
        }

        private void EnsureClipIsNotNull(AudioClip clip)
        {
            if (clip == null)
                throw new ArgumentException("Clip with this hash/name is not registered in assigned NetworkAudioClips object!");
        }
        
        #endregion

=======
        
>>>>>>> Stashed changes

        #region Other functionalities

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ComputeClipHashCode(string clipName)
        {
            return NetworkAudioSyncUtils.GetPlatformStableHashCode(clipName);
        }
        
        // Returns state of assigned AudioSource
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private State GetState()
        {
            if (NetworkAudioSyncManager.AudioSourceStates.TryGetValue(AudioSource, out State value))
                return value;

            return null;
        }
        
        // Represents AudioSource state
        internal sealed class State
        {
            public bool IsPaused { get; set; }
        }

        #endregion
    }
}