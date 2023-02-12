using System;
using System.Runtime.CompilerServices;
using LambdaTheDev.NetworkAudioSync.InternalNetworking;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync
{
    // Unity's AudioSource method wrappers
    public partial class NetworkAudioSource
    {
        [Obsolete("Instead of Pause() and UnPause() method, use Paused property!")]
        public void Pause() { Paused = true; }
        [Obsolete("Instead of Pause() and UnPause() method, use Paused property!")]
        public void UnPause() { Paused = false; }

        public void Stop()
        {
            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.Stop)
                .Send();
        }

        public void Play(int delay = 0)
        {
            EnsureUnityClipIsSet();
            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.Play)
                .WriteByte(AudioSourceActionId.PlayModes.Normal);

            // Delay present == false or true
            if (delay == 0) builder.WriteBool(false);
            else
            {
                builder.WriteBool(true);
                builder.WriteInt(delay);
            }
            
            // Send packet
            builder.Send();
        }

        public void PlayDelayed(float delayInSeconds, bool compensateLatency = true)
        {
            EnsureUnityClipIsSet();
            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.Play)
                .WriteByte(AudioSourceActionId.PlayModes.Delayed)
                .WriteFloat(delayInSeconds)
                .WriteBool(compensateLatency)
                .Send();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayOneShot(int clipHash, float volumeScale = 1.0f)
        { 
            InternalPlayOneShot(clipHash, volumeScale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void PlayOneShot(string clipName, float volumeScale = 1.0f)
        {
            PlayOneShot(ComputeClipHashCode(clipName), volumeScale);
        }

        private void InternalPlayOneShot(int clipHash, float volumeScale)
        {
            // Ensure that NAC is set
            EnsureNetworkAudioClipsIsSet();
            
            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.Play)
                .WriteByte(AudioSourceActionId.PlayModes.OneShot)
                .WriteInt(clipHash)
                .WriteFloat(volumeScale)
                .Send();
        }
        
        public void SetClip(int clipHash)
        {
            EnsureNetworkAudioClipsIsSet();

            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.SetClip)
                .WriteInt(clipHash)
                .Send();
        }

        public void SetClip(string clipName) => SetClip(ComputeClipHashCode(clipName));

        public void PlayClipAtPoint(int clipHash, Vector3 position, float volume = 1.0f)
        {
            using AudioPacketBuilder builder = NetworkAudioSyncPools.RentBuilder(Integration);
            builder.WriteByte(AudioSourceActionId.Play)
                .WriteByte(AudioSourceActionId.PlayModes.AtPoint)
                .WriteInt(clipHash)
                .WriteFloat(volume)
                .WriteVector3(position)
                .Send();
        }

        public void PlayClipAtPoint(string clipName, Vector3 position, float volume = 1.0f)
        {
            PlayClipAtPoint(ComputeClipHashCode(clipName), position, volume);
        }
        
        // Throws an exception if AudioSource's clip is set
        private void EnsureUnityClipIsSet()
        {
            if (AudioSource.clip == null)
                throw new MissingComponentException("You are trying to play AudioSource which has no clip chosen! Set default clip in the editor or call SetClip(...) method!");
        }

        // Throws an exception if NetworkAudioClips SO isn't attached
        private void EnsureNetworkAudioClipsIsSet()
        {
            if(clips == null)
                throw new MissingComponentException("You are trying to chose AudioClip without having NetworkAudioClips component attached!");
        }

        // Returns AudioClip from assigned NetworkAudioClips with provided clip ID, or throws an exception 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private AudioClip GetAudioClipByHashOrThrow(int clipHash)
        {
            AudioClip clip = clips.GetAudioClip(clipHash);
            if (clip == null)
            {
                ThrowClipNotRegistered();
                return null;
            }

            return clip;
        }

        // Just throws an exception
        private static void ThrowClipNotRegistered()
        {
            throw new NullReferenceException("Clip with this hash/name is not registered in assigned NetworkAudioClips object!");
        }
    }
}