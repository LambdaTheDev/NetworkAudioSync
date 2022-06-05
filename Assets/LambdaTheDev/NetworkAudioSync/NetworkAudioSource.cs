using System;
using System.IO;
using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.Integrations;
using LambdaTheDev.NetworkAudioSync.InternalNetworking;
using UnityEngine;
using UnityEngine.Audio;

namespace LambdaTheDev.NetworkAudioSync
{
    // Allows AudioSource settings synchronization over network
    public class NetworkAudioSource : MonoBehaviour
    {
        // Synchronized AudioSource
        [SerializeField] private AudioSource source;

        // Audio clips used by this NAS instance
        [SerializeField] private NetworkAudioClips clips;

        // Chosen networking integration
        private INetworkAudioSyncIntegration _integration;


        private void Awake()
        {
            // Validate component
            if (source == null)
                throw new NullReferenceException("AudioSource bound to this NetworkAudioSource must be set!");

            if (clips == null)
                Debug.LogWarning("NetworkAudioClips instance has not been assigned for this NetworkAudioSource. This is not an error, but you will be unable to play other AudioClips than default one!");
            else
                clips.Initialize();

            // Initialize integration
            _integration = GetComponent<INetworkAudioSyncIntegration>();
            if (_integration == null)
                throw new MissingComponentException("Could not locate networking integration component! It is required to use NetworkAudioSource functionalities. They are usually named like: NetworkAudioSync(NETWORKING_SOLUTION_NAME)!");
            
            _integration.BindPacketCallback(OnNetworkPacket);
            
            // Register this AudioSource
            if (!NetworkAudioSyncManager.AudioSourceStates.ContainsKey(source))
            {
                NetworkAudioSyncManager.AudioSourceStates.Add(source, new State());
                source.UnPause(); // Ensure that saved state matches real state.
                                  // Note: Maybe make ApplyState method & make it serializable
            }
        }

        private void OnDestroy()
        {
            // When object is destroyed, unregister source
            if (source != null)
                NetworkAudioSyncManager.AudioSourceStates.Remove(source);
        }

        #region Internal networking processor

