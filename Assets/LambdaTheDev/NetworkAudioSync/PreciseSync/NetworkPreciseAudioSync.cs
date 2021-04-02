using System;
using Mirror;
using UnityEngine;

namespace LambdaTheDev.NetworkAudioSync.PreciseSync
{
    [RequireComponent(typeof(NetworkAudioClips))]
    public class NetworkPreciseAudioSync : NetworkBehaviour
    {
        public AudioSource source;
        public NetworkAudioClips clips;

        private void Start()
        {
            if (clips == null)
                throw new NullReferenceException("You haven't provided NetworkAudioClips component for this NetworkPreciseAudioSync!");
            
            if (source == null)
                throw new NullReferenceException("You haven't assigned AudioSource for this NetworkPreciseAudioSync!");
            
        }

        [Server]
        public void SyncAudio(AudioClip clip)
        {
            byte targetClip = clips.GetClipId(clip);
            PreciseSyncRequest request = new PreciseSyncRequest
            {
                Clip = targetClip,
                RequestTime = NetworkTime.time
            };
            
            RpcPlayAudio(request);
        }

        [ClientRpc]
        void RpcPlayAudio(PreciseSyncRequest request)
        {
            double offset = NetworkTime.time - request.RequestTime;
            AudioClip clip = clips.GetAudioClip(request.Clip);

            if (clip.length > offset) return;

            source.clip = clip;
            source.time = (float) offset;
            source.Play();
        }
    }
}