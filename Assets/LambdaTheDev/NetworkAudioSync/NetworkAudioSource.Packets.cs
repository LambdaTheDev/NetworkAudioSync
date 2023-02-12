using System;
using LambdaTheDev.NetworkAudioSync.InternalNetworking;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Partial class for parsing incoming packets
    public partial class NetworkAudioSource
    {
        private void OnNetworkPacket(ArraySegment<byte> packet)
        {
            try
            {
                InnerPacketsProcessor(packet);
            }
            catch (Exception e)
            {
                Debug.LogWarning("NetworkAudioSource's packet processor has reported an error!");
                Debug.LogException(e);
            }
        }
        
        private void InnerPacketsProcessor(ArraySegment<byte> packet)
        {
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
                            AudioClip oneShotClip = GetAudioClipByHashOrThrow(oneShotClipHash);

                            AudioSource.clip = oneShotClip;
                            AudioSource.volume = volumeScale;
                            AudioSource.PlayOneShot(oneShotClip, volumeScale);
                            break;
                            
                        case AudioSourceActionId.PlayModes.AtPoint:
                            // Prepare data
                            int clipHash = reader.ReadInt();
                            float volume = reader.ReadFloat();
                            Vector3 position = reader.ReadVector3();

                            AudioClip atPointClip = clips.GetAudioClip(clipHash);
                            // ReSharper disable once RedundantNameQualifier
                            UnityEngine.AudioSource.PlayClipAtPoint(atPointClip, position, volume);
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
                    AudioClip setClip = GetAudioClipByHashOrThrow(setClipHash);
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
                
                case AudioSourceActionId.Pause:
                    bool pause = reader.ReadBool();
                    if(pause) AudioSource.Pause();
                    else AudioSource.UnPause();
                    break;

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
    }
}