        private void OnNetworkPacket(ArraySegment<byte> packet)
        {
            // Server has already applied settings. This shouldn't happen tbh
            if (_integration.IsServer) return;

            using AudioPacketReader reader = NetworkAudioSyncPools.PacketReaderPool.Rent();
            byte packetId = reader.ReadByte();

            switch (packetId)
            {
                #region Methods impl
                    
                case AudioSourceActionId.Play:
                    byte playMode = reader.ReadByte();
                    switch (playMode)
                    {
                        case AudioSourceActionId.PlayModes.Normal:
                            source.Play();
                            break;
                            
                        case AudioSourceActionId.PlayModes.Delayed:
                            float delay = reader.ReadFloat();
                            bool compensate = reader.ReadBool();

                            if (compensate)
                                delay = Mathf.Clamp(delay - _integration.ClientLatency, 0, float.MaxValue);

                            source.PlayDelayed(delay);
                            break;
                            
                        case AudioSourceActionId.PlayModes.OneShot:
                            int oneShotClipHash = reader.ReadInt();
                            float volumeScale = reader.ReadFloat();
                            EnsureNetworkAudioClipsIsSet();
                            AudioClip oneShotClip = clips.GetAudioClip(oneShotClipHash);
                            EnsureClipIsNotNull(oneShotClip);
                                
                            source.PlayOneShot(oneShotClip, volumeScale);
                            break;
                            
                        default:
                            throw new InvalidDataException("Invalid packet arrived!");
                    }

                    break;
                    
                case AudioSourceActionId.Stop:
                    source.Stop();
                    break;
                    
                case AudioSourceActionId.SetClip:
                    int setClipHash = reader.ReadInt();
                    EnsureNetworkAudioClipsIsSet();
                    AudioClip setClip = clips.GetAudioClip(setClipHash);
                    EnsureClipIsNotNull(setClip);
                    source.clip = setClip;
                    break;
                    
                #endregion


                #region Properties impl

                case AudioSourceActionId.BypassEffects:
                    source.bypassEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.BypassListenerEffects:
                    source.bypassListenerEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.BypassReverbZones:
                    source.bypassReverbZones = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.DopplerLevel:
                    source.dopplerLevel = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.GamepadSpeakerOutputType:
                    source.gamepadSpeakerOutputType = (GamepadSpeakerOutputType)reader.ReadByte();
                    break;
                    
                case AudioSourceActionId.IgnoreListenerPause:
                    source.ignoreListenerPause = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.IgnoreListenerVolume:
                    source.ignoreListenerVolume = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.Loop:
                    source.loop = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.MaxDistance:
                    source.maxDistance = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.MinDistance:
                    source.minDistance = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Mute:
                    source.mute = reader.ReadBool();
                    break;
                    
                // TODO: Create OutputAudioMixerGroup support
                    
                case AudioSourceActionId.PanStereo:
                    source.panStereo = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Pitch:
                    source.pitch = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Priority:
                    source.priority = reader.ReadInt();
                    break;
                    
                case AudioSourceActionId.ReverbZoneMix:
                    source.reverbZoneMix = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.RolloffMode:
                    source.rolloffMode = (AudioRolloffMode)reader.ReadByte();
                    break;
                    
                case AudioSourceActionId.SpatialBlend:
                    source.spatialBlend = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Spatialize:
                    source.spatialize = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.SpatializePostEffects:
                    source.spatializePostEffects = reader.ReadBool();
                    break;
                    
                case AudioSourceActionId.Spread:
                    source.spread = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.Time:
                    source.time = reader.ReadFloat();
                    break;
                    
                case AudioSourceActionId.TimeSamples:
                    source.timeSamples = reader.ReadInt();
                    break;
                    
                case AudioSourceActionId.VelocityUpdateMode:
                    source.velocityUpdateMode = (AudioVelocityUpdateMode)reader.ReadByte();
                    break;
                    
                case AudioSourceActionId.Volume:
                    source.volume = reader.ReadFloat();
                    break;

                #endregion
                    
                default:
                    throw new InvalidDataException("Invalid packet arrived!");
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
            get => source.bypassEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassEffects)
                        .WriteBool(value).Send();
                }
                source.bypassEffects = value;
            }
        }
        
        public bool BypassListenerEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.bypassListenerEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassListenerEffects)
                        .WriteBool(value).Send();
                }
                source.bypassListenerEffects = value;
            }
        }

        public bool BypassReverbZones
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.bypassReverbZones;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.BypassReverbZones)
                        .WriteBool(value).Send();
                }
                source.bypassReverbZones = value;
            }
        }

        public float DopplerLevel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.dopplerLevel;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.DopplerLevel)
                        .WriteFloat(value).Send();
                }
                source.dopplerLevel = value;
            }
        }

        public GamepadSpeakerOutputType GamepadSpeakerOutputType
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.gamepadSpeakerOutputType;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.GamepadSpeakerOutputType)
                        .WriteByte((byte)value).Send();
                }
                source.gamepadSpeakerOutputType = value;
            }
        }

        public bool IgnoreListenerPause
        { 
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.ignoreListenerPause;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.IgnoreListenerPause)
                        .WriteBool(value).Send();
                }
                source.ignoreListenerPause = value;
            }
        }

        public bool IgnoreListenerVolume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.ignoreListenerVolume;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.IgnoreListenerVolume)
                        .WriteBool(value).Send();
                }
                source.ignoreListenerVolume = value;
            }
        }

        public bool Loop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.loop;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Loop)
                        .WriteBool(value).Send();
                }
                source.loop = value;
            }
        }

        public float MaxDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.maxDistance;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.MaxDistance)
                        .WriteFloat(value).Send();
                }
                source.maxDistance = value;
            }
        }

        public float MinDistance
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.minDistance;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.MinDistance)
                        .WriteFloat(value).Send();
                }
                source.minDistance = value;
            }
        }

        public bool Mute
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.mute;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Mute)
                        .WriteBool(value).Send();
                }
                source.mute = value;
            }
        }

        public AudioMixerGroup OutputAudioMixerGroup
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.outputAudioMixerGroup;
            set
            {
                throw new NotImplementedException("Right now I have no idea how to synchronize it over network, but I will find out!");
            }
        }

        public float PanStereo
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.panStereo;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.PanStereo)
                        .WriteFloat(value).Send();
                }
                source.panStereo = value;
            }
        }

        public float Pitch
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.pitch;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Pitch)
                        .WriteFloat(value).Send();
                }
                source.pitch = value;
            }
        }

        public int Priority
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.priority;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Priority)
                        .WriteInt(value).Send();
                }
                source.priority = value;
            }
        }

        public float ReverbZoneMix
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.reverbZoneMix;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.ReverbZoneMix)
                        .WriteFloat(value).Send();
                }
                source.reverbZoneMix = value;
            }
        }

        public AudioRolloffMode RolloffMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.rolloffMode;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.RolloffMode)
                        .WriteByte((byte)value).Send();
                }
                source.rolloffMode = value;
            }
        }

        public float SpatialBlend
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.spatialBlend;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.SpatialBlend)
                        .WriteFloat(value).Send();
                }
                source.spatialBlend = value;
            }
        }

        public bool Spatialize
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.spatialize;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Spatialize)
                        .WriteBool(value).Send();
                }
                source.spatialize = value;
            }
        }

        public bool SpatializePostEffects
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.spatializePostEffects;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.SpatializePostEffects)
                        .WriteBool(value).Send();
                }
                source.spatializePostEffects = value;
            }
        }

        public float Spread
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.spread;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Spread)
                        .WriteFloat(value).Send();
                }
                source.spread = value;
            }
        }

        public float Time
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.time;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Time)
                        .WriteFloat(value).Send();
                }
                source.time = value;
            }
        }

        public int TimeSamples
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.timeSamples;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.TimeSamples)
                        .WriteInt(value).Send();
                }
                source.timeSamples = value;
            }
        }

        public AudioVelocityUpdateMode VelocityUpdateMode
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.velocityUpdateMode;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.VelocityUpdateMode)
                        .WriteByte((byte)value).Send();
                }
                source.velocityUpdateMode = value;
            }
        }

        public float Volume
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => source.volume;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Volume)
                        .WriteFloat(value).Send();
                }
                source.volume = value;
            }
        }

        public bool Paused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => GetState().IsPaused;
            set
            {
                using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
                {
                    builder.WriteByte(AudioSourceActionId.Pause)
                        .WriteBool(value).Send();
                }
                if(value) source.Pause();
                else source.UnPause();
                
                
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
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
            {
                builder.WriteByte(AudioSourceActionId.Stop).Send();
            }
            source.Stop();
        }

        public void Play()
        {
            EnsureClipIsSet();
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.Normal)
                    .Send();
            }

            source.Play();
        }

        public void PlayDelayed(float delayInSeconds, bool compensateLatency = true)
        {
            EnsureClipIsSet();
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.Delayed)
                    .WriteFloat(delayInSeconds)
                    .WriteBool(compensateLatency)
                    .Send();
            }

            source.PlayDelayed(delayInSeconds);
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
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
            {
                builder.WriteByte(AudioSourceActionId.Play)
                    .WriteByte(AudioSourceActionId.PlayModes.OneShot)
                    .WriteInt(clipHash)
                    .WriteFloat(volumeScale)
                    .Send();
            }

            source.PlayOneShot(clip);
        }

        public void SetClip(int clipHash)
        {
            EnsureNetworkAudioClipsIsSet();
            AudioClip clip = clips.GetAudioClip(clipHash);
            EnsureClipIsNotNull(clip);
            
            using (AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(_integration))
            {
                builder.WriteByte(AudioSourceActionId.SetClip)
                    .WriteInt(clipHash)
                    .Send();
            }

            source.clip = clip;
        }

        public void SetClip(string clipName) => SetClip(ComputeClipHashCode(clipName));

        private void EnsureClipIsSet()
        {
            if (source.clip == null)
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
            if (NetworkAudioSyncManager.AudioSourceStates.TryGetValue(source, out State value))
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